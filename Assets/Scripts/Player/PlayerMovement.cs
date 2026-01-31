using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player Movement Controller for MASK // LUMIN
/// Rigidbody2D-based movement with left/right and jump.
/// Inspector-tunable values. No wall jump, no dash.
/// Uses new Input System.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField, Tooltip("Horizontal movement speed")]
    private float moveSpeed = 8f;
    
    [SerializeField, Tooltip("Force applied when jumping")]
    private float jumpForce = 14f;
    
    [Header("Ground Check")]
    [SerializeField, Tooltip("Point to check for ground")]
    private Transform groundCheck;
    
    [SerializeField, Tooltip("Radius of ground check circle")]
    private float groundCheckRadius = 0.2f;
    
    [SerializeField, Tooltip("Layer mask for ground detection")]
    private LayerMask groundLayer;
    
    [Header("Movement Modifiers")]
    [SerializeField, Tooltip("Speed multiplier (modified by masks)")]
    private float speedMultiplier = 1f;
    
    [SerializeField, Tooltip("Jump multiplier (modified by masks)")]
    private float jumpMultiplier = 1f;
    
    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    
    // State
    private float horizontalInput;
    private bool isGrounded;
    private bool jumpRequested;
    
    /// <summary>
    /// Current speed multiplier, can be modified by mask system
    /// </summary>
    public float SpeedMultiplier
    {
        get => speedMultiplier;
        set => speedMultiplier = Mathf.Max(0.1f, value);
    }
    
    /// <summary>
    /// Current jump multiplier, can be modified by mask system
    /// </summary>
    public float JumpMultiplier
    {
        get => jumpMultiplier;
        set => jumpMultiplier = Mathf.Max(0.1f, value);
    }
    
    /// <summary>
    /// Returns true if player is on the ground
    /// </summary>
    public bool IsGrounded => isGrounded;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (rb == null)
        {
            Debug.LogError("[PlayerMovement] Missing Rigidbody2D component!");
        }
    }
    
    private void Update()
    {
        // Get horizontal input using new Input System
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            float left = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? -1f : 0f;
            float right = keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f;
            horizontalInput = left + right;
            
            // Check for jump input (only if grounded)
            if ((keyboard.spaceKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame) && isGrounded)
            {
                jumpRequested = true;
            }
        }
        
        // Flip sprite based on movement direction
        if (horizontalInput != 0 && spriteRenderer != null)
        {
            spriteRenderer.flipX = horizontalInput < 0;
        }
    }
    
    private void FixedUpdate()
    {
        // Ground check
        CheckGround();
        
        // Apply horizontal movement
        ApplyMovement();
        
        // Apply jump if requested
        if (jumpRequested)
        {
            ApplyJump();
            jumpRequested = false;
        }
    }
    
    private void CheckGround()
    {
        if (groundCheck == null)
        {
            // Fallback: use player position with offset
            isGrounded = Physics2D.OverlapCircle(
                (Vector2)transform.position + Vector2.down * 0.5f,
                groundCheckRadius,
                groundLayer
            );
        }
        else
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }
    }
    
    private void ApplyMovement()
    {
        float targetVelocityX = horizontalInput * moveSpeed * speedMultiplier;
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }
    
    private void ApplyJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * jumpMultiplier);
    }
    
    /// <summary>
    /// Reset movement modifiers to default
    /// </summary>
    public void ResetModifiers()
    {
        speedMultiplier = 1f;
        jumpMultiplier = 1f;
    }
    
    /// <summary>
    /// Apply heavy/slow movement (used when mask depletes)
    /// </summary>
    public void ApplyHeavyMovement(float slowFactor = 0.5f)
    {
        speedMultiplier = slowFactor;
        jumpMultiplier = slowFactor;
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw ground check radius in editor
        Gizmos.color = Color.green;
        Vector3 checkPos = groundCheck != null 
            ? groundCheck.position 
            : transform.position + Vector3.down * 0.5f;
        Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
    }
}
