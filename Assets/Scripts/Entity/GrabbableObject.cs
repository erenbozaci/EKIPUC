using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Rigidbody2D))]
public class GrabbableObject : MonoBehaviour
{
    public event System.Action OnReleased;

    public bool isBeingHeld = false;
    public bool isGrabbed => isBeingHeld; // shortcut

    public bool isRecentlyThrown = false;


    public void Release(bool applyThrow = false, Vector2 throwForce = default)
    {
        StartCoroutine(ReleaseRoutine(applyThrow, throwForce));
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // Sald�r Kadir!
        rb.isKinematic = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.freezeRotation = true; // d�z kalmas� i�in


    }

    public IEnumerator ReleaseRoutine(bool applyThrow, Vector2 throwForce)
    {
        isBeingHeld = false;
        isRecentlyThrown = true; // <<< BURASI �NEML�
        transform.SetParent(null);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        yield return new WaitForFixedUpdate();

        rb.isKinematic = false;

        if (applyThrow)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(throwForce, ForceMode2D.Impulse);
        }

        OnReleased?.Invoke();

        // Bir s�re sonra tekrar hasar verebilir
        yield return new WaitForSeconds(1f);
        isRecentlyThrown = false; // <<< Koruma s�resi doldu
    }



}
