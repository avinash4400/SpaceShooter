using UnityEngine;

/// <summary>
/// Strategy for determining the spawn position of loot.
/// </summary>
public abstract class SpawnStrategySO : ScriptableObject
{
    public abstract Vector3 CalculateSpawnPosition(Transform sourceTransform);
}