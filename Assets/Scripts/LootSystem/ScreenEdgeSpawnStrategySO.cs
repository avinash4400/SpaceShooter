using UnityEngine;

/// <summary>
/// Spawns the loot at the top of the screen with a random X position.
/// </summary>
[CreateAssetMenu(fileName = "ScreenTopStrategy", menuName = "Game/Loot/Strategies/Screen Top")]
public class ScreenEdgeSpawnStrategySO : SpawnStrategySO
{
    [SerializeField] private float padding = 0.1f;
    [SerializeField] private float topYViewport = 1.1f;

    public override Vector3 CalculateSpawnPosition(Transform sourceTransform)
    {
        Camera cam = Camera.main;
        if (cam == null) return Vector3.zero;

        float randomX = Random.Range(padding, 1f - padding);
        // Z distance for ViewportToWorldPoint only affects how far 'in' it is, we reset it anyway
        Vector3 viewportPos = new Vector3(randomX, topYViewport, cam.nearClipPlane + 10f);

        Vector3 worldPos = cam.ViewportToWorldPoint(viewportPos);

        // Force Z to 0 plane
        worldPos.z = 0f;

        return worldPos;
    }
}