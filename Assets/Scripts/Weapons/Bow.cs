using UnityEngine;

class Bow : WeaponAbstract
{
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float arrowSpeed = 20f;

    void Start()
    {
        if (arrowPrefab == null)
        {
            Debug.LogError("Arrow Prefab is not assigned in Bow script.");
        }
        if (firePoint == null)
        {
            Debug.LogError("Fire Point is not assigned in Bow script.");
        }
    }

    public override void Attack()
    {
        if (arrowPrefab != null && firePoint != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = firePoint.right * arrowSpeed;
            }
            else
            {
                Debug.LogError("Arrow prefab does not have a Rigidbody2D component.");
            }
        }
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Debug.Log($"Bow specific info: Arrow Speed: {arrowSpeed}");
    }
    
}