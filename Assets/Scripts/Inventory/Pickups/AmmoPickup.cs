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


    /// <summary>
    /// Implementation of the IPickup contract.
    /// Returns true if the item was successfully consumed.
    /// </summary>
    public override bool Collect(IActor target)
    {
        if (target == null) return false;

        BulletInventory inventory = target.GetAttachedComponent<BulletInventory>();

        if (inventory != null && bulletType != null)
        {
            inventory.AddAmmo(bulletType, ammoAmount);

            if (EventManager.Instance != null && pickupSound != null)
            {
                EventManager.Instance.TriggerPickupSound(pickupSound);
            }

            return true;
        }

        return false; 
    }
}