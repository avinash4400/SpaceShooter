using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// A reusable component for any entity (Player, Enemy, Boss) that has health and can take damage.
/// Implements IDamageHandler to process damage from any source (IDamageSource).
/// Events are instance-based, allowing a dedicated Manager (e.g., Player.cs) or Spawner to listen.
/// Implements IGameComponent to participate in the Actor's initialization dependency injection.
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

    // New flag for external systems (like Dash) to control invulnerability
    private bool isExternalInvulnerable = false;

    // --- Public Accessors for UI ---
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    // --- Events for Decoupled Communication (INSTANCE EVENTS) ---
    // The GameObject parameter tells subscribers *which* entity was affected.
    public event Action<GameObject, int> OnHealthChanged; // (AffectedObject, CurrentHP)
    public event Action<GameObject> OnHit; // (AffectedObject) - Used for hit flash/SFX
    public event Action<GameObject> OnDeath; // (AffectedObject) - Used by Player.cs or EnemySpawner.cs

    // --- IGameComponent Implementation ---

    public void Initialize(IActor actor)
    {
        // Currently, no initialization logic is needed here, but the contract is fulfilled.
    }

    // --- Standard MonoBehaviour Methods ---

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        // Initial health broadcast
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
        // Check both internal (post-hit) and external (dash/powerup) invulnerability
        if (isExternalInvulnerable || (!disableInvulnerability && isInvulnerable))
        {
            return;
        }

        currentHealth -= info.DamageAmount;
        currentHealth = Mathf.Max(0, currentHealth); // Ensure health doesn't go negative

        // Broadcast the hit event (instance)
        OnHit?.Invoke(gameObject);
        Debug.Log($"{gameObject.name} took {info.DamageAmount} damage. HP remaining: {currentHealth}");

        // Broadcast the change (instance)
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

    private void Die()
    {
        // Broadcast the death event (instance)
        OnDeath?.Invoke(gameObject);
        Debug.Log($"{gameObject.name} Died. Notifying systems.");

        // REMOVED: gameObject.SetActive(false); 
        // We now rely on the listener (Enemy.cs, Player.cs) to handle the destruction/disabling
        // to allow for death sequences (VFX/Animation) to play out first.
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