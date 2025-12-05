using UnityEngine;

/// <summary>
/// Abstract base class for all physical pickup objects (PowerUps, Bullets, Coins).
/// </summary>
public abstract class BasePickup : MonoBehaviour, IPickup
{
    [Header("Base Settings")]
    [SerializeField] protected bool destroyOnPickup = true;

    [Header("Movement")]
    [Tooltip("Strategy for how this item moves through the world.")]
    [SerializeField] protected LootMovementSO movementStrategy;
    [SerializeField] protected float moveSpeed = 3f;

    private Vector3 startPos;
    private float timeAlive;

    private Camera mainCamera;

    protected virtual void Start()
    {
        startPos = transform.position;
        mainCamera = Camera.main;
    }

    protected virtual void Update()
    {
        if (movementStrategy != null)
        {
            timeAlive += Time.deltaTime;
            transform.position = movementStrategy.CalculatePosition(startPos, timeAlive, moveSpeed);
        }

        CheckOutOfBounds();
    }

    private void CheckOutOfBounds()
    {
        if (mainCamera == null) return;

        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        if (viewPos.y < -0.2f || viewPos.y > 1.2f || viewPos.x < -0.2f || viewPos.x > 1.2f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IActor actor = other.GetComponentInParent<IActor>();
        if (actor != null)
        {
            if (Collect(actor))
            {
                OnCollected();
            }
        }
    }

    public abstract bool Collect(IActor target);

    protected virtual void OnCollected()
    {

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}