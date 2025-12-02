using UnityEngine;

[CreateAssetMenu(fileName = "RandomWaypoint", menuName = "Game/Enemy/Movement/Random Waypoint")]
public class RandomWaypointMovementSO : EnemyMovementSO
{
    [SerializeField] private Vector2 minViewport = new Vector2(0.1f, 0.5f);
    [SerializeField] private Vector2 maxViewport = new Vector2(0.9f, 0.9f);
    [SerializeField] private float reachThreshold = 0.5f;

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref object runtimeState)
    {
        if (Camera.main == null) return currentPos;

        Vector3? targetPos = runtimeState as Vector3?;

        if (!targetPos.HasValue)
        {
            targetPos = PickRandomPoint();
            runtimeState = targetPos; // Update state
        }

        if (Vector3.Distance(currentPos, targetPos.Value) < reachThreshold)
        {
            targetPos = PickRandomPoint();
            runtimeState = targetPos; // Update state
        }

        return Vector3.MoveTowards(currentPos, targetPos.Value, speed * Time.fixedDeltaTime);
    }

    private Vector3 PickRandomPoint()
    {
        float x = Random.Range(minViewport.x, maxViewport.x);
        float y = Random.Range(minViewport.y, maxViewport.y);
        Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(x, y, Camera.main.nearClipPlane + 10));
        pos.z = 0f;
        return pos;
    }
}