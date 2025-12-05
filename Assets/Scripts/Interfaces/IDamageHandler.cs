using UnityEngine;

/// <summary>
/// Interface for any object that can receive and process damage.
/// Player, Enemies, and destructible environment elements will implement this.
/// </summary>
public interface IDamageHandler
{
    /// <summary>
    /// Applies damage to the object.
    /// </summary>
    /// <param name="info">A struct containing damage data (amount, source, etc.).</param>
    void HandleDamage(DamageInfo info);
}