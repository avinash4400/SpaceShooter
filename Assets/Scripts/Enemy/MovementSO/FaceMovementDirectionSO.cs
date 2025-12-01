using UnityEngine;

[CreateAssetMenu(fileName = "FaceMovement", menuName = "Game/Enemy/Rotation/Face Movement")]
public class FaceMovementDirectionSO : EnemyRotationSO
{
    [SerializeField] private float angleOffset = -90f;

    public override Quaternion CalculateRotation(Transform self, IActor target)
    {
        // We need the velocity/movement vector. 
        // Since the strategy signature doesn't pass velocity, we can infer it or rely on IActor.
        // But 'self' is a Transform. 
        // Let's assume the Enemy script sets its rotation, this strategy just calculates target rotation.

        // ISSUE: Calculating rotation based on movement requires knowing the movement vector.
        // OPTIMIZATION: Check if 'self' has an IActor component to get velocity?

        IActor enemyActor = self.GetComponent<IActor>();
        if (enemyActor != null)
        {
            Vector2 velocity = enemyActor.GetCurrentVelocity();
            if (velocity.sqrMagnitude > 0.01f)
            {
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + angleOffset;
                return Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        return self.rotation;
    }
}