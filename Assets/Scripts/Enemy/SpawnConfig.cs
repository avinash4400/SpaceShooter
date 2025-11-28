using UnityEngine;

/// <summary>
/// Container for configuration data used by SpawnPatterns.
/// Allows splitting Data (this class) from Logic (SpawnPatternSO).
/// </summary>
[System.Serializable]
public class SpawnConfig
{
    [System.Serializable]
    public struct EnemyPoolEntry
    {
        public Enemy prefab;
        public EnemyDataSO config;
    }

    [Header("Single Enemy Settings")]
    public Enemy enemyPrefab;
    public EnemyDataSO enemyConfig;
    public SpawnStrategySO spawnStrategy;

    [Header("Pool Settings (For Random Patterns)")]
    [Tooltip("Used by patterns that pick random enemies (e.g. DurationPattern).")]
    public EnemyPoolEntry[] enemyPool;

    [Header("Flow Control")]
    [Tooltip("Number of enemies to spawn (Sequence/Elimination).")]
    public int count = 1;

    [Tooltip("Time between spawns.")]
    public float interval = 1f;

    [Tooltip("Total length of the pattern (DurationPattern).")]
    public float duration = 10f;

    [Tooltip("Delay before this specific pattern starts.")]
    public float initialDelay = 0f;
}