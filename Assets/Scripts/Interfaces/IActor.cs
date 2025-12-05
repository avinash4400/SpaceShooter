using UnityEngine;

/// <summary>
/// Interface representing any major entity (actor) in the game world.
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

    Rigidbody GetRigidbody();
    T GetAttachedComponent<T>() where T : IGameComponent;
}