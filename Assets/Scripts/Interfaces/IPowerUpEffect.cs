using UnityEngine;

/// <summary>
/// Strategy interface for power-up logic.
/// Defines the contract for applying and removing effects on an actor.
/// </summary>
public interface IPowerUpEffect
{
    /// <summary>
    /// Applies the power-up effect to the target actor.
    /// </summary>
    /// <param name="target">The actor using the power-up.</param>
    void Apply(IActor target);

    /// <summary>
    /// Removes the power-up effect from the target actor (used for timed effects).
    /// </summary>
    /// <param name="target">The actor to remove the effect from.</param>
    void Remove(IActor target);
}