using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tactics
{
    // Панель хода: показывает, чья сейчас очередь ходить.
    public class TurnPanelView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void Awake()
        {
            if (_text == null) _text = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        public static TurnPanelView Build()
        {
            var canvasObject = new GameObject("TurnPanelCanvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            var view = canvasObject.AddComponent<TurnPanelView>();

            var textObject = new GameObject("TurnText");
            textObject.transform.SetParent(canvasObject.transform, false);
            var text = textObject.AddComponent<TextMeshProUGUI>();
            text.fontSize = 36f;
            text.alignment = TextAlignmentOptions.Top;
            text.color = Color.white;

            RectTransform rect = text.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -20f);
            rect.sizeDelta = new Vector2(400f, 60f);

            view._text = text;
            return view;
        }

        public void SetTurn(Team team)
        {
            _text.text = team == Team.Player1 ? "Ход: Player1" : "Ход: Player2";
        }
    }
}
