using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityHealth : MonoBehaviour, IEntity
{
    [Header("Entity Health Settings")]
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    

    public virtual bool TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public virtual void Die()
    {
        // Handle death logic here
        Debug.Log($"{gameObject.name} has died.");
    }

    public virtual void Move(Vector3 direction)
    {}

    public virtual void Attack(Vector3 targetPosition)
    {}
}
