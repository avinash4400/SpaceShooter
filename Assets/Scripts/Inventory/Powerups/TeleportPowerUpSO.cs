using UnityEngine;

/// <summary>
/// Concrete strategy for the Teleport Power-Up.
/// </summary>
[CreateAssetMenu(fileName = "TeleportEffect", menuName = "Game/Effects/Teleport")]
public class TeleportPowerUpSO : PowerUpEffectSO
{
    [Header("Controller Configuration")]
    [Tooltip("The Controller Prefab that handles the logic.")]
    [SerializeField] private TeleportController controllerPrefab;

    public override void Apply(IActor target)
    {
        if (controllerPrefab == null || target == null)
        {
            Debug.LogError("[TeleportPowerUpSO] Missing Controller Prefab or Target!");
            return;
        }

        Transform targetTransform = target.GetRigidbody().transform.parent;

        TeleportController controller = default;

        if (controller == null)
        {
            controller = Instantiate(controllerPrefab, targetTransform.position, Quaternion.identity);
            controller.transform.SetParent(targetTransform);
            controller.transform.localPosition = Vector3.zero;

            SetLayerRecursively(controller.gameObject, targetTransform.gameObject.layer);

            controller.Initialize(target);
        }
    }

    public override void Remove(IActor target)
    {
        if (target == null) return;

        Transform targetTransform = target.GetRigidbody().transform;
        TeleportController controller = targetTransform.GetComponentInChildren<TeleportController>();

        if (controller != null)
        {
            controller.Cleanup();
        }
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