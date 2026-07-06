using UnityEngine;

namespace FPS
{
    public enum WeaponFireMode
    {
        Hitscan,
        Projectile
    }

    [CreateAssetMenu(menuName = "FPS/Weapon Data", fileName = "NewWeapon")]
    public class WeaponData : ScriptableObject
    {
        [Header("Основное")]
        public string weaponName = "Weapon";
        public WeaponFireMode fireMode = WeaponFireMode.Hitscan;
        public int damage = 10;
        public float fireCooldown = 0.15f;
        public float range = 100f;

        [Header("Дробовик (несколько пуль за выстрел)")]
        [Min(1)] public int pelletsPerShot = 1;
        [Range(0f, 10f)] public float spreadDegrees = 0f;

        [Header("Снаряд (для fireMode = Projectile)")]
        public float projectileSpeed = 25f;

        [Header("Отдача (визуальный punch камеры/оружия)")]
        public float recoilKick = 0.05f;
        public float recoilDuration = 0.08f;

        [Header("Звук выстрела")]
        public string fireSoundPath = FpsAssetPaths.GunshotClip;
    }
}
