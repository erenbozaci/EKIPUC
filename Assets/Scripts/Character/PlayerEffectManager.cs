using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class PlayerEffectManager : MonoBehaviour
{
    public static PlayerEffectManager Instance;

    [Header("Particle Effects")]
    public GameObject hitEffectPrefab;
    public GameObject shootEffectPrefab;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip hitSound;
    public AudioSource audioSource;

    [Header("Camera Shake")]
    public Camera mainCamera;
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.1f;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Vuruş efekti
    public void PlayHitEffect(Vector2 position)
    {
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, position, Quaternion.identity);

        PlaySound(hitSound);
        StartCoroutine(CameraShake());
    }

    // Ok fırlatma efekti
    public void PlayShootEffect(Vector2 position)
    {
        if (shootEffectPrefab != null)
            Instantiate(shootEffectPrefab, position, Quaternion.identity);

        PlaySound(shootSound);
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    System.Collections.IEnumerator CameraShake()
    {
        if (mainCamera == null) yield break;

        Vector3 originalPos = mainCamera.transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;
            mainCamera.transform.position = new Vector3(originalPos.x + offsetX, originalPos.y + offsetY, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalPos;
    }
}

