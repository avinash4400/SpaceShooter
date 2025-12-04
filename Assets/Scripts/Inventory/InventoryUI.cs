using UnityEngine;
using UnityEngine.UI;
using TMPro; // Using TextMeshPro for better text rendering

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text component to display current ammo count.")]
    [SerializeField] private TextMeshProUGUI ammoCountText;

    [Tooltip("Image component to display current weapon or power-up icon.")]
    [SerializeField] private Image weaponIcon;

    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color emptyColor = Color.red;

    private void Start()
    {
        // Initialize UI if needed
        if (ammoCountText == null)
        {
            Debug.LogWarning("InventoryUI: Ammo Count Text reference is missing.");
        }
    }

    /// <summary>
    /// Updates the text display for ammo count.
    /// Called by the PlayerController or WeaponSystem when ammo changes.
    /// </summary>
    /// <param name="bulletType">The type of bullet currently equipped.</param>
    /// <param name="count">The current number of bullets remaining.</param>
    public void UpdateAmmoCount(BulletTypeSO bulletType, int count)
    {
        if (ammoCountText != null)
        {
            if (bulletType.hasLimitedAmmo)
            {
                ammoCountText.text = count.ToString();

                // Optional: Change color if low ammo
                if (count <= 10 && count > 0)
                    ammoCountText.color = warningColor;
                else if (count <= 0)
                    ammoCountText.color = emptyColor;
                else
                    ammoCountText.color = normalColor;
            }
            else
            {
                // For infinite ammo types (like Single Shot)
                // Using Unicode Infinity Symbol
                ammoCountText.text = "\u221E";
                ammoCountText.color = normalColor;
            }
        }
    }

    /// <summary>
    /// Updates the weapon icon displayed in the UI.
    /// </summary>
    public void UpdateWeaponIcon(Sprite icon)
    {
        if (weaponIcon != null && icon != null)
        {
            weaponIcon.sprite = icon;
            weaponIcon.enabled = true;
        }
        else if (weaponIcon != null)
        {
            weaponIcon.enabled = false;
        }
    }

    // --- Power-Up Handlers ---

    /// <summary>
    /// Displays a temporary power-up message or effect.
    /// </summary>
    public void ShowPowerUpIndicator(string powerUpName, float duration)
    {
        // Example implementation for showing a powerup popup
        Debug.Log($"UI: PowerUp Acquired - {powerUpName} for {duration} seconds");
        // Logic to show/hide a panel or text would go here
    }
}