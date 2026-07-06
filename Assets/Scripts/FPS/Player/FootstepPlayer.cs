using UnityEngine;

namespace FPS
{
    [RequireComponent(typeof(CharacterController))]
    public class FootstepPlayer : MonoBehaviour
    {
        [SerializeField] private float _stepInterval = 0.45f;

        private CharacterController _controller;
        private float _timer;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (!_controller.isGrounded || _controller.velocity.sqrMagnitude < 0.1f)
            {
                _timer = 0f;
                return;
            }

            _timer += Time.deltaTime;
            if (_timer < _stepInterval) return;

            _timer = 0f;
            PlayRandomFootstep();
        }

        private void PlayRandomFootstep()
        {
            string[] clips = FpsAssetPaths.FootstepClips;
            if (clips.Length == 0) return;

            string path = clips[Random.Range(0, clips.Length)];
            FpsAudio.PlayAt(path, transform.position, 0.6f);
        }
    }
}
