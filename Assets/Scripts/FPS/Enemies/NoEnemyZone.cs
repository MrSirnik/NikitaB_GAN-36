using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class NoEnemyZone : MonoBehaviour
    {
        private static readonly List<NoEnemyZone> _zones = new();

        [SerializeField] private Vector3 _size = new(6f, 4f, 6f);

        private Bounds WorldBounds => new(transform.position, _size);

        private void OnEnable() => _zones.Add(this);
        private void OnDisable() => _zones.Remove(this);

        public static Vector3 AdjustDestination(Vector3 from, Vector3 desired)
        {
            foreach (NoEnemyZone zone in _zones)
            {
                Bounds bounds = zone.WorldBounds;
                if (bounds.Contains(desired))
                {
                    desired = bounds.ClosestPoint(from);
                }
            }

            return desired;
        }

        public static Vector3 PushOutside(Vector3 point)
        {
            foreach (NoEnemyZone zone in _zones)
            {
                Bounds bounds = zone.WorldBounds;
                if (!bounds.Contains(point)) continue;

                float distMinX = point.x - bounds.min.x;
                float distMaxX = bounds.max.x - point.x;
                float distMinZ = point.z - bounds.min.z;
                float distMaxZ = bounds.max.z - point.z;
                float closest = Mathf.Min(Mathf.Min(distMinX, distMaxX), Mathf.Min(distMinZ, distMaxZ));

                if (closest == distMinX) point.x = bounds.min.x - 0.2f;
                else if (closest == distMaxX) point.x = bounds.max.x + 0.2f;
                else if (closest == distMinZ) point.z = bounds.min.z - 0.2f;
                else point.z = bounds.max.z + 0.2f;
            }

            return point;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
            Gizmos.DrawCube(transform.position, _size);
        }
    }
}
