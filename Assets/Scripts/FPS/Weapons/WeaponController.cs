using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FPS
{
    public class WeaponController : MonoBehaviour
    {
        // Жёсткий потолок скорострельности для всех оружий, не зависит от WeaponData.fireCooldown -
        // ни одно оружие (текущее или будущее) не выстрелит чаще ~12.5 раз в секунду.
        private const float MinFireInterval = 0.08f;

        // Вспышка выстрела должна исчезать намного быстрее интервала между выстрелами,
        // иначе при длинной очереди эффекты накапливаются в кучу.
        private const float MuzzleFlashLifetime = 0.25f;
        private const float ExplosionEffectLifetime = 1.2f;
        private const float SmokeEffectLifetime = 2f;

        [SerializeField] private WeaponData _data;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private Camera _aimCamera;
        [SerializeField] private LayerMask _hitMask = ~0;
        [SerializeField] private Faction _shooterFaction = Faction.Player;

        private float _cooldownTimer;
        private Tanks.ProjectilePool _projectilePool;

        public WeaponData Data => _data;

        public void Configure(WeaponData data, Transform muzzle, Camera aimCamera, Faction shooterFaction = Faction.Player)
        {
            _data = data;
            _muzzle = muzzle;
            _aimCamera = aimCamera;
            _shooterFaction = shooterFaction;
        }

        private void Awake()
        {
            if (_muzzle == null) _muzzle = transform;
        }

        private void Update()
        {
            if (_cooldownTimer > 0f) _cooldownTimer -= Time.deltaTime;
        }

        public void AimAt(Vector3 worldPoint) => _muzzle.LookAt(worldPoint);

        public bool TryFire()
        {
            if (_data == null || _cooldownTimer > 0f) return false;

            _cooldownTimer = Mathf.Max(_data.fireCooldown, MinFireInterval);
            Fire();
            return true;
        }

        private void Fire()
        {
            EffectSpawner.Spawn(FpsAssetPaths.MuzzleFlash, _muzzle.position, _muzzle.rotation, MuzzleFlashLifetime);
            FpsAudio.PlayAt(_data.fireSoundPath, _muzzle.position);
            PlayRecoil();

            if (_data.fireMode == WeaponFireMode.Hitscan)
            {
                FireHitscan();
            }
            else
            {
                FireProjectile();
            }
        }

        private void FireHitscan()
        {
            for (int i = 0; i < _data.pelletsPerShot; i++)
            {
                Vector3 direction = GetAimDirection();
                if (Physics.Raycast(_muzzle.position, direction, out RaycastHit hit, _data.range, _hitMask, QueryTriggerInteraction.Ignore))
                {
                    ApplyHit(hit.collider, hit.point, hit.normal);
                }
            }
        }

        private void ApplyHit(Collider hitCollider, Vector3 point, Vector3 normal)
        {
            var damageable = hitCollider.GetComponentInParent<IDamageable>();
            bool hitsValidTarget = damageable != null && damageable.Faction != _shooterFaction;

            if (hitsValidTarget)
            {
                damageable.TakeDamage(_data.damage);
            }

            SurfaceType surface = hitsValidTarget ? SurfaceType.Flesh : ResolveSurface(hitCollider);
            ImpactDecal.Spawn(surface, point, normal);
        }

        private static SurfaceType ResolveSurface(Collider hitCollider)
        {
            var marker = hitCollider.GetComponentInParent<SurfaceMarker>();
            return marker != null ? marker.Surface : SurfaceType.Stone;
        }

        private Vector3 GetAimDirection()
        {
            Vector3 baseDirection = _aimCamera != null ? _aimCamera.transform.forward : _muzzle.forward;
            if (_data.spreadDegrees <= 0f) return baseDirection;

            Vector2 spread = Random.insideUnitCircle * _data.spreadDegrees;
            return Quaternion.Euler(spread.y, spread.x, 0f) * baseDirection;
        }

        private void PlayRecoil()
        {
            if (_data.recoilKick <= 0f) return;

            transform.DOKill();
            transform.DOPunchPosition(-transform.forward * _data.recoilKick, _data.recoilDuration, vibrato: 1, elasticity: 0f);
        }

        private void EnsureProjectilePool()
        {
            var poolObject = new GameObject($"{_data.weaponName}_ProjectilePool");
            poolObject.transform.SetParent(transform, false);

            var projectilePrefab = ProjectAsset.Load<GameObject>(FpsAssetPaths.ProjectilePrefab)?.GetComponent<Tanks.Projectile>();
            if (projectilePrefab == null) return;

            _projectilePool = poolObject.AddComponent<Tanks.ProjectilePool>();
            _projectilePool.CallInitialize(projectilePrefab, RegisterProjectile, 4);
        }

        private void RegisterProjectile(Tanks.Projectile projectile)
        {
            projectile.OnExplosionHandler += HandleExplosion;
        }

        private void FireProjectile()
        {
            if (_projectilePool == null) EnsureProjectilePool();
            if (_projectilePool == null) return;

            Tanks.Projectile projectile = _projectilePool.GetElement();
            projectile.transform.SetPositionAndRotation(_muzzle.position, _muzzle.rotation);
            projectile.Velocity = GetAimDirection() * _data.projectileSpeed;
        }

        private void HandleExplosion(Vector3 point, int _)
        {
            EffectSpawner.Spawn(FpsAssetPaths.SmallExplosion, point, Quaternion.identity, ExplosionEffectLifetime);
            EffectSpawner.Spawn(FpsAssetPaths.Smoke, point, Quaternion.identity, SmokeEffectLifetime);
            FpsAudio.PlayAt(FpsAssetPaths.ExplosionClip, point);

            var damagedAlready = new HashSet<IDamageable>();
            foreach (Collider hitCollider in Physics.OverlapSphere(point, 3f))
            {
                var damageable = hitCollider.GetComponentInParent<IDamageable>();
                if (damageable != null && damageable.Faction != _shooterFaction && damagedAlready.Add(damageable))
                {
                    damageable.TakeDamage(_data.damage);
                }
            }
        }
    }
}
