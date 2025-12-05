using UnityEngine;

/// <summary>
/// Interface for any object that can be picked up by an Actor.
/// </summary>
public interface IPickup
{
    /// <summary>
    /// Attempts to collect the item.
    /// </summary>
    /// <param name="target">The actor attempting the collection.</param>
    /// <returns>True if collection was successful (and item should be destroyed).</returns>
    bool Collect(IActor target);
}