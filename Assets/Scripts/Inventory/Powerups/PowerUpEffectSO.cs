using UnityEngine;

/// <summary>
/// Abstract ScriptableObject implementation of the power-up strategy.
/// </summary>
public abstract class PowerUpEffectSO : ScriptableObject, IPowerUpEffect
{
    [Tooltip("Is this effect timed? If > 0, Inventory will call Remove() after this duration.")]
    public float duration = 0f;

    public abstract void Apply(IActor target);

    public virtual void Remove(IActor target)
    {
    }
}