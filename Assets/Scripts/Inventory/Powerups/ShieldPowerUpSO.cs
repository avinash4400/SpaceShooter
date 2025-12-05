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

        ShieldController existingShield = targetTransform.GetComponentInChildren<ShieldController>();
        if (existingShield != null)
        {
            Destroy(existingShield.gameObject);
        }

        ShieldController shieldInstance = Instantiate(shieldPrefab, targetTransform.position, Quaternion.identity);
        shieldInstance.transform.SetParent(targetTransform);
        shieldInstance.transform.localPosition = Vector3.zero;

        SetLayerRecursively(shieldInstance.gameObject, targetTransform.gameObject.layer);

        shieldInstance.Initialize(duration, shieldHealth);

    }

    public override void Remove(IActor target)
    {
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