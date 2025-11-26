using UnityEngine;

/// <summary>
/// Interface for any object that can inflict damage.
/// Player bullets, Enemy bullets, and environmental hazards will implement this.
/// </summary>
public interface IDamageSource
{
    /// <summary>
    /// Gets the raw damage amount inflicted by this source.
    /// </summary>
    int DamageAmount { get; }

    /// <summary>
    /// Gets a reference to the GameObject that owns this damage source.
    /// Used for referencing the entity that caused the damage (e.g., the Player).
    /// </summary>
    GameObject SourceObject { get; }

    /// <summary>
    /// Creates a DamageInfo struct containing all necessary data about the damage event.
    /// </summary>
    DamageInfo CreateDamageInfo();
}