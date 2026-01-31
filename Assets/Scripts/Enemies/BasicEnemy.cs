using UnityEngine;

/// <summary>
/// Basic Enemy for MASK // LUMIN
/// Simple patrol enemy that damages player on contact and dies when shot.
/// </summary>
public class BasicEnemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField, Tooltip("Patrol speed")]
    private float patrolSpeed = 2f;
    
    [SerializeField, Tooltip("Left patrol boundary (local offset)")]
    private float leftBoundary = -3f;
    
    [SerializeField, Tooltip("Right patrol boundary (local offset)")]
    private float rightBoundary = 3f;
    
    [Header("Combat Settings")]
    [SerializeField, Tooltip("Damage dealt on contact")]
    private float contactDamage = 20f;
    
    [SerializeField, Tooltip("Enemy health")]
    private float health = 30f;
    
    [Header("Visual")]
    [SerializeField, Tooltip("Glow color")]
    private Color glowColor = new Color(1f, 0.2f, 0.2f); // Red
    
    // State
    private Vector3 startPosition;
    private bool movingRight = true;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
    }
    
    private void Start()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = glowColor;
        }
    }
    
    private void Update()
    {
        if (isDead) return;
        
        Patrol();
    }
    
    private void Patrol()
    {
        // Calculate world boundaries
        float leftLimit = startPosition.x + leftBoundary;
        float rightLimit = startPosition.x + rightBoundary;
        
        // Move
        float direction = movingRight ? 1f : -1f;
        transform.Translate(Vector3.right * direction * patrolSpeed * Time.deltaTime);
        
        // Check boundaries
        if (transform.position.x >= rightLimit)
        {
            movingRight = false;
            FlipSprite();
        }
        else if (transform.position.x <= leftLimit)
        {
            movingRight = true;
            FlipSprite();
        }
    }
    
    private void FlipSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !movingRight;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        
        // Damage player on contact
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(contactDamage);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        
        // Check if hit by projectile
        if (other.CompareTag("Projectile"))
        {
            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null)
            {
                TakeDamage(projectile.Damage);
            }
            Destroy(other.gameObject);
        }
    }
    
    /// <summary>
    /// Take damage from any source
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        health -= damage;
        
        // Visual feedback
        StartCoroutine(FlashRed());
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    private System.Collections.IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }
    
    private void Die()
    {
        isDead = true;
        
        // Simple death effect - could add particles here
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        
        // Disable collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        // Destroy after delay
        Destroy(gameObject, 0.5f);
    }
    
    private void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? startPosition : transform.position;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            center + Vector3.left * Mathf.Abs(leftBoundary),
            center + Vector3.right * rightBoundary
        );
        
        Gizmos.color = glowColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
