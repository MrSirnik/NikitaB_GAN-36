using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Tactics.EditorTools
{
    [InitializeOnLoad]
    public static class TacticsSmokeTest
    {
        private const string ScenePath = "Assets/Scenes/Tactics.unity";
        private const string RunningKey = "Tactics_SmokeTest_Running";
        private const string LogPathKey = "Tactics_SmokeTest_LogPath";

        static TacticsSmokeTest()
        {
            if (SessionState.GetBool(RunningKey, false))
            {
                Application.logMessageReceived += OnLog;
                EditorApplication.update += Tick;
            }
        }

        [MenuItem("Tactics/Run Smoke Test")]
        public static void Run()
        {
            string logPath = Path.Combine(Path.GetTempPath(), "tactics_smoke_test.txt");
            File.WriteAllText(logPath, "");

            SessionState.SetString(LogPathKey, logPath);
            SessionState.SetBool(RunningKey, true);

            Application.logMessageReceived += OnLog;
            EditorApplication.update += Tick;

            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            AppendLog("=== START ===");
            EditorApplication.isPlaying = true;
        }

        private static int _frameCount;

        private static void Tick()
        {
            if (!EditorApplication.isPlaying) return;

            _frameCount++;
            if (_frameCount < 120) return;

            EditorApplication.isPlaying = false;
            EditorApplication.delayCall += Finish;
        }

        private static void AppendLog(string line)
        {
            string path = SessionState.GetString(LogPathKey, Path.Combine(Path.GetTempPath(), "tactics_smoke_test.txt"));
            File.AppendAllText(path, line + "\n");
        }

        private static void OnLog(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
            AppendLog($"[{type}] {condition}\n{stackTrace}\n");
        }

        private static void Finish()
        {
            EditorApplication.update -= Tick;
            Application.logMessageReceived -= OnLog;
            SessionState.SetBool(RunningKey, false);
            AppendLog("=== DONE ===");
            EditorApplication.Exit(0);
        }
    }
}
