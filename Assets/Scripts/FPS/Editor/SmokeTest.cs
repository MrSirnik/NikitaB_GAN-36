using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace FPS.EditorTools
{
    [InitializeOnLoad]
    public static class SmokeTest
    {
        private const string StateKey = "FPS_SmokeTest_SceneIndex";
        private const string RunningKey = "FPS_SmokeTest_Running";
        private const string LogPathKey = "FPS_SmokeTest_LogPath";

        private static readonly string[] Scenes =
        {
            "Assets/Scenes/FPS_Urban.unity",
            "Assets/Scenes/FPS_Forest.unity",
            "Assets/Scenes/FPS_Dungeon.unity",
        };

        private static int _frameCount;

        static SmokeTest()
        {
            if (SessionState.GetBool(RunningKey, false))
            {
                Application.logMessageReceived += OnLog;
                EditorApplication.update += Tick;
            }
        }

        public static void RunAll()
        {
            string logPath = Path.Combine(Path.GetTempPath(), "fps_smoke_test.txt");
            File.WriteAllText(logPath, "");

            SessionState.SetString(LogPathKey, logPath);
            SessionState.SetInt(StateKey, 0);
            SessionState.SetBool(RunningKey, true);

            Application.logMessageReceived += OnLog;
            EditorApplication.update += Tick;

            OpenSceneAndPlay(0);
        }

        private static void OpenSceneAndPlay(int index)
        {
            AppendLog($"=== SCENE {Scenes[index]} ===");
            EditorSceneManager.OpenScene(Scenes[index]);
            _frameCount = 0;
            EditorApplication.isPlaying = true;
        }

        private static void Tick()
        {
            if (!EditorApplication.isPlaying) return;

            _frameCount++;
            if (_frameCount < 90) return;

            int index = SessionState.GetInt(StateKey, 0);
            EditorApplication.isPlaying = false;

            int next = index + 1;
            if (next >= Scenes.Length)
            {
                EditorApplication.delayCall += Finish;
            }
            else
            {
                SessionState.SetInt(StateKey, next);
                EditorApplication.delayCall += () => OpenSceneAndPlay(next);
            }
        }

        private static void AppendLog(string line)
        {
            string path = SessionState.GetString(LogPathKey, Path.Combine(Path.GetTempPath(), "fps_smoke_test.txt"));
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
