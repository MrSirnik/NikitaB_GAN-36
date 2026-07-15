using UnityEngine;
using UnityEngine.EventSystems;

namespace Tactics
{
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private int _x;
        [SerializeField] private int _y;
        [SerializeField] private bool _isDark;
        [SerializeField] private Material _baseMaterial;
        [SerializeField] private BoardSettings _settings;
        [SerializeField] private BattleController _battleController;

        public int X => _x;
        public int Y => _y;
        public bool IsDark => _isDark;
        public Unit OccupyingUnit { get; set; }
        public bool IsEmpty => OccupyingUnit == null;

        private Renderer _renderer;
        private GameStatus _highlight = GameStatus.None;
        private bool _isHovered;

        // Вызывается только при бейке сцены в редакторе - задаёт неизменяемые данные клетки.
        public void Bake(int x, int y, bool isDark, Material baseMaterial, BoardSettings settings, BattleController battleController)
        {
            _x = x;
            _y = y;
            _isDark = isDark;
            _baseMaterial = baseMaterial;
            _settings = settings;
            _battleController = battleController;
        }

        // Восстанавливает то, что не переживает сохранение сцены (текущий обитатель клетки).
        public void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _renderer.sharedMaterial = _baseMaterial;
            OccupyingUnit = GetComponentInChildren<Unit>(true);
        }

        public void SetHighlight(GameStatus status)
        {
            _highlight = status;
            RefreshVisual();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            RefreshVisual();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            RefreshVisual();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _battleController.HandleCellClicked(this);
        }

        private void RefreshVisual()
        {
            if (_highlight == GameStatus.None && !_isHovered)
            {
                _renderer.sharedMaterial = _baseMaterial;
                return;
            }

            Color color = _highlight switch
            {
                GameStatus.Select => _settings.selectColor,
                GameStatus.Move => _settings.moveColor,
                GameStatus.Pending => _settings.pendingColor,
                _ => _settings.hoverColor,
            };
            _renderer.material.color = color;
        }
    }
}
