using UnityEngine;

[CreateAssetMenu(fileName = "FaceMovement", menuName = "Game/Enemy/Rotation/Face Movement")]
public class FaceMovementDirectionSO : EnemyRotationSO
{
    [SerializeField] private float angleOffset = -90f;

    public override Quaternion CalculateRotation(Transform self, IActor target)
    {
        IActor enemyActor = self.GetComponentInParent<IActor>();
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