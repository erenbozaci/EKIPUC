using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public event Action<int> OnHealthChanged;
    public event Action OnDeath;
    public HealthBar healthBar; // << referans


    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer bileþeni Character üzerinde bulunamadý!");
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }

    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }


        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

    }



    private void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}