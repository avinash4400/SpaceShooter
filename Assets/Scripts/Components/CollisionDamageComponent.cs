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
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            IDamageHandler targetHandler = other.GetComponentInParent<IDamageHandler>();

            if (targetHandler != null)
            {
                DamageInfo info = new DamageInfo(damageToTarget, selfActor);
                targetHandler.HandleDamage(info);

            }

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
            Destroy(gameObject);
            return;
        }

        IDamageHandler selfHealth = selfActor.GetAttachedComponent<HealthComponent>();

        if (selfHealth != null)
        {
            IActor targetActor = target.GetComponentInParent<IActor>();
            DamageInfo crashInfo = new DamageInfo(9999, targetActor);
            selfHealth.HandleDamage(crashInfo);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}