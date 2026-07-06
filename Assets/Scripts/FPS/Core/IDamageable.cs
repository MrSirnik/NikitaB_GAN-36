namespace FPS
{
    public interface IDamageable
    {
        Faction Faction { get; }
        void TakeDamage(int amount);
    }
}
