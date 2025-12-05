using UnityEngine;
using UnityEngine.VFX;
using System;
using System.Collections;

/// <summary>
/// Handles the visual death sequence of an enemy.
/// Plays a VFX Graph and lerps a Shader Dissolve value before destroying the object.
/// Updated to align VFX properties (Velocity, BoxCenter, BoxSize) with the sprite's rotation.
/// </summary>
public class DeathVisuals : MonoBehaviour, IGameComponent
{
    [Header("Components")]
    [Tooltip("The SpriteRenderer to apply the dissolve effect to.")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("The VFX Graph to play upon death.")]
    [SerializeField] private VisualEffect deathVFX;

    [Header("Settings")]
    [SerializeField] private float dissolveDuration = 1.0f;
    [SerializeField] private string dissolvePropertyName = "_Dissolve";
    [SerializeField] private float particleSpeed = 2.0f;

    // Optimization: Use PropertyBlock to avoid creating material instances
    private MaterialPropertyBlock propBlock;
    private int dissolvePropID;

    // VFX Graph Property IDs
    private int vfxVelocityID;
    private int vfxBoxCenterID;
    private int vfxBoxSizeID;

    public void Initialize(IActor actor)
    {
        // Auto-find if not assigned
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (deathVFX == null) deathVFX = GetComponentInChildren<VisualEffect>();

        propBlock = new MaterialPropertyBlock();
        dissolvePropID = Shader.PropertyToID(dissolvePropertyName);

        // Cache VFX Property IDs (Exposed names in VFX Graph)
        vfxVelocityID = Shader.PropertyToID("Velocity");
        vfxBoxCenterID = Shader.PropertyToID("BoxCenter");
        vfxBoxSizeID = Shader.PropertyToID("BoxSize");
    }

    /// <summary>
    /// Starts the death sequence.
    /// </summary>
    /// <param name="onComplete">Callback to execute when visuals are finished (usually Destroy).</param>
    public void StartDeathEffect(Action onComplete)
    {
        // 1. Setup and Play Particles
        if (deathVFX != null && spriteRenderer != null)
        {
            ConfigureVFX();
            deathVFX.Play();
        }

        // 2. Start Dissolve
        if (spriteRenderer != null)
        {
            StartCoroutine(DissolveRoutine(onComplete));
        }
        else
        {
            // If no visuals to play, finish immediately
            onComplete?.Invoke();
        }
    }

    private void ConfigureVFX()
    {

        float worldHeight = spriteRenderer.bounds.size.y;
        Vector3 worldCenter = spriteRenderer.bounds.center;
        Vector3 worldBottom = worldCenter + transform.up * -1 * (worldHeight * 0.5f);

        Transform vfxTransform = deathVFX.transform;
        Vector3 localBottom = vfxTransform.InverseTransformPoint(worldBottom);

        deathVFX.SetVector3(vfxBoxCenterID, localBottom);

        Vector3 worldSize = spriteRenderer.bounds.size;
        Vector3 localSize = vfxTransform.InverseTransformVector(worldSize);
        deathVFX.SetVector3(vfxBoxSizeID, localSize);

        Vector3 velocityDir = transform.up;
        deathVFX.SetVector3(vfxVelocityID, velocityDir * particleSpeed);
    }

    private IEnumerator DissolveRoutine(Action onComplete)
    {
        float elapsed = 0f;

        while (elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dissolveDuration);

            // Apply dissolve value (0 to 1)
            spriteRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat(dissolvePropID, t);
            spriteRenderer.SetPropertyBlock(propBlock);

            yield return null;
        }

        // Ensure fully dissolved
        spriteRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat(dissolvePropID, 1f);
        spriteRenderer.SetPropertyBlock(propBlock);

        // 3. Finish
        onComplete?.Invoke();
    }

    public void ResetComponent()
    {
        propBlock.SetFloat(dissolvePropID, 0);
    }
}