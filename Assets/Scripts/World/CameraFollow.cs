using UnityEngine;

/// <summary>
/// Smooth Camera Follow for MASK // LUMIN
/// Follows the player with smoothing and optional bounds clamping.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField, Tooltip("Transform to follow (auto-finds Player if null)")]
    private Transform target;
    
    [Header("Follow Settings")]
    [SerializeField, Tooltip("How smoothly camera follows (lower = smoother)")]
    [Range(0.01f, 1f)]
    private float smoothSpeed = 0.125f;
    
    [SerializeField, Tooltip("Offset from target position")]
    private Vector3 offset = new Vector3(0, 2, -10);
    
    [Header("Bounds (Optional)")]
    [SerializeField, Tooltip("Enable camera bounds clamping")]
    private bool useBounds = false;
    
    [SerializeField, Tooltip("Minimum X position")]
    private float minX = -100f;
    
    [SerializeField, Tooltip("Maximum X position")]
    private float maxX = 100f;
    
    [SerializeField, Tooltip("Minimum Y position")]
    private float minY = -100f;
    
    [SerializeField, Tooltip("Maximum Y position")]
    private float maxY = 100f;
    
    [Header("Look Ahead")]
    [SerializeField, Tooltip("Enable look ahead in movement direction")]
    private bool lookAhead = false;
    
    [SerializeField, Tooltip("How far to look ahead")]
    private float lookAheadDistance = 2f;
    
    [SerializeField, Tooltip("How quickly to shift look ahead")]
    private float lookAheadSpeed = 2f;
    
    // State
    private Vector3 velocity = Vector3.zero;
    private float currentLookAhead = 0f;
    private Vector3 lastTargetPosition;
    
    private void Start()
    {
        // Auto-find player if no target assigned
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                // Try to find PlayerMovement component
                PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
                if (pm != null)
                {
                    target = pm.transform;
                }
            }
        }
        
        if (target != null)
        {
            lastTargetPosition = target.position;
            // Set initial position
            transform.position = target.position + offset;
        }
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Apply look ahead
        if (lookAhead)
        {
            float targetVelocityX = (target.position.x - lastTargetPosition.x) / Time.deltaTime;
            float targetLookAhead = Mathf.Sign(targetVelocityX) * lookAheadDistance;
            
            if (Mathf.Abs(targetVelocityX) > 0.1f)
            {
                currentLookAhead = Mathf.Lerp(currentLookAhead, targetLookAhead, Time.deltaTime * lookAheadSpeed);
            }
            else
            {
                currentLookAhead = Mathf.Lerp(currentLookAhead, 0, Time.deltaTime * lookAheadSpeed);
            }
            
            desiredPosition.x += currentLookAhead;
            lastTargetPosition = target.position;
        }
        
        // Smooth follow
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        
        // Apply bounds
        if (useBounds)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);
        }
        
        // Keep Z constant
        smoothedPosition.z = offset.z;
        
        transform.position = smoothedPosition;
    }
    
    /// <summary>
    /// Set a new target to follow
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            lastTargetPosition = target.position;
        }
    }
    
    /// <summary>
    /// Set camera bounds
    /// </summary>
    public void SetBounds(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
        useBounds = true;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!useBounds) return;
        
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);
        Gizmos.DrawWireCube(center, size);
    }
}
