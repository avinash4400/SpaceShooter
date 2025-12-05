using UnityEngine;

/// <summary>
/// Spawns objects randomly from the Left, Top, or Right edges of the viewport.
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

        int side = Random.Range(0, 3);

        Vector3 viewportPos = Vector3.zero;

        switch (side)
        {
            case 0: // Left Edge
                viewportPos = new Vector3(0f - padding, Random.value, cam.nearClipPlane + 10f);
                break;

            case 1: // Top Edge
                viewportPos = new Vector3(Random.value, 1f + padding, cam.nearClipPlane + 10f);
                break;

            case 2: // Right Edge
                viewportPos = new Vector3(1f + padding, Random.value, cam.nearClipPlane + 10f);
                break;
        }

        Vector3 worldPos = cam.ViewportToWorldPoint(viewportPos);

        worldPos.z = 0f;

        return worldPos;
    }
}