using UnityEngine;
using UnityEngine.EventSystems;

namespace Tactics
{
    public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private static Material _sharedCrownMaterial;

        [SerializeField] private Team _team;
        [SerializeField] private UnitType _type = UnitType.Man;
        [SerializeField] private CheckersSettings _settings;
        [SerializeField] private Material _bodyMaterial;

        private Renderer _bodyRenderer;
        private Transform _crown;

        public Cell CurrentCell => transform.parent != null ? transform.parent.GetComponent<Cell>() : null;
        public Team Team => _team;
        public UnitType Type => _type;

        // Вызывается только при бейке сцены в редакторе - создаёт визуал и ставит фишку на клетку.
        public void Bake(Cell cell, Team team, CheckersSettings settings, Material bodyMaterial)
        {
            _team = team;
            _type = UnitType.Man;
            _settings = settings;
            _bodyMaterial = bodyMaterial;

            transform.SetParent(cell.transform, false);
            cell.OccupyingUnit = this;

            BuildVisual();
            SnapToCell();
        }

        public void Awake()
        {
            _bodyRenderer = transform.Find("Body")?.GetComponent<Renderer>();
            _crown = transform.Find("Crown");
        }

        public void PlaceOnCell(Cell cell)
        {
            CurrentCell.OccupyingUnit = null;
            transform.SetParent(cell.transform, false);
            cell.OccupyingUnit = this;
            SnapToCell();
        }

        public void PromoteToKing()
        {
            if (_type == UnitType.King) return;

            _type = UnitType.King;
            _crown.gameObject.SetActive(true);
        }

        public void Capture()
        {
            if (CurrentCell.OccupyingUnit == this)
            {
                CurrentCell.OccupyingUnit = null;
            }

            Destroy(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData) => CurrentCell.OnPointerEnter(eventData);
        public void OnPointerExit(PointerEventData eventData) => CurrentCell.OnPointerExit(eventData);
        public void OnPointerClick(PointerEventData eventData) => CurrentCell.OnPointerClick(eventData);

        private void SnapToCell()
        {
            transform.position = CurrentCell.transform.position + Vector3.up * (_settings.manHeight * 0.5f);
        }

        private void BuildVisual()
        {
            var body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            body.name = "Body";
            body.transform.SetParent(transform, false);
            body.transform.localScale = new Vector3(_settings.manRadius, _settings.manHeight * 0.5f, _settings.manRadius);
            // Коллайдер визуального дочернего объекта не нужен - клики должен ловить сам Unit (см. ниже).
            Object.DestroyImmediate(body.GetComponent<Collider>());

            _bodyRenderer = body.GetComponent<Renderer>();
            _bodyRenderer.sharedMaterial = _bodyMaterial;

            // Коллайдер вешаем на корневой объект - там же, где сам компонент Unit,
            // иначе PhysicsRaycaster найдёт коллайдер на дочернем "Body" и IPointerClickHandler на Unit не вызовется.
            var collider = gameObject.AddComponent<CapsuleCollider>();
            collider.radius = _settings.manRadius;
            collider.height = _settings.manHeight;

            var crownObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            crownObject.name = "Crown";
            crownObject.transform.SetParent(transform, false);
            crownObject.transform.localPosition = Vector3.up * _settings.manHeight;
            crownObject.transform.localScale = Vector3.one * _settings.kingCrownScale;
            Object.DestroyImmediate(crownObject.GetComponent<Collider>());
            if (_sharedCrownMaterial == null)
            {
                _sharedCrownMaterial = new Material(crownObject.GetComponent<Renderer>().sharedMaterial) { color = Color.yellow };
            }
            crownObject.GetComponent<Renderer>().sharedMaterial = _sharedCrownMaterial;
            crownObject.SetActive(false);
            _crown = crownObject.transform;
        }
    }
}
