using UnityEngine;

namespace FPS
{
    public static class WeaponVisual
    {
        public static void Build(Transform weapon, WeaponData data)
        {
            switch (data != null ? data.weaponName : null)
            {
                case "Pistol": BuildPistol(weapon); break;
                case "SMG": BuildSmg(weapon); break;
                case "Shotgun": BuildShotgun(weapon); break;
                case "Sniper": BuildSniper(weapon); break;
                case "RocketLauncher": BuildRocketLauncher(weapon); break;
                default: BuildPistol(weapon); break;
            }
        }

        private static void AddPart(Transform parent, PrimitiveType type, Vector3 localPos, Vector3 localScale, Color color, Quaternion? localRot = null)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            part.name = "Visual";
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPos;
            part.transform.localRotation = localRot ?? Quaternion.identity;
            part.transform.localScale = localScale;
            part.GetComponent<Renderer>().material.color = color;
            Object.Destroy(part.GetComponent<Collider>());
        }

        private static void BuildPistol(Transform weapon)
        {
            Color slide = new Color(0.15f, 0.15f, 0.16f);
            Color metal = new Color(0.08f, 0.08f, 0.09f);

            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, 0.02f, 0.2f), new Vector3(0.07f, 0.09f, 0.32f), slide);
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, -0.14f, 0.06f), new Vector3(0.06f, 0.16f, 0.07f), slide,
                Quaternion.Euler(15f, 0f, 0f));
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, -0.03f, 0.14f), new Vector3(0.015f, 0.06f, 0.02f), metal);
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, -0.06f, 0.14f), new Vector3(0.04f, 0.015f, 0.05f), metal);
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, 0.075f, 0.34f), new Vector3(0.015f, 0.02f, 0.02f), Color.black);
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, 0.075f, 0.08f), new Vector3(0.02f, 0.02f, 0.03f), Color.black);
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, -0.22f, 0.02f), new Vector3(0.05f, 0.06f, 0.04f), metal);
        }

        private static void BuildSmg(Transform weapon)
        {
            Color body = new Color(0.2f, 0.2f, 0.22f);
            Color metal = new Color(0.1f, 0.1f, 0.1f);

            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, 0f, 0.28f), new Vector3(0.08f, 0.1f, 0.5f), body);
            AddPart(weapon, PrimitiveType.Cylinder, new Vector3(0f, 0.01f, 0.55f), new Vector3(0.025f, 0.1f, 0.025f), metal,
                Quaternion.Euler(90f, 0f, 0f));
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, -0.16f, 0.16f), new Vector3(0.04f, 0.16f, 0.08f), metal,
                Quaternion.Euler(-8f, 0f, 0f));
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, -0.15f, 0.05f), new Vector3(0.05f, 0.14f, 0.09f), new Color(0.1f, 0.1f, 0.1f));
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, 0.02f, -0.13f), new Vector3(0.05f, 0.06f, 0.24f), body);
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, 0.07f, 0.15f), new Vector3(0.03f, 0.02f, 0.3f), metal);
        }

        private static void BuildShotgun(Transform weapon)
        {
            Color wood = new Color(0.35f, 0.22f, 0.1f);
            Color metal = new Color(0.15f, 0.15f, 0.15f);

            AddPart(weapon, PrimitiveType.Cylinder, new Vector3(0f, 0.03f, 0.35f), new Vector3(0.04f, 0.35f, 0.04f), metal,
                Quaternion.Euler(90f, 0f, 0f));
            AddPart(weapon, PrimitiveType.Sphere, new Vector3(0f, 0.03f, 0.7f), new Vector3(0.045f, 0.045f, 0.045f), metal);
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, 0.03f, 0.05f), new Vector3(0.09f, 0.11f, 0.2f), metal);
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, -0.06f, 0.22f), new Vector3(0.06f, 0.06f, 0.2f), wood);
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, -0.03f, -0.1f), new Vector3(0.06f, 0.08f, 0.2f), wood);
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, 0f, -0.22f), new Vector3(0.07f, 0.14f, 0.05f), wood);
        }

        private static void BuildSniper(Transform weapon)
        {
            Color body = new Color(0.08f, 0.08f, 0.1f);
            Color wood = new Color(0.25f, 0.18f, 0.1f);

            AddPart(weapon, PrimitiveType.Cylinder, new Vector3(0f, 0f, 0.4f), new Vector3(0.025f, 0.45f, 0.025f), body,
                Quaternion.Euler(90f, 0f, 0f));
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, -0.03f, -0.05f), new Vector3(0.06f, 0.08f, 0.35f), wood);
            AddPart(weapon, PrimitiveType.Cylinder, new Vector3(0f, 0.09f, 0.15f), new Vector3(0.025f, 0.12f, 0.025f), Color.black,
                Quaternion.Euler(90f, 0f, 0f));
            AddPart(weapon, PrimitiveType.Cylinder, new Vector3(0f, 0.09f, 0.03f), new Vector3(0.028f, 0.01f, 0.028f), new Color(0.3f, 0.5f, 0.55f),
                Quaternion.Euler(90f, 0f, 0f));
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0.03f, -0.06f, 0.18f), new Vector3(0.02f, 0.02f, 0.05f), body,
                Quaternion.Euler(0f, 0f, 20f));
            AddPart(weapon, PrimitiveType.Cube, new Vector3(-0.05f, -0.14f, 0.55f), new Vector3(0.012f, 0.16f, 0.012f), body,
                Quaternion.Euler(30f, 0f, 0f));
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0.05f, -0.14f, 0.55f), new Vector3(0.012f, 0.16f, 0.012f), body,
                Quaternion.Euler(30f, 0f, 0f));
        }

        private static void BuildRocketLauncher(Transform weapon)
        {
            Color body = new Color(0.25f, 0.3f, 0.18f);
            Color metal = new Color(0.1f, 0.1f, 0.1f);

            AddPart(weapon, PrimitiveType.Cylinder, new Vector3(0f, 0.02f, 0.3f), new Vector3(0.13f, 0.4f, 0.13f), body,
                Quaternion.Euler(90f, 0f, 0f));
            AddPart(weapon, PrimitiveType.Cylinder, new Vector3(0f, 0.02f, -0.14f), new Vector3(0.06f, 0.08f, 0.06f), new Color(0.35f, 0.32f, 0.2f),
                Quaternion.Euler(90f, 0f, 0f));
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, -0.2f, 0.05f), new Vector3(0.05f, 0.16f, 0.05f), metal);
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, 0.16f, 0.15f), new Vector3(0.02f, 0.03f, 0.03f), Color.black);
            AddPart(weapon, PrimitiveType.Cube, new Vector3(0f, 0.16f, 0.66f), new Vector3(0.03f, 0.03f, 0.02f), Color.black);
        }
    }
}
