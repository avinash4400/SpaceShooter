using UnityEngine;

/// <summary>
/// Handles collision damage (Crashing).
/// Deals damage to the target (Player/Shield) and massive damage to self (Crash Death).
/// </summary>
public class CollisionDamageComponent : MonoBehaviour, IGameComponent
{
    [Header("Settings")]
    [SerializeField] private int damageToTarget = 1;
    [SerializeField] private bool killSelfOnCollision = true;
    [SerializeField] private LayerMask targetLayers;

    private IActor selfActor;

    // --- IGameComponent Implementation ---
    public void Initialize(IActor actor)
    {
        this.selfActor = actor;
    }

    /// <summary>
    /// Configures the component settings dynamically.
    /// </summary>
    public void Configure(LayerMask targets, int damage, bool killSelf)
    {
        this.targetLayers = targets;
        this.damageToTarget = damage;
        this.killSelfOnCollision = killSelf;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. Check if the object we hit is in our target layer mask
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            // 2. Try to damage the target (Player or Shield)
            IDamageHandler targetHandler = other.GetComponentInParent<IDamageHandler>();

            if (targetHandler != null)
            {
                // Create damage info attributing the source to 'us'
                DamageInfo info = new DamageInfo(damageToTarget, selfActor);
                targetHandler.HandleDamage(info);

            }

            // 3. Crash Logic (Damage Self)
            if (killSelfOnCollision)
            {
                HandleSelfDestruct(other);
            }
        }
    }

    private void HandleSelfDestruct(Collider target)
    {
        if (selfActor == null)
        {
            Debug.LogWarning($"[CollisionDamage] {name} has no actor reference, destroying on crash.");
            Destroy(gameObject);
            return;
        }

        // Use the injected actor reference to find the health component.
        // We request HealthComponent because GetAttachedComponent requires IGameComponent,
        // and HealthComponent is the specific class that implements both.
        IDamageHandler selfHealth = selfActor.GetAttachedComponent<HealthComponent>();

        if (selfHealth != null)
        {
            // We attribute the crash damage to the thing we hit (so it counts as a kill for the player if they shielded)
            IActor targetActor = target.GetComponentInParent<IActor>();
            Debug.Log($"[CollisionDamage] {name} is self-destructing due to crash with {target.name}.");
            // Deal massive damage to ensure death
            DamageInfo crashInfo = new DamageInfo(9999, targetActor);
            selfHealth.HandleDamage(crashInfo);
        }
        else
        {
            // Fallback if no health component (just destroy)
            Debug.LogWarning($"[CollisionDamage] {name} has no HealthComponent, destroying on crash.");
            Destroy(gameObject);
        }
    }
}