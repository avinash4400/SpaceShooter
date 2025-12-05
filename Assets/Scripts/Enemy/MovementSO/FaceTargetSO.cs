using UnityEngine;

[CreateAssetMenu(fileName = "FaceTarget", menuName = "Game/Enemy/Rotation/Face Target")]
public class FaceTargetSO : EnemyRotationSO
{
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float angleOffset = -90f; 

    public override Quaternion CalculateRotation(Transform self, IActor target)
    {
        if (target == null) return self.rotation;

        Vector3 direction = (target.GetTransform().position - self.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset;

        Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);

        return Quaternion.RotateTowards(self.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}