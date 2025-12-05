using UnityEngine;

/// <summary>
/// Handles the runtime logic for the Teleport ability.
/// Attached to the player when the power-up is used.
/// Manages the state of the active beacon and the teleport action.
/// </summary>
public class TeleportController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private TeleportProjectile beaconPrefab;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip teleportSound;

    private IActor owner;
    private TeleportProjectile activeBeacon;

    public void Initialize(IActor owner)
    {
        this.owner = owner;
        AttemptAction();
    }

    /// <summary>
    /// Attempts to perform the action based on current state.
    /// </summary>
    /// <returns>True if the action is complete (item consumed). False if pending (item kept).</returns>
    public bool AttemptAction()
    {
        // 1. Check if we have a valid active beacon
        // (Unity overrides == null, so this handles destroyed objects automatically)
        if (activeBeacon != null)
        {
            PerformTeleport();
            return true; // Action complete -> Consume Item
        }
        else
        {
            FireBeacon();
            return false; // Beacon in flight -> Keep Item
        }
    }

    private void FireBeacon()
    {
        if (beaconPrefab == null || owner == null) return;

        Transform ownerTransform = owner.GetRigidbody().transform;

        // Spawn slightly in front/up
        Vector3 spawnPos = ownerTransform.position + (ownerTransform.up * 0.5f);

        activeBeacon = Instantiate(beaconPrefab, spawnPos, Quaternion.identity);

        // Initialize projectile
        activeBeacon.Initialize(null, owner, ownerTransform.up, 1f, null);

        // Audio

        Debug.Log($"[TeleportController] Beacon launched.");
    }

    private void PerformTeleport()
    {
        if (activeBeacon == null || owner == null) return;

        Transform ownerTransform = owner.GetRigidbody().transform;

        // Move actor
        ownerTransform.position = activeBeacon.transform.position;

        // Audio
        if (teleportSound != null)
        {
            AudioSource.PlayClipAtPoint(teleportSound, ownerTransform.position);
        }

        Debug.Log($"[TeleportController] Teleported to beacon.");

        // Cleanup Beacon
        Destroy(activeBeacon.gameObject);

        // Cleanup Self (The sequence is done)
        // If you want the controller to persist for future uses, remove this line.
        // Destroy(gameObject); 
    }

    public void Cleanup()
    {
        if (activeBeacon != null)
        {
            Destroy(activeBeacon.gameObject);
        }
        Destroy(gameObject);
    }
}