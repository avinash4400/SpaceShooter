using UnityEngine;

/// <summary>
/// Forces the enemy to face a specific global direction.
/// Useful for standard enemies that should always face "Down" (towards player).
/// </summary>
[CreateAssetMenu(fileName = "FixedRotation", menuName = "Game/Enemy/Rotation/Fixed Direction")]
public class FixedRotationSO : EnemyRotationSO
{
    [Tooltip("The angle to face (0=Right, 90=Up, 180=Left, -90=Down).")]
    [SerializeField] private float fixedAngle = -90f;

    public override Quaternion CalculateRotation(Transform self, IActor target)
    {
        return Quaternion.AngleAxis(fixedAngle, Vector3.forward);
    }
}