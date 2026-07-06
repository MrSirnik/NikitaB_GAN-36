using UnityEngine;

namespace FPS
{
    public class Trap : MonoBehaviour
    {
        [SerializeField] private int _damagePerTick = 10;
        [SerializeField] private float _tickInterval = 1f;

        private float _timer;

        private void OnTriggerStay(Collider other)
        {
            var damageable = other.GetComponentInParent<IDamageable>();
            if (damageable == null || damageable.Faction != Faction.Player) return;

            _timer += Time.deltaTime;
            if (_timer < _tickInterval) return;

            _timer = 0f;
            damageable.TakeDamage(_damagePerTick);
        }
    }
}
