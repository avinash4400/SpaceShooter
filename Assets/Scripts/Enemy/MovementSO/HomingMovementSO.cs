using UnityEngine;

[CreateAssetMenu(fileName = "HomingMovement", menuName = "Game/Enemy/Movement/Homing")]
public class HomingMovementSO : EnemyMovementSO
{
    [SerializeField] private bool stopAtTarget = false;
    [SerializeField] private float stoppingDistance = 0.5f;

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref object runtimeState)
    {
        if (target == null) return currentPos + (Vector3.down * speed * Time.fixedDeltaTime);

        Vector3 targetPos = target.GetTransform().position;
        targetPos.z = 0f;

        Vector3 direction = (targetPos - currentPos);
        float distance = direction.magnitude;

        if (stopAtTarget && distance < stoppingDistance) return currentPos;

        direction.Normalize();
        return currentPos + (direction * speed * Time.fixedDeltaTime);
    }
}