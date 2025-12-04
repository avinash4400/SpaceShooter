using UnityEngine;
using System.Collections;

/// <summary>
/// Logic: Spawns nothing. Waits until all active enemies in the scene are cleared.
/// Useful for pacing (e.g. "Defeat all enemies before next wave").
/// </summary>
[CreateAssetMenu(fileName = "Logic_WaitForClear", menuName = "Game/Spawning/Patterns/Logic: Wait For Clear")]
public class WaitForClearPatternSO : SpawnPatternSO
{
    public override IEnumerator Execute(LevelManager manager, SpawnConfig config)
    {
        Debug.LogWarningFormat($"[WaitForClear] Waiting for screen clear...{0}", manager.ActiveEnemyCount);

        // Optimized polling: Uses the event-driven count from LevelManager
        while (manager.ActiveEnemyCount > 0)
        {
            // Check every 0.5 seconds to save performance
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("[WaitForClear] Screen cleared. Proceeding.");
    }
}