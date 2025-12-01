using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the Player's Health Bar UI.
/// Simply listens to the global EventManager for updates.
/// </summary>
public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The image component representing the health bar fill.")]
    [SerializeField] private Image healthFillImage;

    void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnPlayerHealthChanged += UpdateFill;
        }
    }

    void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnPlayerHealthChanged -= UpdateFill;
        }
    }

    void Start()
    {
        if (EventManager.Instance == null)
        {
            Debug.LogError("[HealthUI] EventManager is missing!");
        }
    }

    /// <summary>
    /// Event callback to update the UI image.
    /// </summary>
    private void UpdateFill(int currentHealth, int maxHealth)
    {
        if (healthFillImage == null || maxHealth <= 0) return;

        // Calculate fill amount (0.0 to 1.0)
        float fillAmount = (float)currentHealth / maxHealth;
        healthFillImage.fillAmount = fillAmount;
    }
}