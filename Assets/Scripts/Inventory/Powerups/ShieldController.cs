using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the instantiated Shield object.
/// Intercepts damage (via IDamageHandler) and manages its own lifetime/health.
/// Attached to the Shield Prefab.
/// </summary>
public class ShieldController : MonoBehaviour, IDamageHandler
{
    [Header("Visuals")]
    [SerializeField] private Renderer shieldRenderer;
    [SerializeField] private Color flashColor = Color.white;

    // State
    private int currentHealth;
    private Color originalColor;

    /// <summary>
    /// Initializes the shield with duration and health settings.
    /// Called by the ShieldPowerUpSO immediately after spawning.
    /// </summary>
    public void Initialize(float duration, int health)
    {
        currentHealth = health;

        if (shieldRenderer != null)
        {
            originalColor = shieldRenderer.material.color;
        }

        // Start the lifetime countdown
        if (duration > 0)
        {
            StartCoroutine(DurationCoroutine(duration));
        }
    }

    /// <summary>
    /// Implementation of IDamageHandler.
    /// This allows the shield to intercept bullets that hit its collider.
    /// </summary>
    public void HandleDamage(DamageInfo info)
    {
        currentHealth -= info.DamageAmount;

        // Visual feedback (flash)
        if (shieldRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }

        Debug.Log($"[ShieldController] Shield took {info.DamageAmount} damage. Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            BreakShield();
        }
    }

    private void BreakShield()
    {
        // Optional: Play shatter sound/VFX
        Destroy(gameObject);
    }

    private IEnumerator DurationCoroutine(float duration)
    {
        // Could implement a blinking effect near the end of duration here
        yield return new WaitForSeconds(duration);
        BreakShield();
    }

    private IEnumerator FlashEffect()
    {
        if (shieldRenderer == null) yield break;

        shieldRenderer.material.color = flashColor;
        yield return new WaitForSeconds(0.1f);
        shieldRenderer.material.color = originalColor;
    }
}