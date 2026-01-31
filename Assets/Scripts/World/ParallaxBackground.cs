using UnityEngine;

/// <summary>
/// Parallax Background System for MASK // LUMIN
/// Creates 2.5D depth illusion through layered parallax scrolling.
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("The camera to follow (auto-finds main camera if null)")]
    private Camera targetCamera;
    
    [Header("Parallax Settings")]
    [SerializeField, Tooltip("Parallax effect amount (0 = no movement, 1 = moves with camera)")]
    [Range(0f, 1f)]
    private float parallaxEffect = 0.5f;
    
    [SerializeField, Tooltip("Enable vertical parallax")]
    private bool enableVerticalParallax = false;
    
    [SerializeField, Tooltip("Vertical parallax amount")]
    [Range(0f, 1f)]
    private float verticalParallaxEffect = 0.5f;
    
    [Header("Infinite Scrolling")]
    [SerializeField, Tooltip("Enable infinite horizontal scrolling")]
    private bool infiniteHorizontal = false;
    
    [SerializeField, Tooltip("Width of the sprite for infinite scrolling")]
    private float spriteWidth = 20f;
    
    // Internal state
    private Vector3 startPosition;
    private float startCameraX;
    private float startCameraY;
    
    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }
    
    private void Start()
    {
        startPosition = transform.position;
        
        if (targetCamera != null)
        {
            startCameraX = targetCamera.transform.position.x;
            startCameraY = targetCamera.transform.position.y;
        }
        
        // Auto-calculate sprite width if we have a SpriteRenderer
        if (infiniteHorizontal && spriteWidth <= 0)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                spriteWidth = sr.bounds.size.x;
            }
        }
    }
    
    private void LateUpdate()
    {
        if (targetCamera == null) return;
        
        // Calculate camera movement delta
        float cameraDeltaX = targetCamera.transform.position.x - startCameraX;
        float cameraDeltaY = targetCamera.transform.position.y - startCameraY;
        
        // Calculate parallax offset
        float parallaxOffsetX = cameraDeltaX * parallaxEffect;
        float parallaxOffsetY = enableVerticalParallax ? cameraDeltaY * verticalParallaxEffect : 0f;
        
        // Apply parallax
        transform.position = new Vector3(
            startPosition.x + parallaxOffsetX,
            startPosition.y + parallaxOffsetY,
            startPosition.z
        );
        
        // Handle infinite scrolling
        if (infiniteHorizontal && spriteWidth > 0)
        {
            float relativeCameraX = targetCamera.transform.position.x * (1 - parallaxEffect);
            
            if (relativeCameraX > startPosition.x + spriteWidth)
            {
                startPosition.x += spriteWidth;
            }
            else if (relativeCameraX < startPosition.x - spriteWidth)
            {
                startPosition.x -= spriteWidth;
            }
        }
    }
    
    /// <summary>
    /// Set parallax effect at runtime
    /// </summary>
    public void SetParallaxEffect(float effect)
    {
        parallaxEffect = Mathf.Clamp01(effect);
    }
}
