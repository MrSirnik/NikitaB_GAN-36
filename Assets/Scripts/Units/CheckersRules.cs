using System.Collections.Generic;

namespace Tactics
{
    // Правила ходов шашек: обычная фишка, дамка, обязательная атака, рекурсивный поиск атак.
    public class CheckersRules
    {
        private static readonly (int dx, int dy)[] Diagonals = { (1, 1), (1, -1), (-1, 1), (-1, -1) };

        private readonly Battlefield _battlefield;

        public CheckersRules(Battlefield battlefield)
        {
            _battlefield = battlefield;
        }

        public bool TeamHasCapture(Team team)
        {
            foreach (Unit unit in _battlefield.GetUnitsOfTeam(team))
            {
                if (GetMoves(unit, captureOnly: true).Count > 0) return true;
            }
            return false;
        }

        public List<CheckersMove> GetLegalMoves(Unit unit, bool teamMustCapture)
        {
            return GetMoves(unit, captureOnly: teamMustCapture);
        }

        public bool IsPromotionRow(Team team, int y)
        {
            int lastRow = _battlefield.Size - 1;
            return team == Team.Player1 ? y == lastRow : y == 0;
        }

        private List<CheckersMove> GetMoves(Unit unit, bool captureOnly)
        {
            var moves = new List<CheckersMove>();

            if (unit.Type == UnitType.King)
            {
                CollectKingMoves(unit, moves, captureOnly);
            }
            else
            {
                CollectManMoves(unit, moves, captureOnly);
            }

            return moves;
        }

        private void CollectManMoves(Unit unit, List<CheckersMove> moves, bool captureOnly)
        {
            int forward = unit.Team == Team.Player1 ? 1 : -1;
            Cell from = unit.CurrentCell;

            foreach ((int dx, int dy) in Diagonals)
            {
                Cell step1 = _battlefield.GetCell(from.X + dx, from.Y + dy);
                if (step1 == null) continue;

                if (step1.IsEmpty)
                {
                    bool isForward = dy == forward;
                    if (isForward && !captureOnly)
                    {
                        moves.Add(new CheckersMove(step1, null));
                    }
                    continue;
                }

                if (step1.OccupyingUnit.Team == unit.Team) continue;

                Cell landing = _battlefield.GetCell(from.X + dx * 2, from.Y + dy * 2);
                if (landing != null && landing.IsEmpty)
                {
                    moves.Add(new CheckersMove(landing, step1));
                }
            }
        }

        private void CollectKingMoves(Unit unit, List<CheckersMove> moves, bool captureOnly)
        {
            Cell from = unit.CurrentCell;

            foreach ((int dx, int dy) in Diagonals)
            {
                Cell enemyFound = null;
                int step = 1;

                while (true)
                {
                    Cell current = _battlefield.GetCell(from.X + dx * step, from.Y + dy * step);
                    if (current == null) break;

                    if (current.IsEmpty)
                    {
                        if (enemyFound == null)
                        {
                            if (!captureOnly) moves.Add(new CheckersMove(current, null));
                        }
                        else
                        {
                            moves.Add(new CheckersMove(current, enemyFound));
                        }
                    }
                    else if (current.OccupyingUnit.Team == unit.Team)
                    {
                        break;
                    }
                    else if (enemyFound != null)
                    {
                        break;
                    }
                    else
                    {
                        enemyFound = current;
                    }

                    step++;
                }
            }
        }
    }
}
