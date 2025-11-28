using UnityEngine;

/// <summary>
/// Strategy for calculating enemy rotation updates.
/// </summary>
public abstract class EnemyRotationSO : ScriptableObject
{
    public abstract Quaternion CalculateRotation(Transform self, IActor target);
}