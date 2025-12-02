using UnityEngine;

/// <summary>
/// Scrolls the texture of a Renderer to simulate movement.
/// Attach to a Quad or Plane with a seamless/tiling material.
/// </summary>
public class ParallaxScroller : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float scrollSpeed = 0.5f;
    [SerializeField] private Vector2 scrollDirection = Vector2.up; // Standard Vertical Scroll

    [Header("Shader Settings")]
    [Tooltip("The property name of the texture to scroll. Use '_MainTex' for Standard/Built-in, or '_BaseMap' for URP.")]
    [SerializeField] private string texturePropertyName = "_MainTex";

    private Material targetMaterial;
    private Vector2 currentOffset;
    private int texturePropertyID;

    void Start()
    {
        Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            targetMaterial = r.material;
            // Cache the ID for performance
            texturePropertyID = Shader.PropertyToID(texturePropertyName);
        }
        else
        {
            Debug.LogError("[ParallaxScroller] No Renderer found! Cannot scroll.");
        }
    }

    void Update()
    {
        if (targetMaterial != null)
        {
            // Calculate new offset
            currentOffset += scrollDirection.normalized * scrollSpeed * Time.deltaTime;

            // Apply to material using the cached ID
            targetMaterial.SetTextureOffset(texturePropertyID, currentOffset);
        }
    }
}