using UnityEngine;

/// <summary>
/// A pickup item that instantly restores health to the actor who touches it.
/// </summary>
public class HealthPickup : BasePickup
{
    [Header("Configuration")]
    [Tooltip("Amount of health points to restore.")]
    [SerializeField] private int healAmount = 1;

    [Tooltip("Sound to play on pickup (optional override, typically handled by generic pickup logic).")]
    [SerializeField] private AudioClip pickupSound;

    public override bool Collect(IActor target)
    {
        // 1. Find Health Component
        // Try getting via Interface first (preferred architecture)
        HealthComponent health = target.GetAttachedComponent<HealthComponent>();

        // Fallback if interface lookup fails (e.g. component not registered in list but present on object)
        if (health == null)
        {
            health = target.GetTransform().GetComponent<HealthComponent>();
        }

        // 2. Apply Heal
        if (health != null)
        {
            // Check if healing is actually needed (don't consume if full)
            if (health.CurrentHealth < health.MaxHealth)
            {
                health.Heal(healAmount);

                // Optional: Play specific sound via EventManager if needed, 
                // though BasePickup usually handles generic pickup FX.
                // If we had a generic 'OnItemCollected' event in EventManager, we'd fire it here.

                return true; // Consume item
            }
        }

        return false; // Don't consume if full health or no health component
    }
}