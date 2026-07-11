using UnityEngine;

namespace Tactics
{
    // Все объекты сцены (доска, фишки, камера, UI, консоль) размещаются заранее
    // в редакторе через Tactics/Setup Scene - здесь только дёшево связываем то,
    // что физически невозможно запечь в сцену: CheckersRules и CheckersMoveCommand,
    // это простые C#-объекты, а не Unity Object.
    public static class TacticsBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Run()
        {
            if (Object.FindFirstObjectByType<BakedTacticsMarker>() == null)
            {
                Debug.LogError("Tactics: сцена не запечена. Запустите Tactics/Setup Scene в редакторе.");
                return;
            }

            var battlefield = Object.FindFirstObjectByType<Battlefield>();
            var battleController = Object.FindFirstObjectByType<BattleController>();
            var playerController = Object.FindFirstObjectByType<PlayerController>();
            var turnPanel = Object.FindFirstObjectByType<TurnPanelView>();

            if (battlefield == null || battleController == null || playerController == null)
            {
                Debug.LogError("Tactics: не найдены базовые объекты сцены. Запустите Tactics/Setup Scene в редакторе.");
                return;
            }

            var rules = new CheckersRules(battlefield);
            playerController.WireRules(rules);

            var command = new CheckersMoveCommand(battlefield, rules, playerController);
            battleController.WireCommand(command);

            if (turnPanel != null)
            {
                command.OnTurnChanged += turnPanel.SetTurn;
            }

            command.Initialize();
        }
    }
}
