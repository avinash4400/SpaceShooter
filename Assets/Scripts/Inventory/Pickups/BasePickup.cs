using UnityEngine;

/// <summary>
/// Abstract base class for all physical pickup objects (PowerUps, Bullets, Coins).
/// Implements IPickup to enforce the collection contract.
/// Inherits from MonoBehaviour so it can be dragged into LootItemSO as a prefab.
/// </summary>
public abstract class BasePickup : MonoBehaviour, IPickup
{
    [Header("Base Settings")]
    [SerializeField] protected bool destroyOnPickup = true;

    [Header("Movement")]
    [Tooltip("Strategy for how this item moves through the world.")]
    [SerializeField] protected LootMovementSO movementStrategy;
    [SerializeField] protected float moveSpeed = 3f;

    // Movement State
    private Vector3 startPos;
    private float timeAlive;

    // Cache
    private Camera mainCamera;

    protected virtual void Start()
    {
        startPos = transform.position;
        mainCamera = Camera.main;
    }

    protected virtual void Update()
    {
        // 1. Handle Movement Logic
        if (movementStrategy != null)
        {
            timeAlive += Time.deltaTime;
            transform.position = movementStrategy.CalculatePosition(startPos, timeAlive, moveSpeed);
        }

        // 2. Check Bounds (Destroy if off-screen)
        CheckOutOfBounds();
    }

    private void CheckOutOfBounds()
    {
        if (mainCamera == null) return;

        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        // Allow a slight buffer (0.2) so it fully leaves screen before vanishing
        if (viewPos.y < -0.2f || viewPos.y > 1.2f || viewPos.x < -0.2f || viewPos.x > 1.2f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Standardize collision logic here
        IActor actor = other.GetComponentInParent<IActor>();
        if (actor != null)
        {
            if (Collect(actor))
            {
                OnCollected();
            }
        }
    }

    /// <summary>
    /// Derived classes implement specific effect application.
    /// </summary>
    public abstract bool Collect(IActor target);

    protected virtual void OnCollected()
    {
        // Optional: Spawn generic pickup VFX/Sound here

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