using UnityEngine;
using UnityEngine.UI;

namespace FPS
{
    public class MinimapSetup : MonoBehaviour
    {
        [SerializeField] private Transform _followTarget;
        [SerializeField] private float _height = 30f;
        [SerializeField] private float _orthoSize = 20f;

        private Camera _minimapCamera;

        public void SetFollowTarget(Transform target) => _followTarget = target;

        private void Awake()
        {
            var cameraObject = new GameObject("MinimapCamera");
            _minimapCamera = cameraObject.AddComponent<Camera>();
            _minimapCamera.orthographic = true;
            _minimapCamera.orthographicSize = _orthoSize;
            _minimapCamera.nearClipPlane = 1f;
            _minimapCamera.farClipPlane = _height + 10f;
            _minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            var renderTexture = new RenderTexture(256, 256, 16);
            _minimapCamera.targetTexture = renderTexture;

            BuildUi(renderTexture);
        }

        private void BuildUi(RenderTexture texture)
        {
            var canvasObject = new GameObject("MinimapCanvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            var imageObject = new GameObject("MinimapImage");
            imageObject.transform.SetParent(canvasObject.transform, false);
            var rawImage = imageObject.AddComponent<RawImage>();
            rawImage.texture = texture;

            RectTransform rect = rawImage.rectTransform;
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-20f, -20f);
            rect.sizeDelta = new Vector2(200f, 200f);
        }

        private void LateUpdate()
        {
            if (_followTarget == null) return;

            Vector3 pos = _followTarget.position;
            _minimapCamera.transform.position = new Vector3(pos.x, pos.y + _height, pos.z);
        }
    }
}
