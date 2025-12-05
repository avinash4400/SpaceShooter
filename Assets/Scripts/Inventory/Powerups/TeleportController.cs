using UnityEngine;

/// <summary>
/// Handles the runtime logic for the Teleport ability.
/// Attached to the player when the power-up is used.
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
        if (activeBeacon != null)
        {
            PerformTeleport();
            return true; 
        }
        else
        {
            FireBeacon();
            return false; 
        }
    }

    private void FireBeacon()
    {
        if (beaconPrefab == null || owner == null) return;

        Transform ownerTransform = owner.GetRigidbody().transform;

        Vector3 spawnPos = ownerTransform.position + (ownerTransform.up * 0.5f);

        activeBeacon = Instantiate(beaconPrefab, spawnPos, Quaternion.identity);

        activeBeacon.Initialize(null, owner, ownerTransform.up, 1f, null);

    }

    private void PerformTeleport()
    {
        if (activeBeacon == null || owner == null) return;

        Transform ownerTransform = owner.GetRigidbody().transform;

        ownerTransform.position = activeBeacon.transform.position;


        Destroy(activeBeacon.gameObject);
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