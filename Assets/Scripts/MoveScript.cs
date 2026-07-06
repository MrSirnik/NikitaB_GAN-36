using DG.Tweening;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    // один маршрут: A-B + доп. пути из _extraPaths
    [System.Serializable]
    public class MovementPath
    {
        public string pathName = "Path";
        public Transform[] waypoints;
    }

    [Header("Точки основного маршрута (обязательные)")]
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;

    [Header("Дополнительные маршруты (бонус: выбор пути игроком)")]
    [SerializeField] private MovementPath[] _extraPaths;
    [SerializeField]
    private KeyCode[] _pathSelectKeys =
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4
    };

    [Header("Параметры движения")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private Ease _changeCurve = Ease.InOutSine;
    [SerializeField] private float _delayBetweenLegs = 0f;

    [Header("Эффект сжатия/растяжения при движении")]
    [SerializeField] private float _stretchScale = 2f;
    [SerializeField] private float _normalScale = 1f;
    [SerializeField] private float _scaleTweenDuration = 1f;

    [Header("Эффект смены цвета при движении")]
    [SerializeField] private Color _colorToA = Color.white;
    [SerializeField] private Color _colorToB = Color.cyan;

    [Header("Пыльный след при движении")]
    [SerializeField] private Color _trailColor = new Color(0.6f, 0.5f, 0.35f, 0.6f);
    [SerializeField] private float _trailTime = 0.4f;

    [Header("Управление скоростью (бонус)")]
    [SerializeField] private float _minSpeedMultiplier = 0.25f;
    [SerializeField] private float _maxSpeedMultiplier = 3f;
    [SerializeField] private float _speedChangeRate = 1.5f;

    [Header("Атака (бонус)")]
    [SerializeField] private KeyCode _attackKey = KeyCode.Space;
    [SerializeField] private float _attackDuration = 0.35f;
    [SerializeField] private float _attackPunchScale = 0.4f;
    [SerializeField] private Color _attackFlashColor = Color.red;

    private Material[] _materials;
    private Sequence _pathSequence;
    private Sequence _attackSequence;
    private TrailRenderer _trail;

    private Transform[] _activeWaypoints;
    private int _targetIndex;
    private int _direction = 1;
    private float _speedMultiplier = 1f;

    private void Awake()
    {
        // .material клонирует материал под себя, чтобы DOColor не красил чужие объекты
        var renderers = GetComponentsInChildren<Renderer>();
        _materials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            _materials[i] = renderers[i].material;
        }

        SetupTrail();
    }

    private void Start()
    {
        SelectPath(0);
    }

    private void Update()
    {
        // только ввод, без DOMove/DOScale — раньше твины плодились тут каждый кадр, поэтому и лагало
        HandlePathSelectionInput();
        HandleSpeedInput();
        HandleAttackInput();
    }

    private void SetupTrail()
    {
        // след пыли без ассетов — TrailRenderer + встроенный шейдер
        _trail = gameObject.AddComponent<TrailRenderer>();
        _trail.time = _trailTime;
        _trail.startWidth = 0.3f;
        _trail.endWidth = 0.05f;
        _trail.material = new Material(Shader.Find("Sprites/Default"));
        _trail.startColor = _trailColor;
        _trail.endColor = new Color(_trailColor.r, _trailColor.g, _trailColor.b, 0f); // хвост уходит в прозрачность
        _trail.minVertexDistance = 0.05f;
        _trail.emitting = true;
    }

    private MovementPath GetPath(int index)
    {
        // индекс 0 — старый A-B, чтобы не пересобирать сцену; остальное из _extraPaths
        if (index == 0)
        {
            return new MovementPath { pathName = "A-B", waypoints = new[] { _pointA, _pointB } };
        }

        int extraIndex = index - 1;
        if (_extraPaths != null && extraIndex >= 0 && extraIndex < _extraPaths.Length)
        {
            return _extraPaths[extraIndex];
        }

        return null;
    }

    private int PathCount => 1 + (_extraPaths?.Length ?? 0);

    private void HandlePathSelectionInput()
    {
        for (int i = 0; i < _pathSelectKeys.Length && i < PathCount; i++)
        {
            if (Input.GetKeyDown(_pathSelectKeys[i]))
            {
                SelectPath(i);
                break;
            }
        }
    }

    private void HandleSpeedInput()
    {
        // "Vertical" — это и W/S, и стрелки, и стик джойстика сразу
        float axis = Input.GetAxis("Vertical");
        if (Mathf.Approximately(axis, 0f)) return;

        _speedMultiplier = Mathf.Clamp(
            _speedMultiplier + axis * _speedChangeRate * Time.deltaTime,
            _minSpeedMultiplier,
            _maxSpeedMultiplier);

        // timeScale меняет скорость уже запущенного твина на лету, без пересоздания
        if (_pathSequence != null && _pathSequence.IsActive())
        {
            _pathSequence.timeScale = _speedMultiplier;
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(_attackKey))
        {
            PlayAttack();
        }
    }

    private void SelectPath(int index)
    {
        MovementPath path = GetPath(index);
        if (path == null || path.waypoints == null || path.waypoints.Length < 2)
        {
            Debug.LogWarning($"MoveScript: путь #{index} не настроен (нужно минимум 2 точки).");
            return;
        }

        _pathSequence?.Kill();
        _attackSequence?.Kill();

        _activeWaypoints = path.waypoints;
        transform.position = _activeWaypoints[0].position; // телепорт на старт, без анимации
        _targetIndex = 1;
        _direction = 1;

        MoveToNextWaypoint();
    }

    // одна нога = одна Sequence, следующая стартует только по OnComplete — наложений быть не может
    private void MoveToNextWaypoint()
    {
        Transform target = _activeWaypoints[_targetIndex];
        float distance = Vector3.Distance(transform.position, target.position);
        float legDuration = distance / Mathf.Max(_moveSpeed, 0.01f); // _moveSpeed — юниты/сек

        // туда — растяжка + цвет B, обратно — наоборот
        bool goingToB = _direction > 0;
        float stretch = goingToB ? _stretchScale : _normalScale;
        float squash = goingToB ? _normalScale : _stretchScale;
        Color legColor = goingToB ? _colorToB : _colorToA;

        _pathSequence = DOTween.Sequence();
        _pathSequence.Append(transform.DOMove(target.position, legDuration).SetEase(_changeCurve));
        _pathSequence.Join(transform.DOScaleY(stretch, _scaleTweenDuration)); // Join — параллельно с DOMove
        _pathSequence.Join(transform.DOScaleZ(squash, _scaleTweenDuration));
        foreach (var material in _materials)
        {
            _pathSequence.Join(material.DOColor(legColor, legDuration));
        }

        _pathSequence.AppendInterval(_delayBetweenLegs);
        _pathSequence.timeScale = _speedMultiplier;
        _pathSequence.OnComplete(AdvanceWaypoint); // следующая нога только отсюда
    }

    private void AdvanceWaypoint()
    {
        // разворот на краях маршрута, работает для любого числа точек
        if (_targetIndex >= _activeWaypoints.Length - 1)
        {
            _direction = -1;
        }
        else if (_targetIndex <= 0)
        {
            _direction = 1;
        }

        _targetIndex += _direction;
        MoveToNextWaypoint();
    }

    private void PlayAttack()
    {
        if (_attackSequence != null && _attackSequence.IsActive()) return;

        _pathSequence?.Pause(); // Pause, не Kill — движение просто замирает и потом продолжится с того же места

        _attackSequence = DOTween.Sequence();
        _attackSequence.Append(transform.DOPunchScale(Vector3.one * _attackPunchScale, _attackDuration, vibrato: 6, elasticity: 0.5f));
        foreach (var material in _materials)
        {
            _attackSequence.Join(material.DOColor(_attackFlashColor, _attackDuration * 0.3f).SetLoops(2, LoopType.Yoyo)); // вспышка на удар
        }
        _attackSequence.OnComplete(() => _pathSequence?.Play());
    }

    private void OnDestroy()
    {
        // без Kill() тут DOTween будет ругаться на твины уничтоженного объекта
        _pathSequence?.Kill();
        _attackSequence?.Kill();
    }
}
