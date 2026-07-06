using UnityEngine;

namespace FPS
{
    public enum LightingMode { Day, Night, Dynamic }

    [RequireComponent(typeof(Light))]
    public class DayNightCycle : MonoBehaviour
    {
        [SerializeField] private LightingMode _mode = LightingMode.Dynamic;
        [SerializeField] private float _cycleSpeed = 2f;
        [SerializeField] private Color _dayColor = new(1f, 0.95f, 0.84f);
        [SerializeField] private Color _nightColor = new(0.25f, 0.3f, 0.5f);
        [SerializeField] private float _dayIntensity = 1.2f;
        [SerializeField] private float _nightIntensity = 0.15f;

        private Light _light;
        private float _t;

        public void SetMode(LightingMode mode)
        {
            _mode = mode;
            if (_light != null) ApplyStatic();
        }

        private void Awake()
        {
            _light = GetComponent<Light>();
            ApplyStatic();
        }

        private void Update()
        {
            if (_mode != LightingMode.Dynamic) return;

            _t += Time.deltaTime * _cycleSpeed;
            transform.localRotation = Quaternion.Euler(_t % 360f - 90f, -30f, 0f);

            float dayFactor = Mathf.Clamp01(Vector3.Dot(-transform.forward, Vector3.up) + 0.3f);
            _light.color = Color.Lerp(_nightColor, _dayColor, dayFactor);
            _light.intensity = Mathf.Lerp(_nightIntensity, _dayIntensity, dayFactor);
        }

        private void ApplyStatic()
        {
            switch (_mode)
            {
                case LightingMode.Day:
                    _light.color = _dayColor;
                    _light.intensity = _dayIntensity;
                    break;
                case LightingMode.Night:
                    _light.color = _nightColor;
                    _light.intensity = _nightIntensity;
                    break;
            }
        }
    }
}
