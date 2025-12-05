using UnityEngine;

/// <summary>
/// A structure to pass immutable data about a damage event between systems.
/// </summary>
public struct DamageInfo
{
    public readonly int DamageAmount;
    public readonly IActor Source;

    public DamageInfo(int amount, IActor source)
    {
        DamageAmount = amount;
        Source = source;
    }
}