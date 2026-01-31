using UnityEngine;

/// <summary>
/// Player Shooting System for MASK // LUMIN
/// Only enabled when wearing Hunter mask.
/// </summary>
public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("Projectile prefab")]
    private GameObject projectilePrefab;
    
    [SerializeField, Tooltip("Fire point transform")]
    private Transform firePoint;
    
    [Header("Settings")]
    [SerializeField, Tooltip("Cooldown between shots")]
    private float fireCooldown = 0.3f;
    
    [SerializeField, Tooltip("Fire input button")]
    private KeyCode fireButton = KeyCode.Mouse0;
    
    [SerializeField, Tooltip("Alternative fire button")]
    private KeyCode altFireButton = KeyCode.X;
    
    // State
    private float lastFireTime;
    private MaskSystem maskSystem;
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        maskSystem = GetComponent<MaskSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        // Only shoot if Hunter mask
        if (maskSystem == null || !maskSystem.CanShoot) return;
        
        // Check fire input
        if (Input.GetKey(fireButton) || Input.GetKey(altFireButton))
        {
            TryFire();
        }
    }
    
    private void TryFire()
    {
        // Check cooldown
        if (Time.time - lastFireTime < fireCooldown) return;
        
        Fire();
        lastFireTime = Time.time;
    }
    
    private void Fire()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("[PlayerShooting] No projectile prefab assigned!");
            return;
        }
        
        // Determine fire position
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        
        // Determine direction based on sprite flip
        Vector2 direction = Vector2.right;
        if (spriteRenderer != null && spriteRenderer.flipX)
        {
            direction = Vector2.left;
        }
        
        // Spawn projectile
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        
        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetDirection(direction);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
        }
    }
}
