using UnityEngine;
using UnityEngine.UI; // Required for Image and Text components
using System;
using TMPro;

/// <summary>
/// Handles the display of the Player's Bullet Inventory (selected icon/ammo count)
/// and Power-Up Inventory (selected icon/count).
/// Listens exclusively to events broadcasted by the inventory systems.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("Bullet UI References (Bottom-Left)")]
    [SerializeField] private Image bulletIconImage;
    [SerializeField] private TMP_Text ammoCountText;

    [Header("Power-Up UI References (Bottom-Right)")]
    // Note: The Power-Up UI in the GDD is complex (scrollable bar).
    // These references handle the currently selected power-up only for now.
    [SerializeField] private Image powerupIconImage;
    [SerializeField] private TMP_Text powerupCountText;


    void OnEnable()
    {
        // Subscribe to Bullet Inventory Events
        BulletInventory.OnBulletSelected += UpdateBulletIcon;
        BulletInventory.OnAmmoCountChanged += UpdateAmmoCount;

        // Subscribe to Power-Up Inventory Events (Placeholder until Day 4)
        // PowerupInventory.OnPowerupSelected += UpdatePowerupIcon;
        // PowerupInventory.OnPowerupCountChanged += UpdatePowerupCount;
    }

    void OnDisable()
    {
        // Unsubscribe from Bullet Inventory Events
        BulletInventory.OnBulletSelected -= UpdateBulletIcon;
        BulletInventory.OnAmmoCountChanged -= UpdateAmmoCount;

        // Unsubscribe from Power-Up Inventory Events (Placeholder)
        // PowerupInventory.OnPowerupSelected -= UpdatePowerupIcon;
        // PowerupInventory.OnPowerupCountChanged -= UpdatePowerupCount;
    }

    /// <summary>
    /// Updates the bullet icon when a new bullet type is selected.
    /// </summary>
    private void UpdateBulletIcon(BulletTypeSO bulletType)
    {
        if (bulletIconImage != null)
        {
            bulletIconImage.sprite = bulletType.icon;
            bulletIconImage.color = Color.white;

            // Also refresh the ammo count when the bullet type changes
            // (Assumes BulletInventory will broadcast the current count immediately after selection)
        }
    }

    /// <summary>
    /// Updates the text display for ammo count.
    /// </summary>
    private void UpdateAmmoCount(BulletTypeSO bulletType, int count)
    {
        if (ammoCountText != null)
        {
            if (bulletType.hasLimitedAmmo)
            {
                ammoCountText.text = count.ToString();

                // Optional: Change color if low ammo
                if (count <= 10 && count > 0)
                    ammoCountText.color = Color.yellow;
                else if (count <= 0)
                    ammoCountText.color = Color.red;
                else
                    ammoCountText.color = Color.white;
            }
            else
            {
                // For infinite ammo types (like Single Shot)
                ammoCountText.text = "\u221E";
                ammoCountText.color = Color.white;
            }
        }
    }

    // --- Power-Up Handlers (Placeholder for Day 4) ---

    /*
    private void UpdatePowerupIcon(PowerupTypeSO powerupType)
    {
        if (powerupIconImage != null)
        {
            powerupIconImage.sprite = powerupType.icon;
        }
    }
    
    private void UpdatePowerupCount(PowerupTypeSO powerupType, int count)
    {
        if (powerupCountText != null)
        {
            powerupCountText.text = $"x{count}";
        }
    }
    */
}