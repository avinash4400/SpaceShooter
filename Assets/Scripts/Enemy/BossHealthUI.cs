using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the Boss Health Bar.
/// Listens for the BossSpawned event to appear and link to the specific boss.
/// </summary>
public class BossHealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject container; 
    [SerializeField] private Image fillImage;

    private HealthComponent bossHealth;

    void Awake()
    {
        if (container != null) container.SetActive(false);
    }

    void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnBossSpawned += InitializeBossBar;
        }
    }

    void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnBossSpawned -= InitializeBossBar;
        }

        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged -= UpdateFill;
            bossHealth.OnDeath -= HandleBossDeath;
        }
    }

    private void InitializeBossBar(HealthComponent health)
    {
        bossHealth = health;

        if (container != null) container.SetActive(true);

        bossHealth.OnHealthChanged += UpdateFill;
        bossHealth.OnDeath += HandleBossDeath;

        UpdateFill(null, bossHealth.CurrentHealth);
    }

    private void UpdateFill(GameObject source, int currentHealth)
    {
        if (fillImage != null && bossHealth != null)
        {
            float fill = (float)currentHealth / bossHealth.MaxHealth;
            fillImage.fillAmount = fill;
        }
    }

    private void HandleBossDeath(GameObject deadObject)
    {
        if (container != null) container.SetActive(false);

        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged -= UpdateFill;
            bossHealth.OnDeath -= HandleBossDeath;
            bossHealth = null;
        }
    }
}