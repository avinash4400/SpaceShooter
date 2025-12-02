using UnityEngine;

[CreateAssetMenu(fileName = "EnemyLinearMovement", menuName = "Game/Enemy/Movement/Linear")]
public class EnemyLinearMovementSO : EnemyMovementSO
{
    [SerializeField] private Vector3 direction = Vector3.down;

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref object runtimeState)
    {
        return currentPos + (direction.normalized * speed * Time.fixedDeltaTime);
    }
}