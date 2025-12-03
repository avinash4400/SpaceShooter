using UnityEngine;

/// <summary>
/// Spawns objects randomly from the Left, Top, or Right edges of the viewport.
/// Excludes the Bottom edge. Useful for ambushes or surrounding waves.
/// </summary>
[CreateAssetMenu(fileName = "ThreeSideSpawn", menuName = "Game/Loot/Strategies/Three Side Spawn")]
public class ThreeSideSpawnStrategySO : SpawnStrategySO
{
    [Header("Settings")]
    [Tooltip("Padding from the absolute edge of the viewport. e.g. 0.1 means -0.1 or 1.1")]
    [SerializeField] private float padding = 0.1f;

    public override Vector3 CalculateSpawnPosition(Transform sourceTransform)
    {
        Camera cam = Camera.main;
        if (cam == null) return Vector3.zero;

        // 1. Pick a Side (0 = Left, 1 = Top, 2 = Right)
        int side = Random.Range(0, 3);

        Vector3 viewportPos = Vector3.zero;

        switch (side)
        {
            case 0: // Left Edge
                // X is fixed off-screen left, Y is random (0 to 1)
                viewportPos = new Vector3(0f - padding, Random.value, cam.nearClipPlane + 10f);
                break;

            case 1: // Top Edge
                // X is random (0 to 1), Y is fixed off-screen top
                viewportPos = new Vector3(Random.value, 1f + padding, cam.nearClipPlane + 10f);
                break;

            case 2: // Right Edge
                // X is fixed off-screen right, Y is random (0 to 1)
                viewportPos = new Vector3(1f + padding, Random.value, cam.nearClipPlane + 10f);
                break;
        }

        // 2. Convert to World Space
        Vector3 worldPos = cam.ViewportToWorldPoint(viewportPos);

        // 3. Force Z to 0 plane
        worldPos.z = 0f;

        return worldPos;
    }
}