using UnityEngine;

/// <summary>
/// Spawns the loot exactly where the source object (e.g., Enemy) is.
/// </summary>
[CreateAssetMenu(fileName = "AtSourceStrategy", menuName = "Game/Loot/Strategies/At Source Position")]
public class AtSourcePositionStrategySO : SpawnStrategySO
{
    public override Vector3 CalculateSpawnPosition(Transform sourceTransform)
    {
        if (sourceTransform == null) return Vector3.zero;

        Vector3 pos = sourceTransform.position;
        pos.z = 0f; // Force 2D plane

        return pos;
    }
}