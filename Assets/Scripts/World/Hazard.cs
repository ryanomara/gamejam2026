using UnityEngine;

/// <summary>
/// Hazard for MASK // LUMIN
/// Damages player on contact.
/// </summary>
public class Hazard : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField, Tooltip("Damage dealt on contact")]
    private float damage = 25f;
    
    [SerializeField, Tooltip("If true, kills player instantly")]
    private bool instantKill = false;
    
    [SerializeField, Tooltip("If true, applies continuous damage while in contact")]
    private bool continuousDamage = false;
    
    [SerializeField, Tooltip("Damage per second for continuous damage")]
    private float damagePerSecond = 20f;
    
    [Header("Visual")]
    [SerializeField, Tooltip("Glow color for this hazard")]
    private Color hazardColor = Color.red;
    
    // State
    private bool playerInContact;
    private PlayerHealth playerHealthInContact;
    
    private void Start()
    {
        // Apply hazard color to sprite
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = hazardColor;
        }
    }
    
    private void Update()
    {
        // Handle continuous damage
        if (continuousDamage && playerInContact && playerHealthInContact != null)
        {
            playerHealthInContact.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            if (instantKill)
            {
                playerHealth.TakeDamage(playerHealth.MaxHealth * 10);
            }
            else
            {
                playerHealth.TakeDamage(damage);
            }
            
            if (continuousDamage)
            {
                playerInContact = true;
                playerHealthInContact = playerHealth;
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerInContact = false;
            playerHealthInContact = null;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            if (instantKill)
            {
                playerHealth.TakeDamage(playerHealth.MaxHealth * 10);
            }
            else
            {
                playerHealth.TakeDamage(damage);
            }
            
            if (continuousDamage)
            {
                playerInContact = true;
                playerHealthInContact = playerHealth;
            }
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerInContact = false;
            playerHealthInContact = null;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = hazardColor;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
