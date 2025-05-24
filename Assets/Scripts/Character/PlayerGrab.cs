using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    public Transform holdPoint; // El pozisyonu
    public float grabRange = 1.5f;
    public LayerMask grabbableLayer;
    public float throwForce = 10f;

    private GrabbableObject heldObject;
    private Rigidbody2D heldRb;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                TryGrab();
            else
                Drop();
        }
        if (Input.GetKeyDown(KeyCode.F)) // örnek: F tuþu ile fýrlat
        {
            if (heldObject != null)
            {
                Throw();
            }
        }

    }

    void TryGrab()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, grabRange, grabbableLayer);

        foreach (var hit in hits)
        {
            GrabbableObject grabbable = hit.GetComponent<GrabbableObject>();
            if (grabbable != null && !grabbable.isBeingHeld)
            {
                heldObject = grabbable;
                heldObject.isBeingHeld = true;

                heldRb = heldObject.GetComponent<Rigidbody2D>();
                heldRb.isKinematic = true;
                heldRb.velocity = Vector2.zero;

                heldObject.transform.position = holdPoint.position;
                heldObject.transform.SetParent(holdPoint);
                break;
            }
        }
    }

    void Drop()
    {
        if (heldObject != null)
        {
            heldObject.isBeingHeld = false;
            heldObject.transform.SetParent(null);

            heldRb.isKinematic = false;

            heldObject.Release(); // <<<<< BUNU KULLANIYOR MUSUN?
            heldObject = null;
            heldRb = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }


    void Throw()
    {
        if (heldObject != null)
        {
            Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            Vector2 force = direction * throwForce + Vector2.up * 2f;

            heldObject.Release(true, force); // coroutine burada çalýþýyor

            heldObject = null;
            heldRb = null;
        }
    }

}
