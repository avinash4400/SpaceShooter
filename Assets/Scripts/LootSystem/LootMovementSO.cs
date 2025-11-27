using UnityEngine;

/// <summary>
/// Abstract strategy for calculating the position of a loot item over time.
/// Allows defining different movement patterns (Linear, Curve, Sine Wave) as assets.
/// </summary>
public abstract class LootMovementSO : ScriptableObject
{
    /// <summary>
    /// Calculates the new position based on time elapsed.
    /// </summary>
    /// <param name="startPos">The position where the object spawned.</param>
    /// <param name="time">Time elapsed since spawn.</param>
    /// <param name="speed">The movement speed multiplier.</param>
    /// <returns>The calculated world position.</returns>
    public abstract Vector3 CalculatePosition(Vector3 startPos, float time, float speed);
}