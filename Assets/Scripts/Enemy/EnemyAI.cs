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

    [SerializeField] private float attackPauseDuration = 1f; // Saldýrý sonrasý bekleme süresi
    private bool isAttacking = false; // Saldýrý sýrasýnda mý?
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private bool movingRight = true;
    private float lastAttackTime;
    private GrabbableObject grabCheck;
    private bool isStunned = false;
    [SerializeField] private float stunDuration = 1.5f;
    private float stunTimer = 5f;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        grabCheck = GetComponent<GrabbableObject>();

        if (grabCheck != null)
        {
            grabCheck.OnReleased += StunAfterGrab;
            Debug.Log($"{gameObject.name} event'e baðlandý!");
        }

    }

    void Update()
    {
        if (grabCheck != null && grabCheck.isGrabbed)
        {
            
            anim.SetBool("isWalking", false);
            return;
        }

        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            
            anim.SetBool("isWalking", false);

            if (stunTimer <= 0f)
            {
                isStunned = false;
                Debug.Log($"{gameObject.name} toparlandý.");
            }
            return;
        }

        if (isAttacking)
        {
            
            return; // saldýrý bitmeden baþka hareket yapma
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            
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

            StartCoroutine(AttackPause()); // saldýrý süresi boyunca dur
        }
    }

    private System.Collections.IEnumerator AttackPause()
    {
        isAttacking = true;
        

        yield return new WaitForSeconds(attackPauseDuration);

        isAttacking = false;
    }


    void Flip()
    {
        movingRight = !movingRight;
        sr.flipX = !sr.flipX;
    }

    public void Die()
    {
        anim.SetTrigger("die");
        
        Destroy(gameObject, 0.5f);
    }

    private void StunAfterGrab()
    {
        Debug.Log($"{gameObject.name} sersemledi!");
        isStunned = true;
        stunTimer = stunDuration;
    }

    void OnDestroy()
    {
        if (grabCheck != null)
            grabCheck.OnReleased -= StunAfterGrab;
    }

    public bool IsStunned()
    {
        return isStunned;
    }

}
