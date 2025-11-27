using UnityEngine;

/// <summary>
/// Concrete strategy for the Shield Power-Up.
/// Spawns a shield prefab and initializes its controller with specific stats.
/// </summary>
[CreateAssetMenu(fileName = "ShieldEffect", menuName = "Game/Effects/Shield")]
public class ShieldPowerUpSO : PowerUpEffectSO
{
    [Header("Shield Configuration")]
    [Tooltip("The Shield Prefab (must have ShieldController attached).")]
    [SerializeField] private ShieldController shieldPrefab;

    [Tooltip("How much damage the shield can absorb before breaking.")]
    [SerializeField] private int shieldHealth = 3;

    public override void Apply(IActor target)
    {
        if (shieldPrefab == null || target == null)
        {
            Debug.LogError("[ShieldPowerUpSO] Missing Prefab or Target!");
            return;
        }

        Transform targetTransform = target.GetTransform();

        // Check if a shield already exists to prevent stacking
        ShieldController existingShield = targetTransform.GetComponentInChildren<ShieldController>();
        if (existingShield != null)
        {
            // Option A: Refresh existing shield
            // existingShield.Initialize(duration, shieldHealth);
            // return;

            // Option B: Destroy old and replace (Simpler visual reset)
            Destroy(existingShield.gameObject);
        }

        // Spawn the shield attached to the actor
        // Since shieldPrefab is of type ShieldController, Instantiate returns the component directly.
        ShieldController shieldInstance = Instantiate(shieldPrefab, targetTransform.position, Quaternion.identity);
        shieldInstance.transform.SetParent(targetTransform);
        shieldInstance.transform.localPosition = Vector3.zero; // Center on player

        // Initialize the logic component
        // No need to use GetComponent since we instantiated the specific component type.
        shieldInstance.Initialize(duration, shieldHealth);

        Debug.Log($"[ShieldPowerUp] Activated on {targetTransform.name}");
    }

    public override void Remove(IActor target)
    {
        // Cleanup is handled by the ShieldController's own duration timer.
        // However, if we forced removal (e.g. death), we could look for the component here.
    }
}