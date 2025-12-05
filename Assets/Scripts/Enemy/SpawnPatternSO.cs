using UnityEngine;
using System.Collections;

/// <summary>
/// Abstract strategy for spawning enemies.
/// Receives config at runtime from the Wave.
/// </summary>
public abstract class SpawnPatternSO : ScriptableObject
{
    /// <summary>
    /// Executes the spawn logic using the provided configuration.
    /// </summary>
    /// <param name="manager">Reference to the LevelManager.</param>
    /// <param name="config">Data defining counts, prefabs, and timing.</param>
    public abstract IEnumerator Execute(LevelManager manager, SpawnConfig config);
}