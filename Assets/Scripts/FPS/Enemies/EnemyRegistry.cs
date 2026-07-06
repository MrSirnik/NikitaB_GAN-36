using System.Collections.Generic;

namespace FPS
{
    public static class EnemyRegistry
    {
        private static readonly List<EnemyBase> _enemies = new();

        public static IReadOnlyList<EnemyBase> All => _enemies;

        public static void Register(EnemyBase enemy)
        {
            if (!_enemies.Contains(enemy))
            {
                _enemies.Add(enemy);
            }
        }

        public static void Unregister(EnemyBase enemy)
        {
            _enemies.Remove(enemy);
        }
    }
}
