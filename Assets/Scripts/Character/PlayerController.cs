using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerController : MonoBehaviour, IEntity
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool facingRight = true;
    
    private PlayerHealth playerHealth; // Health system variable

    public WeaponAbstract currentWeapon;
    public TMPro.TextMeshPro textMesh;
    
    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("CharacterHealth bileşeni Player üzerinde bulunamadı!");
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D bileşeni Player üzerinde bulunamadı!");
        }

        if (groundCheck == null)
        {
            Debug.LogWarning("GroundCheck Transform'u PlayerController'a atanmamış. Yere temas kontrolü düzgün çalışmayabilir.");
        }
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            isGrounded = true;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public void TakeDamage(int damage)
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
        else
        {
            Debug.LogError("CharacterHealth bileşeni bulunamadı!");
        }
    }

    public void Heal(int amount)
    {
        if (playerHealth != null)
        {
             playerHealth.Heal(amount);
        }
        else
        {
            Debug.LogError("CharacterHealth bileşeni bulunamadı!");
        }
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }

    public void Move(Vector3 direction)
    {
    }

    public void Attack(Vector3 targetPosition)
    {
        // Attack logic here
        Debug.Log("Attack at " + targetPosition);
    }
    
    public void EquipWeapon(WeaponAbstract weapon)
    {
        if (currentWeapon != null)
        {
            return;
        }
        
        currentWeapon = weapon;
        Debug.Log("Equipped weapon: " + currentWeapon.weaponName);
    }
    public void IsEquipped(WeaponEquipable weapon)
    {
        if (weapon != null)
        {
            currentWeapon = weapon.getWeapon();
            if (currentWeapon != null)
            {
                Debug.Log("Weapon equipped: " + currentWeapon.weaponName);
            }
            else
            {
                Debug.LogError("WeaponAbstract bileşeni bulunamadı!");
            }
        }
        else
        {
            Debug.LogError("WeaponEquipable bileşeni null!");
        }
    }
    
    public void showTextMesh()
    {
        if (textMesh != null)
        {
            textMesh.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("TextMeshPro bileşeni Player üzerinde bulunamadı!");
        }
    }
    
    public void hideTextMesh()
    {
        if (textMesh != null)
        {
            textMesh.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("TextMeshPro bileşeni Player üzerinde bulunamadı!");
        }
    }
    
    public void setPlayerText(string text)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
        }
        else
        {
            Debug.LogError("TextMeshPro bileşeni Player üzerinde bulunamadı!");
        }
    }
}
