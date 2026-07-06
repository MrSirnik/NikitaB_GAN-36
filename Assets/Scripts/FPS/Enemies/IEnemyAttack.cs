using UnityEngine;

namespace FPS
{
    public interface IEnemyAttack
    {
        float AttackRange { get; }
        bool TryAttack(Transform target);
    }
}
