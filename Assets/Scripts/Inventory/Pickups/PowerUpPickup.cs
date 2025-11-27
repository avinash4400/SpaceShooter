using UnityEngine;

/// <summary>
/// Concrete pickup for Power-Ups.
/// Inherits from BasePickup for standard behavior.
/// </summary>
public class PowerUpPickup : BasePickup
{
    [Header("Specific Configuration")]
    [SerializeField] private PowerUpDataSO powerUpData;

    // Optional: Allow initializing dynamically (for Loot Spawner)
    public void Initialize(PowerUpDataSO data)
    {
        this.powerUpData = data;
        // Logic to update sprite renderer based on data.icon could go here
    }

    public override bool Collect(IActor target)
    {
        if (powerUpData == null)
        {
            Debug.LogWarning("[PowerUpPickup] Missing PowerUp Data!");
            return false;
        }

        // Find the inventory via the Actor's Transform
        PowerUpInventory inventory = target.GetTransform().GetComponent<PowerUpInventory>();

        if (inventory != null)
        {
            inventory.AddPowerUp(powerUpData);
            Debug.Log($"[PowerUpPickup] Collected {powerUpData.powerUpName}");
            return true;
        }

        return false;
    }
}