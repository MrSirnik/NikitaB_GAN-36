using UnityEngine;
using UnityEngine.InputSystem;

namespace Tactics
{
    // Обрабатывает инпут игрока с клавиатуры (Cancel/Confirm через новый Input System)
    // и пересылает клики по клеткам/фишкам текущей команде.
    public class BattleController : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActions;

        private InputAction _cancelAction;
        private InputAction _confirmAction;
        private IGameplayCommand _command;
        private CheckersMoveCommand _checkersCommand;

        // Вызывается только при бейке сцены в редакторе - задаёт ссылку на ассет ввода.
        public void Bake(InputActionAsset inputActions)
        {
            _inputActions = inputActions;
        }

        // Дёшево - просто подписка на события, вызывается каждый раз в рантайме после загрузки сцены.
        public void WireCommand(CheckersMoveCommand command)
        {
            _checkersCommand = command;
            _command = command;

            InputActionMap map = _inputActions.FindActionMap("Gameplay");
            _cancelAction = map.FindAction("Cancel");
            _confirmAction = map.FindAction("Confirm");

            _cancelAction.performed += OnCancelPerformed;
            _confirmAction.performed += OnConfirmPerformed;

            map.Enable();
        }

        private void OnDestroy()
        {
            if (_cancelAction != null) _cancelAction.performed -= OnCancelPerformed;
            if (_confirmAction != null) _confirmAction.performed -= OnConfirmPerformed;
        }

        public void HandleCellClicked(Cell cell)
        {
            _command.Interact(cell);
        }

        private void OnCancelPerformed(InputAction.CallbackContext context) => _checkersCommand.Cancel();
        private void OnConfirmPerformed(InputAction.CallbackContext context) => _checkersCommand.Confirm();
    }
}
