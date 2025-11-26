using UnityEngine;

/// <summary>
/// Interface representing any major entity (actor) in the game world.
/// Provides essential, common data points like the Transform and movement state.
/// Player and all Enemy classes will implement this interface.
/// </summary>
public interface IActor
{
    /// <summary>
    /// Gets the Transform component of the Actor.
    /// </summary>
    Transform GetTransform();

    /// <summary>
    /// Gets the current calculated movement velocity of the Actor.
    /// </summary>
    Vector2 GetCurrentVelocity();

    /// <summary>
    /// Sets the current calculated movement velocity of the Actor.
    /// Used by the movement component to share velocity data.
    /// </summary>
    void SetCurrentVelocity(Vector2 velocity);
}