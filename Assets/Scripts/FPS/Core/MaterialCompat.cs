using UnityEngine;

namespace FPS
{
    public static class MaterialCompat
    {
        public static void FixForBuiltinPipeline(GameObject root)
        {
            foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                Material[] materials = renderer.sharedMaterials;
                bool changed = false;

                for (int i = 0; i < materials.Length; i++)
                {
                    Material source = materials[i];
                    if (source == null || source.shader == null) continue;
                    if (!source.shader.name.StartsWith("Universal Render Pipeline")) continue;

                    materials[i] = ConvertToBuiltin(source, renderer is ParticleSystemRenderer);
                    changed = true;
                }

                if (changed) renderer.sharedMaterials = materials;
            }
        }

        private static Material ConvertToBuiltin(Material source, bool isParticle)
        {
            Shader shader = isParticle ? Shader.Find("Particles/Standard Unlit") : Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Sprites/Default");

            var converted = new Material(shader) { name = source.name + "_Builtin" };

            Color color = Color.white;
            if (source.HasProperty("_BaseColor")) color = source.GetColor("_BaseColor");
            else if (source.HasProperty("_Color")) color = source.GetColor("_Color");

            Texture mainTexture = null;
            if (source.HasProperty("_BaseMap")) mainTexture = source.GetTexture("_BaseMap");
            else if (source.HasProperty("_MainTex")) mainTexture = source.GetTexture("_MainTex");

            if (converted.HasProperty("_Color")) converted.SetColor("_Color", color);
            if (converted.HasProperty("_TintColor")) converted.SetColor("_TintColor", color);
            if (mainTexture != null && converted.HasProperty("_MainTex")) converted.SetTexture("_MainTex", mainTexture);

            return converted;
        }
    }
}
