using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float MaxHealth = 100f;
    public float CurrentHealth;

    // Eventos
    public event Action<float> OnHealthChanged;
    public event Action OnDeath;

    void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public bool TakeDamage(float amount)
    {
        if (CurrentHealth <= 0) return false;

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        OnHealthChanged?.Invoke(CurrentHealth / MaxHealth);

        if (CurrentHealth <= 0)
        {
            Die();
            return true;
        }

        return false;
    }

    public void Heal(float amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth += amount;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);

        OnHealthChanged?.Invoke(CurrentHealth / MaxHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
}
