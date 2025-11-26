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

    // --- Events for Decoupled Communication (INSTANCE EVENTS) ---
    // The GameObject parameter tells subscribers *which* entity was affected.
    public event Action<GameObject, int> OnHealthChanged; // (AffectedObject, CurrentHP)
    public event Action<GameObject> OnHit; // (AffectedObject) - Used for hit flash/SFX
    public event Action<GameObject> OnDeath; // (AffectedObject) - Used by Player.cs or EnemySpawner.cs

    // --- IGameComponent Implementation ---

    /// <summary>
    /// Initializes the component with a reference to its owning Actor.
    /// Although HealthComponent doesn't currently need the IActor reference, 
    /// this method is required to participate in the initialization contract.
    /// </summary>
    /// <param name="actor">The IActor interface of the owning entity.</param>
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
    /// Required implementation of IDamageHandler. Processes incoming damage.
    /// This method is called when this object is hit by an IDamageSource (like a bullet).
    /// </summary>
    /// <param name="info">Damage data including amount and source.</param>
    public void HandleDamage(DamageInfo info)
    {
        if (!disableInvulnerability && isInvulnerable)
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
        gameObject.SetActive(false); // Simple removal
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