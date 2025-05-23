using System;
using UnityEngine;
using UnityEngine.UI;  // UI namespace ekle

public class CharacterController : MonoBehaviour
{
    #region Variables
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 50f;
    [SerializeField] private float airControlMultiplier = 0.7f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    [Header("Jump Settings")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private int maxAirJumps = 0;

    [Header("Wall Movement")]
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private float wallCheckDistance = 0.2f;
    [SerializeField] private float wallSlideSpeed = 1f;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private Vector2 wallJumpDirection = new Vector2(1f, 2f);
    [SerializeField] private float wallJumpTime = 0.15f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private TrailRenderer dashTrail;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invincibilityTime = 1f;
    [SerializeField] private bool showDamageFlash = true;
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    // References
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // Movement
    private float horizontalInput;
    private bool facingRight = true;
    private Vector2 velocity;

    // Jump variables
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isJumping;
    private int airJumpsLeft;

    // Ground check
    private bool isGrounded;

    // Wall variables
    private bool isTouchingWall;
    private bool isWallSliding;
    private float wallJumpCounter;

    // Dash variables
    private bool canDash = true;
    private bool isDashing;
    private float dashTimeCounter;
    private float dashCooldownCounter;

    // Health variables
    private int currentHealth;
    private bool isInvincible = false;
    private float invincibilityTimeCounter = 0f;
    private Color originalSpriteColor;
    private bool isDead = false;

    // State
    private CharacterState currentState = CharacterState.Idle;

    // Events
    public event Action<CharacterState> OnStateChanged;
    public event Action OnJump;
    public event Action OnLand;
    public event Action OnDash;
    public event Action<int, int> OnHealthChanged; // Current health, max health
    public event Action OnDamage;
    public event Action OnDeath;

    // UI
    private Image healthBarUI;

    #endregion

    #region Properties
    
    public bool IsGrounded => isGrounded;
    public CharacterState CurrentState => currentState;
    public bool IsFacingRight => facingRight;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsInvincible => isInvincible;
    public bool IsDead => isDead;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Sağlık sistemini başlat
        currentHealth = maxHealth;
        if (spriteRenderer != null)
        {
            originalSpriteColor = spriteRenderer.color;
        }

        // Configure Rigidbody2D for better platformer physics
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        // Create wall check point if not assigned
        if (wallCheckPoint == null)
        {
            GameObject wallCheck = new GameObject("WallCheckPoint");
            wallCheck.transform.parent = transform;
            wallCheck.transform.localPosition = new Vector3(0.5f, 0, 0);
            wallCheckPoint = wallCheck.transform;
        }

        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }

        // Verify collider settings
        VerifyColliderSetup();
        
        OnHealthChanged += (current, max) =>
        {
            if (healthBarUI != null)
                healthBarUI.fillAmount = (float)current / max;
        };
    }

    private void Update()
    {
        if (isDashing) return;
        
        // Ölümse kontrol et ve işlem yapma
        if (isDead) return;

        GetInput();
        UpdateTimers();
        CheckForWall();
        
        HandleJumpInput();
        HandleDashInput();
        HandleAttackInput();


        UpdateCharacterState();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            HandleDashing();
            return;
        }

        CheckGround();
        HandleWallSliding();
        ProcessMovement();
        BetterJump();
    }

    #endregion

    #region Input Handling

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Z)) // Saldırı tuşu
        {
            Vector2 attackDirection = facingRight ? Vector2.right : Vector2.left;
            float attackRange = 1f; // menzil

            RaycastHit2D hit = Physics2D.Raycast(transform.position, attackDirection, attackRange, LayerMask.GetMask("Enemy"));

            Debug.DrawRay(transform.position, attackDirection * attackRange, Color.yellow, 0.2f);

            if (hit.collider != null)
            {
                // Düşman bulundu
                EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(1); // 1 hasar veriyoruz
                    Debug.Log("Enemy hit!");
                }
            }
        }
    }


    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        
        // Flip character based on input direction
        if (horizontalInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && facingRight)
        {
            Flip();
        }
    }

    private void HandleJumpInput()
    {
        // Jump buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Jump işlemi
        if (jumpBufferCounter > 0f)
        {
            // Normal jump from ground or wall
            if ((coyoteTimeCounter > 0f || isWallSliding) && !isJumping)
            {
                if (isWallSliding)
                {
                    WallJump();
                }
                else
                {
                    Jump();
                }
                jumpBufferCounter = 0f;
            }
            // Double jump in air
            else if (airJumpsLeft > 0 && isJumping)
            {
                Jump();
                airJumpsLeft--;
                jumpBufferCounter = 0f;
            }
        }

        // Jump iptal (düşük zıplama için)
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartDash();
        }
    }

    #endregion

    #region Movement

    private void ProcessMovement()
    {
        // Wall jump overrides user input for a brief time
        if (wallJumpCounter > 0)
        {
            wallJumpCounter -= Time.fixedDeltaTime;
            return;
        }

        // Movement calculation
        float targetSpeed = horizontalInput * moveSpeed;

        // Reduce control in air
        if (!isGrounded)
        {
            targetSpeed *= airControlMultiplier;
        }

        float speedDiff = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, 0.96f) * Mathf.Sign(speedDiff);
        
        // Apply
        rb.AddForce(movement * Vector2.right);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        coyoteTimeCounter = 0f;
        isJumping = true;
        OnJump?.Invoke();
    }

    private void WallJump()
    {
        Vector2 direction = wallJumpDirection;
        
        // Adjust direction based on which side the wall is
        if (isTouchingWall && horizontalInput > 0)
        {
            direction.x = -wallJumpDirection.x;
        }
        
        // Normalize and apply the wall jump force
        direction = direction.normalized * wallJumpForce;
        
        rb.velocity = Vector2.zero;
        rb.AddForce(direction, ForceMode2D.Impulse);
        
        wallJumpCounter = wallJumpTime;
        isJumping = true;
        OnJump?.Invoke();
    }

    private void StartDash()
    {
        isDashing = true;
        canDash = false;
        dashTimeCounter = dashTime;
        dashCooldownCounter = dashCooldown;
        
        // Disable gravity during dash
        rb.gravityScale = 0;
        
        if (dashTrail != null)
        {
            dashTrail.emitting = true;
        }
        
        OnDash?.Invoke();
    }

    private void HandleDashing()
    {
        if (dashTimeCounter > 0)
        {
            // Apply dash force
            float dashDirection = facingRight ? 1f : -1f;
            rb.velocity = new Vector2(dashDirection * dashSpeed, 0);
            dashTimeCounter -= Time.fixedDeltaTime;
        }
        else
        {
            // End dash
            isDashing = false;
            rb.gravityScale = 1; // Reset gravity
            
            if (dashTrail != null)
            {
                dashTrail.emitting = false;
            }
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        
        // Option 1: Flip the sprite renderer
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !facingRight;
        }
        // Option 2: Flip the transform (if not using sprite renderer flip)
        else
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    #endregion

    #region Physics Checks

    private void CheckGround()
    {
        if (groundCheck == null) 
        {
            Debug.LogWarning("Ground check is not assigned on " + gameObject.name);
            return;
        }
        
        // Verify ground layer mask is set
        if (groundLayer.value == 0)
        {
            Debug.LogWarning("Ground layer mask is not set on " + gameObject.name);
            // Attempt to use a default layer if none is set
            groundLayer = LayerMask.GetMask("Ground") != 0 ? 
                LayerMask.GetMask("Ground") : (1 << 6); // Layer 6 is often used for ground
        }
        
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Debug visuals for ground detection
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckRadius, isGrounded ? Color.green : Color.red);

        // Coyote time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            isJumping = false;
            airJumpsLeft = maxAirJumps;
            
            // Landing event
            if (!wasGrounded)
            {
                OnLand?.Invoke();
            }
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }
    }

    private void CheckForWall()
    {
        if (wallCheckPoint == null) return;
        
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        isTouchingWall = Physics2D.Raycast(
            wallCheckPoint.position,
            direction,
            wallCheckDistance,
            groundLayer
        );
    }

    private void HandleWallSliding()
    {
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    #endregion

    #region Physics Tweaks

    private void BetterJump()
    {
        if (rb.velocity.y < 0)
        {
            // Düşerken daha hızlı düşme
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Düşük zıplama
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    #endregion

    #region State Management

    private void UpdateCharacterState()
    {
        CharacterState newState;

        if (isDashing)
        {
            newState = CharacterState.Dashing;
        }
        else if (isWallSliding)
        {
            newState = CharacterState.WallSliding;
        }
        else if (!isGrounded)
        {
            newState = rb.velocity.y > 0 ? CharacterState.Jumping : CharacterState.Falling;
        }
        else if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            newState = CharacterState.Running;
        }
        else
        {
            newState = CharacterState.Idle;
        }

        if (newState != currentState)
        {
            currentState = newState;
            OnStateChanged?.Invoke(currentState);
        }
    }

    #endregion

    #region Utilities

    private void UpdateTimers()
    {
        // Handle dash cooldown
        if (!canDash)
        {
            dashCooldownCounter -= Time.deltaTime;
            if (dashCooldownCounter <= 0)
            {
                canDash = true;
            }
        }
        
        // Handle invincibility timer
        if (isInvincible)
        {
            invincibilityTimeCounter -= Time.deltaTime;
            if (invincibilityTimeCounter <= 0)
            {
                isInvincible = false;
                // Reset sprite color if it was changed
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = originalSpriteColor;
                }
            }
        }
    }

    private void VerifyColliderSetup()
    {
        // Check if the character's collider is set as trigger
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var collider in colliders)
        {
            if (collider.isTrigger)
            {
                Debug.LogWarning("Character collider is set as trigger. This may cause physics issues.");
                // Optionally automatically fix: collider.isTrigger = false;
            }
        }
        
        // Check if rigidbody settings are appropriate
        if (rb != null)
        {
            if (rb.gravityScale <= 0)
            {
                Debug.LogWarning("Rigidbody2D gravityScale is zero or negative.");
            }
            
            if (rb.bodyType != RigidbodyType2D.Dynamic)
            {
                Debug.LogWarning("Rigidbody2D is not set to Dynamic. Physics won't work properly.");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        else
        {
            // Draw warning if ground check is missing
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position - new Vector3(0, 1, 0), 0.2f);
        }
        
        // Wall check
        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            Vector2 direction = facingRight ? Vector2.right : Vector2.left;
            Gizmos.DrawLine(wallCheckPoint.position, 
                (Vector2)wallCheckPoint.position + direction * wallCheckDistance);
        }
    }

    #endregion

    #region Health System

    /// <summary>
    /// Karaktere hasar verir.
    /// </summary>
    /// <param name="damage">Verilecek hasar miktarı</param>
    /// <returns>Gerçekten hasar alındı mı? (invincible değilse true)</returns>
    public bool TakeDamage(int damage)
    {
        // Karakter ölü veya hasar alamaz durumdaysa işlem yapma
        if (isDead || isInvincible) return false;

        // Hasar al
        currentHealth -= damage;
        
        // Sağlık değişikliği olayını tetikle
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Hasar alma olayını tetikle
        OnDamage?.Invoke();
        
        // Damage alındığında yanıp sönme efekti
        if (showDamageFlash && spriteRenderer != null)
        {
            StartDamageFlash();
        }
        
        // Hasar sonrası görünmezlik süresini başlat
        isInvincible = true;
        invincibilityTimeCounter = invincibilityTime;
        
        // Sağlık 0 veya altındaysa ölüm işlemini başlat
        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        
        return true;
    }
    
    /// <summary>
    /// Karakteri iyileştirir.
    /// </summary>
    /// <param name="amount">İyileştirilecek sağlık miktarı</param>
    public void Heal(int amount)
    {
        // Ölü karakteri iyileştirme
        if (isDead) return;
        
        // Sağlık değerini güncelle ve maksimum sağlığı geçmemesini sağla
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        
        // Sağlık değişikliği olayını tetikle
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    /// <summary>
    /// Karakteri tamamen iyileştirir.
    /// </summary>
    public void HealFull()
    {
        // Ölü karakteri iyileştirme
        if (isDead) return;
        
        // Sağlık değerini maksimuma çıkar
        currentHealth = maxHealth;
        
        // Sağlık değişikliği olayını tetikle
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    /// <summary>
    /// Ölüm işlemlerini gerçekleştirir.
    /// </summary>
    private void Die()
    {
        // Karakter zaten ölüyse işlem yapma
        if (isDead) return;
        
        // Ölüm durumunu ayarla
        isDead = true;
        currentHealth = 0;
        
        // Fizik özelliklerini pasif hale getir
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            rb.isKinematic = true;
        }
        
        // Collider'ı devre dışı bırak
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
        
        // Ölüm olayını tetikle
        OnDeath?.Invoke();
        
        Debug.Log($"{gameObject.name} died!");
    }
    
    /// <summary>
    /// Hasar alındığında renk değiştirme efekti
    /// </summary>
    private void StartDamageFlash()
    {
        if (spriteRenderer == null) return;
        
        spriteRenderer.color = damageFlashColor;
        
        // Coroutine kullanmadan rengi eski haline getirmek için invincibility süresi sonunda rengi değiştireceğiz
        // Eğer coroutine kullanmak istersek, ayrı bir metod yazıp StartCoroutine ile çağırabiliriz
    }
    
    /// <summary>
    /// Karakteri yeniden canlandırır.
    /// </summary>
    public void Revive()
    {
        if (!isDead) return;
        
        // Ölüm durumunu sıfırla
        isDead = false;
        
        // Sağlığı yarısına çıkar (veya istediğiniz bir değere)
        currentHealth = maxHealth / 2;
        
        // Fizik özelliklerini tekrar aktif hale getir
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.gravityScale = 1;
        }
        
        // Collider'ı tekrar aktif et
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
        
        // Sağlık değişikliği olayını tetikle
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    #endregion

}

// Karakter durumlarını tanımlayan enum
public enum CharacterState
{
    Idle,
    Running,
    Jumping,
    Falling,
    WallSliding,
    Dashing
}
