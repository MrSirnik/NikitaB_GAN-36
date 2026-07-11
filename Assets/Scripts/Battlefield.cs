using System.Collections.Generic;
using UnityEngine;

namespace Tactics
{
    public class Battlefield : MonoBehaviour
    {
        [SerializeField] private BoardSettings _settings;

        private readonly Dictionary<int, Cell> _cellByCoordinate = new();
        private readonly List<Cell> _highlightedCells = new();

        public int Size => _settings.size;

        // Вызывается только при бейке сцены в редакторе - создаёт клетки и фишки как настоящие объекты сцены.
        public void Bake(BoardSettings settings, CheckersSettings checkersSettings, BattleController battleController,
            Material lightMaterial, Material darkMaterial, Material player1Material, Material player2Material)
        {
            _settings = settings;

            for (int x = 0; x < settings.size; x++)
            {
                for (int y = 0; y < settings.size; y++)
                {
                    bool isDark = (x + y) % 2 == 0;

                    var cellObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cellObject.name = $"Cell_{x}_{y}";
                    cellObject.transform.SetParent(transform, false);
                    cellObject.transform.localPosition = new Vector3(x * settings.cellSize, 0f, y * settings.cellSize);
                    cellObject.transform.localScale = new Vector3(settings.cellSize, 0.2f, settings.cellSize);

                    Cell cell = cellObject.AddComponent<Cell>();
                    cell.Bake(x, y, isDark, isDark ? darkMaterial : lightMaterial, settings, battleController);
                    _cellByCoordinate[CoordinateKey(x, y)] = cell;
                }
            }

            SpawnCheckers(checkersSettings, player1Material, player2Material);
        }

        // Восстанавливает быструю выборку клеток по координатам после загрузки сцены - клетки уже существуют,
        // просто индексируем их заново (дёшево, без создания новых объектов).
        public void Awake()
        {
            _cellByCoordinate.Clear();
            foreach (Cell cell in GetComponentsInChildren<Cell>(true))
            {
                _cellByCoordinate[CoordinateKey(cell.X, cell.Y)] = cell;
            }
        }

        public Cell GetCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Size || y >= Size) return null;
            return _cellByCoordinate.TryGetValue(CoordinateKey(x, y), out Cell cell) ? cell : null;
        }

        public IEnumerable<Unit> GetUnitsOfTeam(Team team)
        {
            foreach (Cell cell in _cellByCoordinate.Values)
            {
                if (cell.OccupyingUnit != null && cell.OccupyingUnit.Team == team)
                {
                    yield return cell.OccupyingUnit;
                }
            }
        }

        public void Highlight(Cell cell, GameStatus status)
        {
            cell.SetHighlight(status);
            _highlightedCells.Add(cell);
        }

        public void ClearHighlights()
        {
            foreach (Cell cell in _highlightedCells)
            {
                cell.SetHighlight(GameStatus.None);
            }
            _highlightedCells.Clear();
        }

        private int CoordinateKey(int x, int y) => x * Size + y;

        private void SpawnCheckers(CheckersSettings checkersSettings, Material player1Material, Material player2Material)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    if ((x + y) % 2 != 0) continue;
                    SpawnUnit(GetCell(x, y), Team.Player1, checkersSettings, player1Material);
                }
            }

            for (int y = Size - 3; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    if ((x + y) % 2 != 0) continue;
                    SpawnUnit(GetCell(x, y), Team.Player2, checkersSettings, player2Material);
                }
            }
        }

        private void SpawnUnit(Cell cell, Team team, CheckersSettings checkersSettings, Material material)
        {
            var unitObject = new GameObject($"Unit_{team}_{cell.X}_{cell.Y}");
            Unit unit = unitObject.AddComponent<Unit>();
            unit.Bake(cell, team, checkersSettings, material);
        }
    }
}
