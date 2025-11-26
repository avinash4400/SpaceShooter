using UnityEngine;

/// <summary>
/// A structure to pass immutable data about a damage event between systems.
/// </summary>
public struct DamageInfo
{
    public readonly int DamageAmount;
    public readonly GameObject Source;
    // We can expand this later with: public readonly DamageType Type;

    public DamageInfo(int amount, GameObject source)
    {
        DamageAmount = amount;
        Source = source;
    }
}