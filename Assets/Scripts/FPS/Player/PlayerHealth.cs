using System;
using UnityEngine;

namespace FPS
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private int _maxHealth = 100;

        public event Action<int, int> HealthChanged;
        public event Action Died;

        public int MaxHealth => _maxHealth;
        public int CurrentHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0;

        private void Awake()
        {
            CurrentHealth = _maxHealth;
        }

        public void TakeDamage(int amount)
        {
            if (IsDead || amount <= 0) return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            HealthChanged?.Invoke(CurrentHealth, _maxHealth);

            if (IsDead)
            {
                Died?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            if (IsDead || amount <= 0) return;

            CurrentHealth = Mathf.Min(_maxHealth, CurrentHealth + amount);
            HealthChanged?.Invoke(CurrentHealth, _maxHealth);
        }
    }
}
