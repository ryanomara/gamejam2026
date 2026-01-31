using UnityEngine;

/// <summary>
/// Fake Glow System for MASK // LUMIN
/// Creates bioluminescent glow effect using sprite duplication.
/// No shaders required - works with standard sprite renderers.
/// </summary>
public class FakeGlow : MonoBehaviour
{
    [Header("Glow Settings")]
    [SerializeField, Tooltip("Glow color (alpha controls intensity)")]
    private Color glowColor = new Color(0f, 1f, 1f, 0.5f); // Cyan with 50% alpha
    
    [SerializeField, Tooltip("Scale multiplier for glow sprite")]
    [Range(1f, 2f)]
    private float glowScale = 1.3f;
    
    [SerializeField, Tooltip("Sorting order offset for glow (should be behind main sprite)")]
    private int sortingOrderOffset = -1;
    
    [Header("Pulse Animation")]
    [SerializeField, Tooltip("Enable pulsing glow")]
    private bool enablePulse = true;
    
    [SerializeField, Tooltip("Pulse speed")]
    private float pulseSpeed = 2f;
    
    [SerializeField, Tooltip("Minimum alpha during pulse")]
    [Range(0f, 1f)]
    private float minAlpha = 0.3f;
    
    [SerializeField, Tooltip("Maximum alpha during pulse")]
    [Range(0f, 1f)]
    private float maxAlpha = 0.7f;
    
    [Header("Auto Setup")]
    [SerializeField, Tooltip("Automatically create glow sprite on start")]
    private bool autoCreateGlow = true;
    
    // References
    private SpriteRenderer mainSprite;
    private SpriteRenderer glowSprite;
    private GameObject glowObject;
    
    /// <summary>
    /// Current glow color
    /// </summary>
    public Color GlowColor
    {
        get => glowColor;
        set
        {
            glowColor = value;
            if (glowSprite != null)
            {
                glowSprite.color = glowColor;
            }
        }
    }
    
    private void Awake()
    {
        mainSprite = GetComponent<SpriteRenderer>();
    }
    
    private void Start()
    {
        if (autoCreateGlow && mainSprite != null)
        {
            CreateGlowSprite();
        }
    }
    
    private void Update()
    {
        if (glowSprite == null || !enablePulse) return;
        
        // Pulse animation using sin wave
        float pulse = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        Color c = glowSprite.color;
        c.a = pulse;
        glowSprite.color = c;
    }
    
    /// <summary>
    /// Create the glow sprite child object
    /// </summary>
    public void CreateGlowSprite()
    {
        if (mainSprite == null)
        {
            Debug.LogWarning("[FakeGlow] No SpriteRenderer found on main object!");
            return;
        }
        
        // Clean up existing glow if any
        DestroyGlow();
        
        // Create glow object
        glowObject = new GameObject("Glow");
        glowObject.transform.SetParent(transform);
        glowObject.transform.localPosition = Vector3.zero;
        glowObject.transform.localRotation = Quaternion.identity;
        glowObject.transform.localScale = Vector3.one * glowScale;
        
        // Add sprite renderer
        glowSprite = glowObject.AddComponent<SpriteRenderer>();
        glowSprite.sprite = mainSprite.sprite;
        glowSprite.color = glowColor;
        glowSprite.sortingLayerName = mainSprite.sortingLayerName;
        glowSprite.sortingOrder = mainSprite.sortingOrder + sortingOrderOffset;
        
        // Copy flip state
        glowSprite.flipX = mainSprite.flipX;
        glowSprite.flipY = mainSprite.flipY;
    }
    
    /// <summary>
    /// Destroy the glow sprite
    /// </summary>
    public void DestroyGlow()
    {
        if (glowObject != null)
        {
            if (Application.isPlaying)
            {
                Destroy(glowObject);
            }
            else
            {
                DestroyImmediate(glowObject);
            }
            glowObject = null;
            glowSprite = null;
        }
    }
    
    /// <summary>
    /// Update glow sprite to match main sprite changes
    /// </summary>
    public void SyncGlowSprite()
    {
        if (mainSprite == null || glowSprite == null) return;
        
        glowSprite.sprite = mainSprite.sprite;
        glowSprite.flipX = mainSprite.flipX;
        glowSprite.flipY = mainSprite.flipY;
    }
    
    /// <summary>
    /// Set glow intensity (alpha)
    /// </summary>
    public void SetIntensity(float intensity)
    {
        if (glowSprite == null) return;
        
        Color c = glowSprite.color;
        c.a = Mathf.Clamp01(intensity);
        glowSprite.color = c;
    }
    
    /// <summary>
    /// Enable or disable the glow
    /// </summary>
    public void SetGlowEnabled(bool enabled)
    {
        if (glowObject != null)
        {
            glowObject.SetActive(enabled);
        }
    }
    
    private void OnDestroy()
    {
        DestroyGlow();
    }
    
    private void OnValidate()
    {
        // Update in editor
        if (glowSprite != null)
        {
            glowSprite.color = glowColor;
            glowObject.transform.localScale = Vector3.one * glowScale;
        }
    }
}
