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

                Debug.Log($"[CollisionDamage] {name} crashed into {other.name}!");
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
        // We find our own health component
        // Note: Using GetAttachedComponent if implemented, or GetComponent
        IDamageHandler selfHealth = GetComponent<IDamageHandler>();

        if (selfHealth != null)
        {
            // We attribute the crash damage to the thing we hit (so it counts as a kill for the player if they shielded)
            IActor targetActor = target.GetComponentInParent<IActor>();

            // Deal massive damage to ensure death
            DamageInfo crashInfo = new DamageInfo(9999, targetActor);
            selfHealth.HandleDamage(crashInfo);
        }
        else
        {
            // Fallback if no health component (just destroy)
            Destroy(gameObject);
        }
    }
}