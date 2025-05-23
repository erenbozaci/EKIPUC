using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Test için karaktere hasar vermek amacıyla kullanılacak basit bir script
/// </summary>
public class DamageTest : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    
    private CharacterController playerController;
    
    private void Start()
    {
        playerController = FindObjectOfType<CharacterController>();
    }
    
    private void Update()
    {
        // E tuşuna basılınca karaktere hasar ver
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerController != null)
            {
                playerController.TakeDamage(damageAmount);
                Debug.Log($"Oyuncuya {damageAmount} hasar verildi. Kalan sağlık: {playerController.CurrentHealth}");
            }
        }
        
        // R tuşuna basılınca karakteri iyileştir
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (playerController != null)
            {
                playerController.Heal(damageAmount * 2);
                Debug.Log($"Oyuncu {damageAmount * 2} iyileştirildi. Güncel sağlık: {playerController.CurrentHealth}");
            }
        }
    }
}
