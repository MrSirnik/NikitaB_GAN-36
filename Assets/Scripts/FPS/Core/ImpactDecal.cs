using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    // Плоский след от попадания вместо шарика частиц. На статичных поверхностях
    // след остаётся (буквально "оставляет след" из ТЗ), но их количество ограничено -
    // старые следы удаляются, когда новых становится слишком много.
    // На живых целях след короткий: цель двигается, и висящий в воздухе след выглядит плохо.
    public static class ImpactDecal
    {
        private const int MaxPersistentDecals = 40;
        private const float FleshLifetime = 0.3f;
        private const float DecalSize = 0.12f;

        private static readonly Queue<GameObject> _persistentDecals = new();

        public static void Spawn(SurfaceType surface, Vector3 point, Vector3 normal)
        {
            GameObject decal = CreateQuad(point, normal, ColorFor(surface));

            if (surface == SurfaceType.Flesh)
            {
                Object.Destroy(decal, FleshLifetime);
                return;
            }

            _persistentDecals.Enqueue(decal);
            if (_persistentDecals.Count > MaxPersistentDecals)
            {
                GameObject oldest = _persistentDecals.Dequeue();
                if (oldest != null) Object.Destroy(oldest);
            }
        }

        private static GameObject CreateQuad(Vector3 point, Vector3 normal, Color color)
        {
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "ImpactDecal";
            quad.transform.position = point + normal * 0.01f;
            quad.transform.rotation = Quaternion.LookRotation(-normal);
            quad.transform.localScale = Vector3.one * DecalSize;
            quad.GetComponent<Renderer>().material.color = color;
            Object.Destroy(quad.GetComponent<Collider>());
            return quad;
        }

        private static Color ColorFor(SurfaceType surface) => surface switch
        {
            SurfaceType.Metal => new Color(0.05f, 0.05f, 0.05f),
            SurfaceType.Wood => new Color(0.15f, 0.08f, 0.03f),
            SurfaceType.Sand => new Color(0.35f, 0.28f, 0.15f),
            SurfaceType.Flesh => new Color(0.4f, 0.02f, 0.02f),
            _ => new Color(0.08f, 0.08f, 0.08f),
        };
    }
}
