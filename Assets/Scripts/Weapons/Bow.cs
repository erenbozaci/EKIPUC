using UnityEngine;

class Bow : WeaponAbstract
{
    public GameObject arrowPrefab;
    public float arrowSpeed = 20f;

    void Start()
    {
        if (arrowPrefab == null)
        {
            Debug.LogError("Arrow Prefab is not assigned in Bow script.");
        }
    }

    public override void Attack(Vector3 targetPosition)
    {
        if (arrowPrefab == null)
        {
            Debug.LogError("Arrow Prefab or Fire Point is not set.");
            return;
        }

        GameObject arrow = Instantiate(arrowPrefab, targetPosition, Quaternion.identity);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = (targetPosition).normalized;
            rb.velocity = direction * arrowSpeed;
        }
        else
        {
            Debug.LogError("Arrow prefab does not have a Rigidbody2D component.");
        }
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Debug.Log($"Bow specific info: Arrow Speed: {arrowSpeed}");
    }
    
}