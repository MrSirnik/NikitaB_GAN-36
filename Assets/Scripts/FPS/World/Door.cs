using DG.Tweening;
using UnityEngine;

namespace FPS
{
    [RequireComponent(typeof(Collider))]
    public class Door : MonoBehaviour
    {
        [SerializeField] private Vector3 _openOffset = new(0f, 3f, 0f);
        [SerializeField] private float _duration = 0.6f;

        private Vector3 _closedPosition;
        private int _occupants;

        private void Awake()
        {
            _closedPosition = transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            _occupants++;
            transform.DOKill();
            transform.DOMove(_closedPosition + _openOffset, _duration);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            _occupants = Mathf.Max(0, _occupants - 1);
            if (_occupants == 0)
            {
                transform.DOKill();
                transform.DOMove(_closedPosition, _duration);
            }
        }
    }
}
