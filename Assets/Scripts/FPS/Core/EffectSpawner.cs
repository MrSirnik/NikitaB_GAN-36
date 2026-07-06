using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public static class EffectSpawner
    {
        private const float DefaultLifetime = 4f;
        private static readonly Dictionary<string, GameObject> _cache = new();

        public static void Spawn(string assetPath, Vector3 position, Quaternion rotation, float lifetime = DefaultLifetime)
        {
            GameObject prefab = GetPrefab(assetPath);
            if (prefab == null) return;

            GameObject instance = Object.Instantiate(prefab, position, rotation);
            MaterialCompat.FixForBuiltinPipeline(instance);
            Object.Destroy(instance, lifetime);
        }

        public static GameObject GetPrefab(string assetPath)
        {
            if (_cache.TryGetValue(assetPath, out GameObject cached) && cached != null)
            {
                return cached;
            }

            GameObject prefab = ProjectAsset.Load<GameObject>(assetPath);
            if (prefab != null)
            {
                _cache[assetPath] = prefab;
            }

            return prefab;
        }
    }
}
