using UnityEngine;

/// <summary>
/// Spawns objects at the Left (0) or Right (1) edge of the viewport.
/// </summary>
[CreateAssetMenu(fileName = "RandomSideSpawn", menuName = "Game/Loot/Strategies/Random Side")]
public class RandomSideSpawnStrategySO : SpawnStrategySO
{
    [Header("Vertical Range (Viewport 0-1)")]
    [SerializeField] private float minY = 0.2f;
    [SerializeField] private float maxY = 0.8f;

    [Header("Offset")]
    [Tooltip("How far off-screen to spawn (in Viewport units). e.g. 0.1 means -0.1 or 1.1")]
    [SerializeField] private float horizontalPadding = 0.1f;

    public override Vector3 CalculateSpawnPosition(Transform sourceTransform)
    {
        Camera cam = Camera.main;
        if (cam == null) return Vector3.zero;

        bool isRight = Random.value > 0.5f;
        float spawnX = isRight ? (1f + horizontalPadding) : (0f - horizontalPadding);

        float spawnY = Random.Range(minY, maxY);

        Vector3 viewportPos = new Vector3(spawnX, spawnY, cam.nearClipPlane + 10f);
        Vector3 worldPos = cam.ViewportToWorldPoint(viewportPos);

        worldPos.z = 0f;

        return worldPos;
    }
}