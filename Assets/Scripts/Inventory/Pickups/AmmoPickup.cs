using UnityEngine;

public class AmmoPickup : BasePickup
{
    [Header("Ammo Configuration")]
    [Tooltip("Which bullet type does this pickup replenish?")]
    [SerializeField] private BulletTypeSO bulletType;

    [Tooltip("How much ammo to add?")]
    [SerializeField] private int ammoAmount = 20;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;

    // BasePickup handles Update() for Movement via LootMovementSO.

    /// <summary>
    /// Implementation of the IPickup contract.
    /// Returns true if the item was successfully consumed.
    /// </summary>
    public override bool Collect(IActor target)
    {
        if (target == null) return false;

        // Get the inventory via the Actor interface
        BulletInventory inventory = target.GetAttachedComponent<BulletInventory>();

        if (inventory != null && bulletType != null)
        {
            inventory.AddAmmo(bulletType, ammoAmount);

            // Generic Audio Event: Passes the specific clip to the EventManager
            // Note: Ensure EventManager has: public void TriggerPickupSound(AudioClip clip)
            if (EventManager.Instance != null && pickupSound != null)
            {
                EventManager.Instance.TriggerPickupSound(pickupSound);
            }

            return true; // Successfully collected
        }

        return false; // Could not collect (no inventory or bad config)
    }
}