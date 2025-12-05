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
        HealthComponent health = target.GetAttachedComponent<HealthComponent>();

        if (health == null)
        {
            health = target.GetTransform().GetComponent<HealthComponent>();
        }

        if (health != null)
        {
            if (health.CurrentHealth < health.MaxHealth)
            {
                health.Heal(healAmount);

                return true; 
            }
        }

        return false; 
    }
}