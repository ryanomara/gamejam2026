using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Player Health System for MASK // LUMIN
/// Handles damage, death, and health regeneration.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField, Tooltip("Maximum health")]
    private float maxHealth = 100f;
    
    [SerializeField, Tooltip("Current health")]
    private float currentHealth;
    
    [Header("Mask Depletion Damage")]
    [SerializeField, Tooltip("Damage per second when mask is depleted")]
    private float depletionDamageRate = 10f;
    
    [Header("Invincibility")]
    [SerializeField, Tooltip("Time player is invincible after taking damage")]
    private float invincibilityDuration = 0.5f;
    
    [Header("Events")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnDeath;
    public UnityEvent OnDamaged;
    
    // State
    private bool isDead;
    private bool isInvincible;
    private float invincibilityTimer;
    private bool isTakingDepletionDamage;
    
    /// <summary>
    /// Current health value
    /// </summary>
    public float CurrentHealth => currentHealth;
    
    /// <summary>
    /// Maximum health value
    /// </summary>
    public float MaxHealth => maxHealth;
    
    /// <summary>
    /// Health as a normalized value (0-1)
    /// </summary>
    public float HealthNormalized => currentHealth / maxHealth;
    
    /// <summary>
    /// Is the player dead?
    /// </summary>
    public bool IsDead => isDead;
    
    /// <summary>
    /// Is the player currently taking depletion damage?
    /// </summary>
    public bool IsTakingDepletionDamage => isTakingDepletionDamage;
    
    private void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;
    }
    
    private void Update()
    {
        // Handle invincibility timer
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
        
        // Handle depletion damage
        if (isTakingDepletionDamage && !isDead)
        {
            TakeDamage(depletionDamageRate * Time.deltaTime, ignoreInvincibility: true);
        }
    }
    
    /// <summary>
    /// Take damage from any source
    /// </summary>
    /// <param name="damage">Amount of damage</param>
    /// <param name="ignoreInvincibility">If true, damage is applied even during invincibility</param>
    public void TakeDamage(float damage, bool ignoreInvincibility = false)
    {
        if (isDead) return;
        
        if (isInvincible && !ignoreInvincibility) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        OnHealthChanged?.Invoke(currentHealth);
        
        if (!ignoreInvincibility)
        {
            OnDamaged?.Invoke();
            StartInvincibility();
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Heal the player
    /// </summary>
    /// <param name="amount">Amount to heal</param>
    public void Heal(float amount)
    {
        if (isDead) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    /// <summary>
    /// Start taking damage from mask depletion
    /// </summary>
    public void StartDepletionDamage()
    {
        isTakingDepletionDamage = true;
    }
    
    /// <summary>
    /// Stop taking damage from mask depletion
    /// </summary>
    public void StopDepletionDamage()
    {
        isTakingDepletionDamage = false;
    }
    
    /// <summary>
    /// Reset health to maximum
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        isTakingDepletionDamage = false;
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    private void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }
    
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        isTakingDepletionDamage = false;
        OnDeath?.Invoke();
        
        Debug.Log("[PlayerHealth] Player died!");
    }
}
