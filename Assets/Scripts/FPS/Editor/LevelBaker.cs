using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FPS.EditorTools
{
    // Инструмент редактора: один раз строит уровень тем же кодом, что и рантайм-бутстрап,
    // и сохраняет результат прямо в файл сцены. После этого при запуске Play сцена
    // уже готова и не собирается заново с нуля (см. BakedLevelMarker в GameBootstrap.Run()).
    public static class LevelBaker
    {
        private static readonly string[] ScenePaths =
        {
            "Assets/Scenes/FPS_Urban.unity",
            "Assets/Scenes/FPS_Forest.unity",
            "Assets/Scenes/FPS_Dungeon.unity",
        };

        [MenuItem("FPS/Bake All Levels")]
        public static void BakeAll()
        {
            foreach (string path in ScenePaths)
            {
                BakeScene(path);
            }

            Debug.Log("FPS: все уровни собраны и сохранены в сценах.");
        }

        private static void BakeScene(string scenePath)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            if (Object.FindFirstObjectByType<BakedLevelMarker>() != null)
            {
                Debug.Log($"FPS: {scenePath} уже собрана, пропускаю.");
                return;
            }

            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            GameBootstrap.BakeLevelInEditor(sceneName);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }
}
