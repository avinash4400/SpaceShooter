using System;

/// <summary>
/// Interface for any component that needs to temporarily block or take over 
/// features like Dash, Stun, or Time Slow effects.
/// </summary>
public interface IMovementBlocker
{
    /// <summary>
    /// Event fired when the component starts blocking/controlling movement.
    /// </summary>
    event Action OnMovementBlockStart;

    /// <summary>
    /// Event fired when the component stops blocking/controlling movement.
    /// </summary>
    event Action OnMovementBlockEnd;
}