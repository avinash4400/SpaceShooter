using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Handles the visual feedback when an actor is healed.
/// </summary>
public class HealVisuals : MonoBehaviour, IGameComponent
{
    [Header("Components")]
    [Tooltip("The SpriteRenderer to apply the flash effect to.")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("The VFX Graph to play upon healing.")]
    [SerializeField] private VisualEffect healVFX;


    public void Initialize(IActor actor)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (healVFX == null) healVFX = GetComponentInChildren<VisualEffect>();

    }

    /// <summary>
    /// Triggers the healing visual effects (Particles + Flash).
    /// </summary>
    public void PlayHealEffect()
    {
        // 1. Setup and Play Particles
        if (healVFX != null && spriteRenderer != null)
        {
            healVFX.Stop();
            healVFX.Play();
        }
    }
}