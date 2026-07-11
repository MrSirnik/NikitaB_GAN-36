using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tactics
{
    // Полупрозрачная подсказка по управлению в углу экрана.
    public static class ControlsHud
    {
        private const string ControlsText =
            "ЛКМ — выбрать клетку/фишку\n" +
            "Space — подтвердить ход\n" +
            "ESC — отменить выбор\n" +
            "Удержание TAB — рестарт";

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
            text.fontSize = 20f;
            text.color = new Color(1f, 1f, 1f, 0.45f);
            text.alignment = TextAlignmentOptions.BottomLeft;

            RectTransform rect = text.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.pivot = Vector2.zero;
            rect.anchoredPosition = new Vector2(20f, 20f);
            rect.sizeDelta = new Vector2(280f, 100f);
        }
    }
}
