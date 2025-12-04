using UnityEngine;
using UnityEngine.VFX;

public class BossHitVisuals : MonoBehaviour, IGameComponent
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private VisualEffect impactVFX;

    [Header("Audio")]
    [SerializeField] private AudioClip impactSound; // NEW

    [Header("Settings")]
    [SerializeField] private float cooldown = 0.05f;
    [SerializeField] private float positionZOffset = -0.1f;

    private float lastHitTime;
    private HealthComponent healthComponent;

    public void Initialize(IActor actor)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        healthComponent = actor.GetAttachedComponent<HealthComponent>();

        if (healthComponent != null)
        {
            healthComponent.OnHit += HandleHit;
        }
    }

    void OnDestroy()
    {
        if (healthComponent != null)
        {
            healthComponent.OnHit -= HandleHit;
        }
    }

    private void HandleHit(GameObject source)
    {
        if (Time.time < lastHitTime + cooldown) return;
        if (impactVFX == null || spriteRenderer == null) return;

        lastHitTime = Time.time;

        // 1. Visuals
        Bounds bounds = spriteRenderer.localBounds;
        float randX = Random.Range(bounds.min.x, bounds.max.x);
        float randY = Random.Range(bounds.min.y, bounds.max.y);
        Vector3 localPoint = new Vector3(randX, randY, 0f);
        localPoint.z += positionZOffset;

        impactVFX.transform.localPosition = localPoint;
        impactVFX.Play();

        // 2. Audio (NEW)
        if (impactSound != null && EventManager.Instance != null)
        {
            // Use existing generic sound trigger
            EventManager.Instance.TriggerExplosion(transform.position, impactSound);
        }
    }
}