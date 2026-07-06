using UnityEngine;

namespace FPS
{
    public class Destructible : MonoBehaviour, IDamageable
    {
        [SerializeField] private int _health = 30;
        [SerializeField] private string _destroyEffectPath = FpsAssetPaths.SmallExplosion;

        public Faction Faction => Faction.Neutral;

        public void TakeDamage(int amount)
        {
            if (amount <= 0) return;

            _health -= amount;
            if (_health <= 0)
            {
                EffectSpawner.Spawn(_destroyEffectPath, transform.position, Quaternion.identity);
                EffectSpawner.Spawn(FpsAssetPaths.Smoke, transform.position, Quaternion.identity);
                FpsAudio.PlayAt(FpsAssetPaths.ExplosionClip, transform.position);
                gameObject.SetActive(false);
            }
        }
    }
}
