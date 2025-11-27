using UnityEngine;

/// <summary>
/// Spawns the loot at the top of the screen with a random X position.
/// Good for Supply Drops.
/// </summary>
[CreateAssetMenu(fileName = "ScreenTopStrategy", menuName = "Game/Loot/Strategies/Screen Top")]
public class ScreenEdgeSpawnStrategySO : SpawnStrategySO
{
    [Header("Settings")]
    [Tooltip("Padding from the absolute edge of the viewport (0-1).")]
    [SerializeField] private float padding = 0.1f;
    [SerializeField] private float topYViewport = 1.1f; // Just above screen

    public override Vector3 CalculateSpawnPosition(Transform sourceTransform)
    {
        Camera cam = Camera.main;
        if (cam == null) return Vector3.zero;

        // Random X between left and right padding
        float randomX = Random.Range(padding, 1f - padding);
        Vector3 viewportPos = new Vector3(randomX, topYViewport, cam.nearClipPlane + 10f);

        return cam.ViewportToWorldPoint(viewportPos);
    }
}