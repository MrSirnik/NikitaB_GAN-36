using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FPS
{
    public static class ControlsHud
    {
        private const string ControlsText =
            "WASD — движение\n" +
            "Shift — бег\n" +
            "Пробел — прыжок\n" +
            "Мышь — обзор\n" +
            "ЛКМ — стрельба\n" +
            "1-5 — смена оружия";

        public static void Build()
        {
            var canvasObject = new GameObject("ControlsHudCanvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            var textObject = new GameObject("ControlsText");
            textObject.transform.SetParent(canvasObject.transform, false);

            var text = textObject.AddComponent<TextMeshProUGUI>();
            text.text = ControlsText;
            text.fontSize = 22f;
            text.color = new Color(1f, 1f, 1f, 0.45f);
            text.alignment = TextAlignmentOptions.TopLeft;

            RectTransform rect = text.rectTransform;
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(20f, 0f);
            rect.sizeDelta = new Vector2(220f, -40f);
        }
    }
}
