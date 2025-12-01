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

        Transform targetTransform = target.GetRigidbody().transform;

        // Check if a shield already exists to prevent stacking
        ShieldController existingShield = targetTransform.GetComponentInChildren<ShieldController>();
        if (existingShield != null)
        {
            Destroy(existingShield.gameObject);
        }

        // Spawn the shield attached to the actor
        ShieldController shieldInstance = Instantiate(shieldPrefab, targetTransform.position, Quaternion.identity);
        shieldInstance.transform.SetParent(targetTransform);
        shieldInstance.transform.localPosition = Vector3.zero;

        // CRITICAL FIX: Set the layer to match the Player so Enemy Bullets collide with it
        // Also ensure we set it recursively in case the shield has child visuals/colliders
        SetLayerRecursively(shieldInstance.gameObject, targetTransform.gameObject.layer);

        // Initialize the logic component
        shieldInstance.Initialize(duration, shieldHealth);

        Debug.Log($"[ShieldPowerUp] Activated on {targetTransform.name}");
    }

    public override void Remove(IActor target)
    {
        // Cleanup handled by ShieldController
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}