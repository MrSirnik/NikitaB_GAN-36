using UnityEngine;

namespace FPS
{
    [RequireComponent(typeof(WeaponController))]
    public class AutoTurret : MonoBehaviour
    {
        [SerializeField] private float _viewRadius = 20f;
        [SerializeField] private float _rotationSpeed = 90f;
        [SerializeField] private float _fireAngleThreshold = 5f;

        private WeaponController _weapon;

        private void Awake()
        {
            _weapon = GetComponent<WeaponController>();
        }

        private void Update()
        {
            EnemyBase target = FindNearestEnemy();
            if (target == null) return;

            Vector3 direction = target.transform.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f) return;

            Quaternion desired = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desired, _rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, desired) < _fireAngleThreshold)
            {
                _weapon.AimAt(target.transform.position + Vector3.up);
                _weapon.TryFire();
            }
        }

        private EnemyBase FindNearestEnemy()
        {
            EnemyBase nearest = null;
            float nearestDistance = _viewRadius;

            foreach (EnemyBase enemy in EnemyRegistry.All)
            {
                if (enemy == null) continue;

                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = enemy;
                }
            }

            return nearest;
        }
    }
}
