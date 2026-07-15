using System;
using System.Collections;
using UnityEngine;

namespace Tactics
{
    // Блокирует управление на время визуализации хода, визуализирует перемещение
    // и поглощение фишки, обрабатывает результат хода (превращение в дамку).
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CheckersSettings _settings;

        private CheckersRules _rules;

        public bool IsBusy { get; private set; }

        // Вызывается только при бейке сцены в редакторе - задаёт ссылку на настройки.
        public void Bake(CheckersSettings settings)
        {
            _settings = settings;
        }

        // Дёшево - вызывается каждый раз в рантайме после загрузки сцены (CheckersRules нельзя запечь, это не Unity-объект).
        public void WireRules(CheckersRules rules)
        {
            _rules = rules;
        }

        public void ExecuteMove(Unit unit, CheckersMove move, Action<Unit, CheckersMove> onComplete)
        {
            StartCoroutine(MoveRoutine(unit, move, onComplete));
        }

        private IEnumerator MoveRoutine(Unit unit, CheckersMove move, Action<Unit, CheckersMove> onComplete)
        {
            IsBusy = true;

            Cell fromCell = unit.CurrentCell;
            Vector3 startPosition = unit.transform.position;
            float heightOffset = startPosition.y - fromCell.transform.position.y;
            Vector3 endPosition = move.Destination.transform.position + Vector3.up * heightOffset;

            float elapsed = 0f;
            while (elapsed < _settings.moveDuration)
            {
                elapsed += Time.deltaTime;
                unit.transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / _settings.moveDuration);
                yield return null;
            }
            unit.transform.position = endPosition;

            if (move.IsCapture)
            {
                yield return new WaitForSeconds(_settings.captureRemoveDelay);
                move.CapturedCell.OccupyingUnit?.Capture();
            }

            unit.PlaceOnCell(move.Destination);

            if (_rules.IsPromotionRow(unit.Team, move.Destination.Y))
            {
                unit.PromoteToKing();
            }

            IsBusy = false;
            onComplete?.Invoke(unit, move);
        }
    }
}
