using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invincibilityTime = 1f;
    [SerializeField] private bool showDamageFlash = true;
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    public event Action<int, int> OnHealthChanged; // current, max
    public event Action OnDamage;
    public event Action OnDeath;

    private int currentHealth;
    private bool isInvincible = false;
    private float invincibilityTimeCounter = 0f;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsInvincible => isInvincible;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimeCounter -= Time.deltaTime;
            if (invincibilityTimeCounter <= 0f)
            {
                isInvincible = false;
                if (spriteRenderer != null)
                    spriteRenderer.color = originalColor;
            }
        }
    }

    public bool TakeDamage(int damage)
    {
        if (IsDead || isInvincible) return false;
        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnDamage?.Invoke();
        if (showDamageFlash && spriteRenderer != null)
            spriteRenderer.color = damageFlashColor;
        isInvincible = true;
        invincibilityTimeCounter = invincibilityTime;
        if (currentHealth <= 0)
            Die();
        return true;
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void HealFull()
    {
        if (IsDead) return;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        currentHealth = 0;
        OnDeath?.Invoke();
    }

    public void Revive()
    {
        if (!IsDead) return;
        IsDead = false;
        currentHealth = maxHealth / 2;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
