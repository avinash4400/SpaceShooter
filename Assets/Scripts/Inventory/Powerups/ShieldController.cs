using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the instantiated Shield object.
/// Attached to the Shield Prefab.
/// </summary>
public class ShieldController : MonoBehaviour, IDamageHandler
{
    [Header("Visuals")]
    [SerializeField] private Renderer shieldRenderer;
    [SerializeField] private Color flashColor = Color.white;

    private int currentHealth;
    private Color originalColor;

    /// <summary>
    /// Initializes the shield with duration and health settings.
    /// </summary>
    public void Initialize(float duration, int health)
    {
        currentHealth = health;

        if (shieldRenderer != null)
        {
            originalColor = shieldRenderer.material.color;
        }

        if (duration > 0)
        {
            StartCoroutine(DurationCoroutine(duration));
        }
    }

    /// <summary>
    /// Implementation of IDamageHandler.
    /// </summary>
    public void HandleDamage(DamageInfo info)
    {
        currentHealth -= info.DamageAmount;

        if (shieldRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }


        if (currentHealth <= 0)
        {
            BreakShield();
        }
    }

    private void BreakShield()
    {
        Destroy(gameObject);
    }

    private IEnumerator DurationCoroutine(float duration)
    {
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