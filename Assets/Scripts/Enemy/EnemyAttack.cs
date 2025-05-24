using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float knockbackForceX = 10f;

    private float lastAttackTime;
    private GrabbableObject grabCheck;
    private EnemyAI enemyAI;
    private bool canDealDamage = true;
    [SerializeField] private float damageCooldownAfterThrow = 1.0f;



    void Start()
    {
        grabCheck = GetComponent<GrabbableObject>();
        enemyAI = GetComponent<EnemyAI>();

        if (grabCheck != null)
        {
            grabCheck.OnReleased += HandleReleased;
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (grabCheck != null && grabCheck.isRecentlyThrown)
        {
            Debug.Log($" Hasar verilmedi ��nk� d��man yeni f�rlat�ld�");
            return;
        }

        Debug.Log($" {gameObject.name} �arp��t� � canDealDamage: {canDealDamage}");

        if (!canDealDamage)
        {
            Debug.Log($" Hasar verilmedi ��nk� canDealDamage = false");
            return;
        }

        if (grabCheck != null && grabCheck.isBeingHeld)
        {
            Debug.Log($" Hasar verilmedi ��nk� d��man elde");
            return;
        }

        if (enemyAI != null && enemyAI.IsStunned())
        {
            Debug.Log($" Hasar verilmedi ��nk� d��man sersem");
            return;
        }

        if (Time.time < lastAttackTime + attackCooldown)
        {
            Debug.Log($" Cooldown hen�z dolmam��");
            return;
        }

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

                Debug.Log($" Hasar verildi, knockback yolland�");
            }
        }

    }


    private void HandleReleased()
    {
        canDealDamage = false;
        Debug.Log($" {gameObject.name} f�rlat�ld�, hasar KAPATILDI");
        StartCoroutine(EnableDamageAfterDelay(1f));
        //StartCoroutine(EnableDamageAfterDelay(damageCooldownAfterThrow));
    }

    private System.Collections.IEnumerator EnableDamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canDealDamage = true;
        Debug.Log($" {gameObject.name} tekrar hasar verebilir");
    }

    void OnDestroy()
    {
        if (grabCheck != null)
            grabCheck.OnReleased -= HandleReleased;
    }

}
