using UnityEngine;
using System.Collections;

/// <summary>
/// Shakes the camera when triggered by the EventManager.
/// Listens to the global EventManager.
/// </summary>
public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [Tooltip("How long the shake lasts.")]
    [SerializeField] private float duration = 0.2f;

    [Tooltip("How violently the camera shakes (Radius of random sphere).")]
    [SerializeField] private float magnitude = 0.3f;

    // Cache the initial position to reset after shaking
    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        originalPosition = transform.localPosition;
    }

    void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnCameraShake += TriggerShake;
        }
    }

    void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnCameraShake -= TriggerShake;
        }
    }

    private void TriggerShake()
    {
        // If already shaking, stop and restart (or add duration, but restart feels punchier)
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            transform.localPosition = originalPosition; // Reset first
        }

        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Generate a random offset
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Apply relative to original position
            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Restore original position
        transform.localPosition = originalPosition;
        shakeCoroutine = null;
    }
}