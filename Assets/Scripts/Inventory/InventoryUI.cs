using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic; 
using TMPro;

/// <summary>
/// Handles the display of the Player's Bullet Inventory (selected icon/ammo count)
/// and Power-Up Inventory (selected icon/count).
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("Bullet UI References (Bottom-Left)")]
    [SerializeField] private Image bulletIconImage;
    [SerializeField] private TMP_Text ammoCountText;

    [Header("Power-Up UI References (Bottom-Right)")]
    [SerializeField] private Image powerupIconImage;
    [SerializeField] private TMP_Text powerupCountText;

    private List<PowerUpDataSO> currentPowerUps = new List<PowerUpDataSO>();
    private PowerUpDataSO currentSelectedPowerUp;
    private BulletTypeSO currentSelectedBulletType;

    void OnEnable()
    {
        BulletInventory.OnBulletSelected += UpdateBulletIcon;
        BulletInventory.OnAmmoCountChanged += UpdateAmmoCount;

        PowerUpInventory.OnPowerUpSelected += UpdatePowerupSelection;
        PowerUpInventory.OnInventoryUpdated += UpdatePowerupList;
    }

    void OnDisable()
    {
        BulletInventory.OnBulletSelected -= UpdateBulletIcon;
        BulletInventory.OnAmmoCountChanged -= UpdateAmmoCount;

        PowerUpInventory.OnPowerUpSelected -= UpdatePowerupSelection;
        PowerUpInventory.OnInventoryUpdated -= UpdatePowerupList;
    }

    /// <summary>
    /// Updates the bullet icon when a new bullet type is selected.
    /// </summary>
    private void UpdateBulletIcon(BulletTypeSO bulletType)
    {
        currentSelectedBulletType = bulletType;
        if (bulletIconImage != null)
        {
            bulletIconImage.sprite = bulletType.icon;
            bulletIconImage.color = Color.white;
        }
    }

    /// <summary>
    /// Updates the text display for ammo count.
    /// </summary>
    private void UpdateAmmoCount(BulletTypeSO bulletType, int count)
    {
        if(currentSelectedBulletType != null && bulletType != currentSelectedBulletType)
            return;
        if (ammoCountText != null)
        {
            if (bulletType.hasLimitedAmmo)
            {
                ammoCountText.text = count.ToString();

                if (count <= 10 && count > 0)
                    ammoCountText.color = Color.yellow;
                else if (count <= 0)
                    ammoCountText.color = Color.red;
                else
                    ammoCountText.color = Color.white;
            }
            else
            {
                ammoCountText.text = "\u221E";
                ammoCountText.color = Color.white;
            }
        }
    }
    private void UpdatePowerupSelection(PowerUpDataSO data)
    {
        currentSelectedPowerUp = data;

        if (powerupIconImage != null)
        {
            if (data != null)
            {
                powerupIconImage.sprite = data.icon;
                powerupIconImage.color = Color.white;
                powerupIconImage.enabled = true;
            }
            else
            {
                powerupIconImage.sprite = null;
                powerupIconImage.color = Color.clear;
                powerupIconImage.enabled = false;
            }
        }

        RefreshPowerupCount();
    }

    private void UpdatePowerupList(List<PowerUpDataSO> list)
    {
        currentPowerUps = list ?? new List<PowerUpDataSO>();
        RefreshPowerupCount();
    }

    private void RefreshPowerupCount()
    {
        if (powerupCountText != null)
        {
            if (currentSelectedPowerUp != null && currentPowerUps != null)
            {
                int count = 0;
                foreach (var item in currentPowerUps)
                {
                    if (item == currentSelectedPowerUp) count++;
                }

                powerupCountText.text = count > 0 ? count.ToString() : "";
            }
            else
            {
                powerupCountText.text = "";
            }
        }
    }
}