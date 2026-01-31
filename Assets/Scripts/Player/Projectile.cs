using UnityEngine;

/// <summary>
/// Projectile for Hunter Mask shooting in MASK // LUMIN
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Tooltip("Projectile speed")]
    private float speed = 15f;
    
    [SerializeField, Tooltip("Damage dealt")]
    private float damage = 15f;
    
    [SerializeField, Tooltip("Lifetime in seconds")]
    private float lifetime = 3f;
    
    [Header("Visual")]
    [SerializeField, Tooltip("Projectile color")]
    private Color projectileColor = new Color(1f, 0.5f, 0f); // Orange
    
    // Direction
    private Vector2 direction = Vector2.right;
    
    /// <summary>
    /// Damage dealt by this projectile
    /// </summary>
    public float Damage => damage;
    
    private void Start()
    {
        // Set color
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = projectileColor;
        }
        
        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }
    
    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
    
    /// <summary>
    /// Set projectile direction
    /// </summary>
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        
        // Rotate to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Don't hit player
        if (other.GetComponent<PlayerMovement>() != null) return;
        if (other.CompareTag("Player")) return;
        
        // Hit enemy
        BasicEnemy enemy = other.GetComponent<BasicEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        
        // Hit wall/ground
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
