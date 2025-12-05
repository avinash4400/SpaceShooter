using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

/// <summary>
/// Handles the visual feedback when an actor is healed.
/// Plays a VFX Graph and animates a Shader Flash value.
/// Aligns VFX properties (BoxCenter, BoxSize) with the sprite's dimensions.
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
        // Auto-find if not assigned
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
            Debug.LogWarningFormat("[HealVisuals] Playing heal VFX for {0}", gameObject.name);
            healVFX.Stop();
            healVFX.Play();
        }
    }
}