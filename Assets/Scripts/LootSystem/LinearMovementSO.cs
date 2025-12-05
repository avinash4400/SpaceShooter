using UnityEngine;

/// <summary>
/// Moves the object in a straight line direction.
/// </summary>
[CreateAssetMenu(fileName = "LinearMovement", menuName = "Game/Loot/Movement/Linear")]
public class LinearMovementSO : LootMovementSO
{
    [Tooltip("Direction of travel (normalized). Default is Down.")]
    [SerializeField] private Vector3 direction = Vector3.down;

    public override Vector3 CalculatePosition(Vector3 startPos, float time, float speed)
    {
        return startPos + (direction.normalized * speed * time);
    }
}