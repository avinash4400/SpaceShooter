using UnityEngine;

[CreateAssetMenu(fileName = "EvasiveMovement", menuName = "Game/Enemy/Movement/Evasive")]
public class EvasiveMovementSO : EnemyMovementSO
{
    [SerializeField] private Vector3 baseDirection = Vector3.down;
    [SerializeField] private float detectionRadius = 3.0f;
    [SerializeField] private float evasionStrength = 5.0f;
    [SerializeField] private LayerMask dangerLayers;

    private static readonly Collider[] hitBuffer = new Collider[5];

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref Vector3? storedPosition)
    {
        Vector3 moveStep = baseDirection.normalized * speed * Time.fixedDeltaTime;
        Vector3 avoidanceVector = Vector3.zero;
        int hitCount = Physics.OverlapSphereNonAlloc(currentPos, detectionRadius, hitBuffer, dangerLayers);

        if (hitCount > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                Collider threat = hitBuffer[i];
                if (threat == null) continue;

                Vector3 threatPos = threat.transform.position;
                Vector3 awayFromThreat = currentPos - threatPos;

                float dist = awayFromThreat.magnitude;
                float weight = 1f / (dist * dist + 0.1f);

                avoidanceVector += awayFromThreat.normalized * weight;
            }
            avoidanceVector = avoidanceVector.normalized * evasionStrength * Time.fixedDeltaTime;
            avoidanceVector.z = 0f;
        }

        return currentPos + moveStep + avoidanceVector;
    }
}