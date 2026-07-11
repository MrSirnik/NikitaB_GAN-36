using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace Tactics.EditorTools
{
    // Готовит проект и один раз собирает игровую сцену: доска, фишки, камера, EventSystem,
    // UI и консоль отладки размещаются как настоящие объекты сцены (не создаются кодом в рантайме).
    // Повторный запуск на уже собранной сцене ничего не пересоздаёт (см. BakedTacticsMarker).
    public static class TacticsSceneBaker
    {
        private const string ResourcesFolder = "Assets/Resources";
        private const string BoardSettingsPath = ResourcesFolder + "/BoardSettings.asset";
        private const string CheckersSettingsPath = ResourcesFolder + "/CheckersSettings.asset";
        private const string InputActionsPath = "Assets/GameControls.inputactions";
        private const string ScenePath = "Assets/Scenes/Tactics.unity";
        private const string DebugConsolePrefabPath = "Assets/Plugins/IngameDebugConsole/IngameDebugConsole.prefab";

        private const string MaterialsFolder = "Assets/Materials/Tactics";
        private const string LightMaterialPath = MaterialsFolder + "/CellLight.mat";
        private const string DarkMaterialPath = MaterialsFolder + "/CellDark.mat";
        private const string Player1MaterialPath = MaterialsFolder + "/Player1.mat";
        private const string Player2MaterialPath = MaterialsFolder + "/Player2.mat";

        [MenuItem("Tactics/Setup Scene")]
        public static void SetupScene()
        {
            BoardSettings boardSettings = EnsureSettingsAssets();

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null)
            {
                EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
                if (Object.FindFirstObjectByType<BakedTacticsMarker>() != null)
                {
                    Debug.Log("Tactics: сцена уже собрана, пропускаю.");
                    return;
                }
            }

            // Старой сцены либо нет, либо она из прошлой (нерабочей) архитектуры без бейка -
            // начинаем с чистого листа, чтобы не тащить за собой битые ссылки на удалённые скрипты.
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var checkersSettings = AssetDatabase.LoadAssetAtPath<CheckersSettings>(CheckersSettingsPath);
            var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);

            EnsureMaterials(boardSettings, checkersSettings,
                out Material lightMaterial, out Material darkMaterial, out Material player1Material, out Material player2Material);

            SetupEventSystem();
            BuildCamera(boardSettings);

            var battlefieldObject = new GameObject("Battlefield");
            Battlefield battlefield = battlefieldObject.AddComponent<Battlefield>();

            var battleControllerObject = new GameObject("BattleController");
            BattleController battleController = battleControllerObject.AddComponent<BattleController>();
            battleController.Bake(inputActions);

            battlefield.Bake(boardSettings, checkersSettings, battleController, lightMaterial, darkMaterial, player1Material, player2Material);

            var playerControllerObject = new GameObject("PlayerController");
            PlayerController playerController = playerControllerObject.AddComponent<PlayerController>();
            playerController.Bake(checkersSettings);

            TurnPanelView.Build();
            RestartBar.Build();
            ControlsHud.Build();

            EnsureDebugConsole();

            new GameObject("BakedTacticsMarker").AddComponent<BakedTacticsMarker>();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);

            SetupBuildSettings();

            Debug.Log("Tactics: сцена собрана и сохранена в " + ScenePath);
        }

        private static BoardSettings EnsureSettingsAssets()
        {
            if (!AssetDatabase.IsValidFolder(ResourcesFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            var board = AssetDatabase.LoadAssetAtPath<BoardSettings>(BoardSettingsPath);
            if (board == null)
            {
                board = ScriptableObject.CreateInstance<BoardSettings>();
                AssetDatabase.CreateAsset(board, BoardSettingsPath);
            }

            if (AssetDatabase.LoadAssetAtPath<CheckersSettings>(CheckersSettingsPath) == null)
            {
                var checkers = ScriptableObject.CreateInstance<CheckersSettings>();
                AssetDatabase.CreateAsset(checkers, CheckersSettingsPath);
            }

            AssetDatabase.SaveAssets();
            return board;
        }

        private static void EnsureMaterials(BoardSettings boardSettings, CheckersSettings checkersSettings,
            out Material lightMaterial, out Material darkMaterial, out Material player1Material, out Material player2Material)
        {
            if (!AssetDatabase.IsValidFolder(MaterialsFolder))
            {
                AssetDatabase.CreateFolder("Assets/Materials", "Tactics");
            }

            lightMaterial = CreateOrLoadMaterial(LightMaterialPath, boardSettings.lightCellColor);
            darkMaterial = CreateOrLoadMaterial(DarkMaterialPath, boardSettings.darkCellColor);
            player1Material = CreateOrLoadMaterial(Player1MaterialPath, checkersSettings.player1Color);
            player2Material = CreateOrLoadMaterial(Player2MaterialPath, checkersSettings.player2Color);

            AssetDatabase.SaveAssets();
        }

        private static Material CreateOrLoadMaterial(string path, Color color)
        {
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            var material = new Material(Shader.Find("Standard")) { color = color };
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static void SetupEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null) return;

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private static void BuildCamera(BoardSettings settings)
        {
            if (Object.FindFirstObjectByType<Camera>() != null) return;

            var cameraObject = new GameObject("MainCamera");
            cameraObject.tag = "MainCamera";
            cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            cameraObject.AddComponent<PhysicsRaycaster>();

            float center = (settings.size - 1) * settings.cellSize * 0.5f;
            float boardSpan = settings.size * settings.cellSize;

            cameraObject.transform.position = new Vector3(center, boardSpan * 0.95f, -boardSpan * 0.65f);
            cameraObject.transform.LookAt(new Vector3(center, 0f, center));
        }

        private static void EnsureDebugConsole()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(DebugConsolePrefabPath);
            if (prefab == null)
            {
                Debug.LogWarning("Tactics: не найден префаб IngameDebugConsole по пути " + DebugConsolePrefabPath);
                return;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = prefab.name;
        }

        private static void SetupBuildSettings()
        {
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            }
        }
    }
}
