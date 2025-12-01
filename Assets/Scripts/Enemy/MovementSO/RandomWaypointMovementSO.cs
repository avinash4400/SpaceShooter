using UnityEngine;

[CreateAssetMenu(fileName = "RandomWaypoint", menuName = "Game/Enemy/Movement/Random Waypoint")]
public class RandomWaypointMovementSO : EnemyMovementSO
{
    [Header("Arena Viewport Bounds")]
    [SerializeField] private Vector2 minViewport = new Vector2(0.1f, 0.5f);
    [SerializeField] private Vector2 maxViewport = new Vector2(0.9f, 0.9f);
    [SerializeField] private float reachThreshold = 0.5f;

    public override Vector3 CalculateMovement(Vector3 currentPos, IActor target, float timeAlive, float speed, ref Vector3? storedPosition)
    {
        if (Camera.main == null) return currentPos;

        // Initialize target if null
        if (!storedPosition.HasValue)
        {
            storedPosition = PickRandomPoint();
        }

        // Check distance
        if (Vector3.Distance(currentPos, storedPosition.Value) < reachThreshold)
        {
            storedPosition = PickRandomPoint();
        }

        // Move
        return Vector3.MoveTowards(currentPos, storedPosition.Value, speed * Time.fixedDeltaTime);
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