using System;
using UnityEngine;

public class PlayerPickupRaycast : MonoBehaviour
{
    public float pickupRange = 1.5f; // Ray uzunluğu
    public KeyCode pickupKey = KeyCode.E; // Alma tuşu
    public LayerMask pickableLayer; // Hedef nesne katmanı
    public Transform rayOrigin; // Ray'in çıkış noktası
    private Vector2 lookDirection = Vector2.right; // Varsayılan bakış yönü
    
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        UpdateLookDirection();

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, lookDirection, pickupRange, pickableLayer);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Pickable"))
            {
                playerController.InteractKeySprite.SetActive(true);
                if (Input.GetKeyDown(pickupKey))
                {
                    PickUpItem(hit.collider.gameObject.GetComponent<Pickable>());
                }
            }
            else
            {
                playerController.InteractKeySprite.SetActive(false);
            }
        }
        else
        {
            playerController.InteractKeySprite.SetActive(false);
        }
        
        Debug.DrawRay(rayOrigin.position, lookDirection * pickupRange, Color.green);
    }

    void UpdateLookDirection()
    {
        float move = Input.GetAxisRaw("Horizontal");
        if (move != 0)
        {
            lookDirection = new Vector2(move, 0).normalized;
        }
    }

    void PickUpItem(Pickable pickable)
    {
        pickable.OnPick();
        Destroy(pickable.gameObject);
    }
}