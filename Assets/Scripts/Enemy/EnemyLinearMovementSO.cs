using UnityEngine;

/// <summary>
/// Moves the enemy in a straight line based on a direction vector.
/// Renamed to avoid conflict with Loot system movement.
/// </summary>
[CreateAssetMenu(fileName = "EnemyLinearMovement", menuName = "Game/Enemy/Movement/Linear")]
public class EnemyLinearMovementSO : EnemyMovementSO
{
    [Tooltip("Direction vector (normalized). Default is Down (0, -1, 0).")]
    [SerializeField] private Vector3 direction = Vector3.down;

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed)
    {
        // Simple displacement: Position + (Direction * Speed * DeltaTime)
        // Note: The speed passed here is typically (MoveSpeed * Time.deltaTime) from the Enemy script
        return currentPos + (direction.normalized * speed * Time.fixedDeltaTime);
    }
}