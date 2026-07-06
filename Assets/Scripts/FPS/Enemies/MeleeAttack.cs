using UnityEngine;

namespace FPS
{
    public class MeleeAttack : MonoBehaviour, IEnemyAttack
    {
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private int _damage = 20;
        [SerializeField] private float _attackCooldown = 1.2f;
        [SerializeField] private string _attackStateName = "NormalAttack01_SwordShield";
        [SerializeField] private Animator _animator;

        private float _cooldownTimer;

        public float AttackRange => _attackRange;

        private void Awake()
        {
            if (_animator == null) _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (_cooldownTimer > 0f) _cooldownTimer -= Time.deltaTime;
        }

        public bool TryAttack(Transform target)
        {
            if (target == null || _cooldownTimer > 0f) return false;

            _cooldownTimer = _attackCooldown;
            _animator?.Play(_attackStateName, 0, 0f);

            var damageable = target.GetComponentInParent<IDamageable>();
            damageable?.TakeDamage(_damage);
            return true;
        }
    }
}
