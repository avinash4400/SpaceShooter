using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// A reusable component for any entity (Player, Enemy, Boss) that has health and can take damage.
/// </summary>
public class HealthComponent : MonoBehaviour, IDamageHandler, IGameComponent
{
    [Header("Health Settings")]
    [Tooltip("The maximum and starting health of this entity.")]
    [SerializeField] private int maxHealth = 3;
    [Tooltip("Duration of invulnerability frames after taking damage.")]
    [SerializeField] private float invulnerabilityDuration = 0.5f;
    [Tooltip("If true, this component does not enter an invulnerable state after being hit (e.g., for enemies).")]
    [SerializeField] private bool disableInvulnerability = false;

    private int currentHealth;
    private bool isInvulnerable = false;

    private bool isExternalInvulnerable = false;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    public event Action<GameObject, int> OnHealthChanged;
    public event Action<GameObject, int> OnHeal; 
    public event Action<GameObject> OnHit; 
    public event Action<GameObject> OnDeath;

    // --- IGameComponent Implementation ---

    public void Initialize(IActor actor)
    {
        
    }


    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        OnHealthChanged?.Invoke(gameObject, currentHealth);
    }

    /// <summary>
    /// Allows other components (like Dash) to toggle invulnerability.
    /// </summary>
    public void SetExternalInvulnerability(bool state)
    {
        isExternalInvulnerable = state;
    }

    /// <summary>
    /// Required implementation of IDamageHandler. Processes incoming damage.
    /// This method is called when this object is hit by an IDamageSource (like a bullet).
    /// </summary>
    /// <param name="info">Damage data including amount and source.</param>
    public void HandleDamage(DamageInfo info)
    {
        if (isExternalInvulnerable || (!disableInvulnerability && isInvulnerable))
        {
            return;
        }

        currentHealth -= info.DamageAmount;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHit?.Invoke(gameObject);

        OnHealthChanged?.Invoke(gameObject, currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (!disableInvulnerability)
        {
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    /// <summary>
    /// Restores health to the entity.
    /// </summary>
    /// <param name="amount">Amount to heal.</param>
    public void Heal(int amount)
    {
        if (currentHealth >= maxHealth) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        OnHealthChanged?.Invoke(gameObject, currentHealth);
        OnHeal?.Invoke(gameObject, currentHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke(gameObject);
        Debug.Log($"{gameObject.name} Died. Notifying systems.");
    }

    /// <summary>
    /// Coroutine to grant temporary invulnerability after a hit.
    /// </summary>
    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    /// <summary>
    /// Resets the entity's health to max. Useful for pooling or stage restarts.
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isInvulnerable = false;
        OnHealthChanged?.Invoke(gameObject, currentHealth);
    }
}