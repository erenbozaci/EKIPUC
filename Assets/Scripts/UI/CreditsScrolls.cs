using UnityEngine;

public class CreditsScroll : MonoBehaviour
{
    public float scrollSpeed = 50f;  // Hýzý ayarla

    void Update()
    {
        transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
    }
}
