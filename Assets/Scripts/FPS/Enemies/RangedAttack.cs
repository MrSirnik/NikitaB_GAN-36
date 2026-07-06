using UnityEngine;

namespace FPS
{
    [RequireComponent(typeof(WeaponController))]
    public class RangedAttack : MonoBehaviour, IEnemyAttack
    {
        [SerializeField] private float _attackRange = 15f;

        private WeaponController _weapon;

        public float AttackRange => _attackRange;

        private void Awake()
        {
            _weapon = GetComponent<WeaponController>();
        }

        public bool TryAttack(Transform target)
        {
            if (target == null) return false;

            _weapon.AimAt(target.position + Vector3.up);
            return _weapon.TryFire();
        }
    }
}
