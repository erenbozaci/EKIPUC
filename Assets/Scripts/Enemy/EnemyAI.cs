using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    public Transform groundCheck;
    public float groundCheckDistance = 1f;
    public LayerMask groundLayer;
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;
    public LayerMask wallLayer;
    public Transform player;
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public float attackCooldown = 1.5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private bool movingRight = true;
    private float lastAttackTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            rb.velocity = Vector2.zero;
            Attack();
        }
        else if (distanceToPlayer < detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D wallInfo = Physics2D.Raycast(wallCheck.position, movingRight ? Vector2.right : Vector2.left, wallCheckDistance, wallLayer);

        if (!groundInfo || wallInfo)
            Flip();

        rb.velocity = new Vector2((movingRight ? 1 : -1) * speed, rb.velocity.y);
        anim.SetBool("isWalking", true);
    }

    void ChasePlayer()
    {
        if (player.position.x > transform.position.x && !movingRight)
            Flip();
        else if (player.position.x < transform.position.x && movingRight)
            Flip();

        rb.velocity = new Vector2((movingRight ? 1 : -1) * speed, rb.velocity.y);
        anim.SetBool("isWalking", true);
    }

    void Attack()
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            anim.SetTrigger("attack");
            // Hasar verme kaldýrýldý
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        sr.flipX = !sr.flipX;
    }

    public void Die()
    {
        anim.SetTrigger("die");
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 0.5f);
    }
}
