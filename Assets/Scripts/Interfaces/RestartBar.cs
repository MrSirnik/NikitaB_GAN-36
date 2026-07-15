using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tactics
{
    // Удержание TAB заполняет шкалу, при полном заполнении перезапускает сцену.
    public class RestartBar : MonoBehaviour
    {
        private const float HoldDuration = 1.5f;

        [SerializeField] private Image _fillImage;
        private float _timer;

        public void Awake()
        {
            if (_fillImage == null) _fillImage = transform.Find("RestartBarBackground/RestartBarFill")?.GetComponent<Image>();
        }

        public static RestartBar Build()
        {
            var canvasObject = new GameObject("RestartBarCanvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            var view = canvasObject.AddComponent<RestartBar>();

            var backgroundObject = new GameObject("RestartBarBackground");
            backgroundObject.transform.SetParent(canvasObject.transform, false);
            var background = backgroundObject.AddComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.4f);

            RectTransform backgroundRect = background.rectTransform;
            backgroundRect.anchorMin = new Vector2(0.5f, 0f);
            backgroundRect.anchorMax = new Vector2(0.5f, 0f);
            backgroundRect.pivot = new Vector2(0.5f, 0f);
            backgroundRect.anchoredPosition = new Vector2(0f, 20f);
            backgroundRect.sizeDelta = new Vector2(300f, 20f);

            var fillObject = new GameObject("RestartBarFill");
            fillObject.transform.SetParent(backgroundObject.transform, false);
            var fill = fillObject.AddComponent<Image>();
            fill.color = new Color(0.9f, 0.3f, 0.2f, 0.9f);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = 0f;

            RectTransform fillRect = fill.rectTransform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            view._fillImage = fill;
            return view;
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            bool held = keyboard != null && keyboard.tabKey.isPressed;

            if (held)
            {
                _timer += Time.deltaTime;
                if (_timer >= HoldDuration)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    return;
                }
            }
            else
            {
                _timer = 0f;
            }

            _fillImage.fillAmount = _timer / HoldDuration;
        }
    }
}
