using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the Player's Health Bar UI.
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

        float fillAmount = (float)currentHealth / maxHealth;
        healthFillImage.fillAmount = fillAmount;
    }
}