using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactics
{
    // "Команда ТЗ": обрабатывает клики по клеткам/фишкам, регистрирует выбор фишки (Target)
    // и клетки назначения (Destination), исполняет ход по Confirm, учитывает обязательную
    // атаку и рекурсивный поиск атак.
    public class CheckersMoveCommand : IGameplayCommand
    {
        private readonly Battlefield _battlefield;
        private readonly CheckersRules _rules;
        private readonly PlayerController _playerController;

        private Team _currentTeam;
        private bool _teamMustCapture;
        private Unit _selectedUnit;
        private List<CheckersMove> _currentMoves = new();
        private CheckersMove? _pendingMove;
        private bool _lockedForRecapture;

        public event Action<Team> OnTurnChanged;

        public CheckersMoveCommand(Battlefield battlefield, CheckersRules rules, PlayerController playerController)
        {
            _battlefield = battlefield;
            _rules = rules;
            _playerController = playerController;
        }

        public void Initialize()
        {
            Team startingTeam = UnityEngine.Random.value < 0.5f ? Team.Player1 : Team.Player2;
            StartTurn(startingTeam);
        }

        public void Interact(Cell cell)
        {
            if (_playerController.IsBusy) return;

            if (_selectedUnit != null && cell.OccupyingUnit == _selectedUnit) return;

            if (_selectedUnit == null)
            {
                TrySelect(cell);
                return;
            }

            CheckersMove? matched = FindMove(cell);
            if (matched.HasValue)
            {
                SetPendingMove(matched.Value);
                return;
            }

            if (_lockedForRecapture) return;

            if (cell.OccupyingUnit != null && cell.OccupyingUnit.Team == _currentTeam)
            {
                TrySelect(cell);
            }
        }

        public void Confirm()
        {
            if (_playerController.IsBusy) return;
            if (_selectedUnit == null || !_pendingMove.HasValue) return;

            Unit unit = _selectedUnit;
            CheckersMove move = _pendingMove.Value;

            _battlefield.ClearHighlights();
            _selectedUnit = null;
            _pendingMove = null;
            _currentMoves.Clear();

            _playerController.ExecuteMove(unit, move, OnMoveComplete);
        }

        public void Cancel()
        {
            if (_playerController.IsBusy || _lockedForRecapture) return;

            _battlefield.ClearHighlights();
            _selectedUnit = null;
            _pendingMove = null;
            _currentMoves.Clear();

            if (_teamMustCapture)
            {
                HighlightForcedUnits();
            }
        }

        private void TrySelect(Cell cell)
        {
            if (cell.OccupyingUnit == null || cell.OccupyingUnit.Team != _currentTeam) return;

            List<CheckersMove> moves = _rules.GetLegalMoves(cell.OccupyingUnit, _teamMustCapture);
            if (moves.Count == 0) return;

            _battlefield.ClearHighlights();
            _selectedUnit = cell.OccupyingUnit;
            _currentMoves = moves;
            _pendingMove = null;

            _battlefield.Highlight(cell, GameStatus.Select);
            foreach (CheckersMove move in moves)
            {
                _battlefield.Highlight(move.Destination, GameStatus.Move);
            }
        }

        private CheckersMove? FindMove(Cell cell)
        {
            foreach (CheckersMove move in _currentMoves)
            {
                if (move.Destination == cell) return move;
            }
            return null;
        }

        private void SetPendingMove(CheckersMove move)
        {
            if (_pendingMove.HasValue)
            {
                _battlefield.Highlight(_pendingMove.Value.Destination, GameStatus.Move);
            }

            _pendingMove = move;
            _battlefield.Highlight(move.Destination, GameStatus.Pending);
        }

        private void OnMoveComplete(Unit unit, CheckersMove move)
        {
            if (move.IsCapture)
            {
                List<CheckersMove> continuation = _rules.GetLegalMoves(unit, teamMustCapture: true);
                if (continuation.Count > 0)
                {
                    _lockedForRecapture = true;
                    _selectedUnit = unit;
                    _currentMoves = continuation;
                    _pendingMove = null;

                    _battlefield.Highlight(unit.CurrentCell, GameStatus.Select);
                    foreach (CheckersMove next in continuation)
                    {
                        _battlefield.Highlight(next.Destination, GameStatus.Move);
                    }
                    return;
                }
            }

            _lockedForRecapture = false;
            StartTurn(_currentTeam == Team.Player1 ? Team.Player2 : Team.Player1);
        }

        private void StartTurn(Team team)
        {
            _currentTeam = team;
            _teamMustCapture = _rules.TeamHasCapture(team);
            OnTurnChanged?.Invoke(team);

            if (_teamMustCapture)
            {
                HighlightForcedUnits();
            }
        }

        private void HighlightForcedUnits()
        {
            foreach (Unit unit in _battlefield.GetUnitsOfTeam(_currentTeam))
            {
                if (_rules.GetLegalMoves(unit, teamMustCapture: true).Count > 0)
                {
                    _battlefield.Highlight(unit.CurrentCell, GameStatus.Select);
                }
            }
        }
    }
}
