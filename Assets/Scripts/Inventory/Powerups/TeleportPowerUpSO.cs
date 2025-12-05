using UnityEngine;

/// <summary>
/// Concrete strategy for the Teleport Power-Up.
/// Spawns/Retrieves a TeleportController on the target and delegates logic to it.
/// </summary>
[CreateAssetMenu(fileName = "TeleportEffect", menuName = "Game/Effects/Teleport")]
public class TeleportPowerUpSO : PowerUpEffectSO
{
    [Header("Controller Configuration")]
    [Tooltip("The Controller Prefab that handles the logic.")]
    [SerializeField] private TeleportController controllerPrefab;

    public override void Apply(IActor target)
    {
        // 1. Validation (Matching ShieldSO style)
        if (controllerPrefab == null || target == null)
        {
            Debug.LogError("[TeleportPowerUpSO] Missing Controller Prefab or Target!");
            return;
        }

        Transform targetTransform = target.GetRigidbody().transform.parent;

        // 2. Get or Spawn the Controller
        TeleportController controller = default;

        if (controller == null)
        {
            // Spawn new controller attached to the actor
            controller = Instantiate(controllerPrefab, targetTransform.position, Quaternion.identity);
            controller.transform.SetParent(targetTransform);
            controller.transform.localPosition = Vector3.zero;

            // Set the layer to match the Player (Consistency with ShieldSO)
            // Useful if the controller has visual indicators attached
            SetLayerRecursively(controller.gameObject, targetTransform.gameObject.layer);

            // Initialize
            controller.Initialize(target);
        }
    }

    public override void Remove(IActor target)
    {
        // Cleanup if unequipped
        if (target == null) return;

        Transform targetTransform = target.GetRigidbody().transform;
        TeleportController controller = targetTransform.GetComponentInChildren<TeleportController>();

        if (controller != null)
        {
            controller.Cleanup();
        }
    }

    /// <summary>
    /// Helper to ensure the spawned object and all children match the actor's layer.
    /// </summary>
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}