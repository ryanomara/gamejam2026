using UnityEngine;

/// <summary>
/// Mask Pickup for MASK // LUMIN
/// When player contacts this pickup, it assigns a mask type and refills timer.
/// </summary>
public class MaskPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField, Tooltip("Type of mask this pickup provides")]
    private MaskType maskType = MaskType.Runner;
    
    [SerializeField, Tooltip("Glow color for this pickup")]
    private Color glowColor = Color.cyan;
    
    [Header("Visual Feedback")]
    [SerializeField, Tooltip("Float amplitude")]
    private float floatAmplitude = 0.2f;
    
    [SerializeField, Tooltip("Float speed")]
    private float floatSpeed = 2f;
    
    [SerializeField, Tooltip("Rotation speed (degrees per second)")]
    private float rotationSpeed = 45f;
    
    [Header("Audio")]
    [SerializeField, Tooltip("Sound played when picked up")]
    private AudioClip pickupSound;
    
    // Components
    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition;
    
    /// <summary>
    /// The mask type this pickup provides
    /// </summary>
    public MaskType MaskType => maskType;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
    }
    
    private void Start()
    {
        // Apply glow color based on mask type
        UpdateGlowColor();
    }
    
    private void Update()
    {
        // Float animation
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = startPosition + Vector3.up * yOffset;
        
        // Rotation animation
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if player
        MaskSystem maskSystem = other.GetComponent<MaskSystem>();
        if (maskSystem != null)
        {
            // Apply mask to player
            maskSystem.SetMask(maskType);
            
            // Play sound
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            
            // Destroy pickup
            Destroy(gameObject);
        }
    }
    
    private void UpdateGlowColor()
    {
        // Set color based on mask type
        glowColor = maskType switch
        {
            MaskType.Runner => Color.cyan,
            MaskType.Hunter => new Color(1f, 0.5f, 0f), // Orange
            MaskType.Ghost => new Color(0.5f, 0f, 1f), // Purple
            _ => Color.white
        };
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = glowColor;
        }
    }
    
    private void OnValidate()
    {
        // Update color in editor when mask type changes
        UpdateGlowColor();
    }
    
    private void OnDrawGizmos()
    {
        // Draw pickup icon in editor
        Gizmos.color = glowColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
