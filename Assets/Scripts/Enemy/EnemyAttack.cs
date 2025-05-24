using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float knockbackForceX = 10f;

    private float lastAttackTime;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            bool tookDamage = player.TakeDamage(damage);
            if (tookDamage)
            {
                float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
                Vector2 knockback = new Vector2(direction * knockbackForceX, 0f);
                player.ApplyKnockback(knockback, 0.2f);
                lastAttackTime = Time.time;
                Debug.Log("Knockback force sent!");
            }
        }

    }
}
