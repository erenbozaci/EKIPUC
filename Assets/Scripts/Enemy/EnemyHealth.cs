using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    private EnemyAI ai;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        ai = GetComponent<EnemyAI>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        // Buraya vurulma efekti/animasyonu da koyabilirsin
        Debug.Log("Enemy took damage! HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
            ai.Die();
        }
    }
}
