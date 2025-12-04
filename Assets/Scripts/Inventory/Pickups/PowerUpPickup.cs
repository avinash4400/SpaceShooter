using UnityEngine;

public class PowerUpPickup : BasePickup
{
    [Header("Specific Configuration")]
    [SerializeField] private PowerUpDataSO powerUpData;

    public void Initialize(PowerUpDataSO data)
    {
        this.powerUpData = data;
    }

    public override bool Collect(IActor target)
    {
        if (powerUpData == null) return false;

        PowerUpInventory inventory = target.GetAttachedComponent<PowerUpInventory>();

        if (inventory != null)
        {
            inventory.AddPowerUp(powerUpData);

            // NEW: Trigger Audio Event
            if (EventManager.Instance != null)
            {
                EventManager.Instance.TriggerPowerUpCollected(powerUpData);
            }

            return true;
        }

        return false;
    }
}