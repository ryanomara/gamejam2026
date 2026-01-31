using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Game HUD for MASK // LUMIN
/// Displays mask timer, health, and current mask type.
/// </summary>
public class GameHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("Reference to player's MaskSystem")]
    private MaskSystem maskSystem;
    
    [SerializeField, Tooltip("Reference to player's PlayerHealth")]
    private PlayerHealth playerHealth;
    
    [Header("Mask Timer UI")]
    [SerializeField, Tooltip("Timer fill bar image")]
    private Image timerFillBar;
    
    [SerializeField, Tooltip("Timer background image")]
    private Image timerBackground;
    
    [SerializeField, Tooltip("Text showing mask name")]
    private TextMeshProUGUI maskNameText;
    
    [Header("Health UI")]
    [SerializeField, Tooltip("Health fill bar image")]
    private Image healthFillBar;
    
    [SerializeField, Tooltip("Health text (optional)")]
    private TextMeshProUGUI healthText;
    
    [Header("Warning Colors")]
    [SerializeField, Tooltip("Normal timer color")]
    private Color normalColor = Color.cyan;
    
    [SerializeField, Tooltip("Low timer warning color")]
    private Color lowColor = Color.yellow;
    
    [SerializeField, Tooltip("Critical timer warning color")]
    private Color criticalColor = Color.red;
    
    [Header("Animation")]
    [SerializeField, Tooltip("Pulse speed when timer is low")]
    private float pulseSpeed = 4f;
    
    [SerializeField, Tooltip("Pulse intensity")]
    private float pulseIntensity = 0.2f;
    
    private bool isPulsing;
    
    private void Start()
    {
        // Auto-find player references if not set
        if (maskSystem == null)
        {
            maskSystem = FindFirstObjectByType<MaskSystem>();
        }
        
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }
        
        // Subscribe to events
        if (maskSystem != null)
        {
            maskSystem.OnMaskChanged.AddListener(OnMaskChanged);
            maskSystem.OnTimerChanged.AddListener(OnTimerChanged);
            maskSystem.OnLowTimerWarning.AddListener(OnLowTimerWarning);
            maskSystem.OnMaskDepleted.AddListener(OnMaskDepleted);
        }
        
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(OnHealthChanged);
        }
        
        // Initial update
        UpdateUI();
    }
    
    private void Update()
    {
        // Handle pulsing animation
        if (isPulsing && timerFillBar != null)
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
            timerFillBar.transform.localScale = Vector3.one * pulse;
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (maskSystem != null)
        {
            maskSystem.OnMaskChanged.RemoveListener(OnMaskChanged);
            maskSystem.OnTimerChanged.RemoveListener(OnTimerChanged);
            maskSystem.OnLowTimerWarning.RemoveListener(OnLowTimerWarning);
            maskSystem.OnMaskDepleted.RemoveListener(OnMaskDepleted);
        }
        
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(OnHealthChanged);
        }
    }
    
    private void UpdateUI()
    {
        if (maskSystem != null)
        {
            OnMaskChanged(maskSystem.CurrentMaskType);
            OnTimerChanged(maskSystem.CurrentTimer);
        }
        
        if (playerHealth != null)
        {
            OnHealthChanged(playerHealth.CurrentHealth);
        }
    }
    
    private void OnMaskChanged(MaskType maskType)
    {
        // Update mask name
        if (maskNameText != null)
        {
            maskNameText.text = maskType.ToString().ToUpper();
        }
        
        // Update timer bar color
        if (timerFillBar != null)
        {
            timerFillBar.color = maskSystem.CurrentGlowColor;
        }
        
        // Reset pulsing
        isPulsing = false;
        if (timerFillBar != null)
        {
            timerFillBar.transform.localScale = Vector3.one;
        }
    }
    
    private void OnTimerChanged(float timer)
    {
        if (maskSystem == null || timerFillBar == null) return;
        
        // Update fill amount
        timerFillBar.fillAmount = maskSystem.TimerNormalized;
        
        // Update color based on timer state
        if (maskSystem.IsTimerCritical)
        {
            timerFillBar.color = criticalColor;
        }
        else if (maskSystem.IsTimerLow)
        {
            timerFillBar.color = lowColor;
        }
        else
        {
            timerFillBar.color = maskSystem.CurrentGlowColor;
        }
    }
    
    private void OnHealthChanged(float health)
    {
        if (playerHealth == null) return;
        
        // Update health bar
        if (healthFillBar != null)
        {
            healthFillBar.fillAmount = playerHealth.HealthNormalized;
            
            // Color based on health
            if (playerHealth.HealthNormalized <= 0.25f)
            {
                healthFillBar.color = criticalColor;
            }
            else if (playerHealth.HealthNormalized <= 0.5f)
            {
                healthFillBar.color = lowColor;
            }
            else
            {
                healthFillBar.color = Color.green;
            }
        }
        
        // Update health text
        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(health)}/{Mathf.CeilToInt(playerHealth.MaxHealth)}";
        }
    }
    
    private void OnLowTimerWarning()
    {
        isPulsing = true;
    }
    
    private void OnMaskDepleted()
    {
        if (maskNameText != null)
        {
            maskNameText.text = "NO MASK!";
            maskNameText.color = criticalColor;
        }
        
        if (timerFillBar != null)
        {
            timerFillBar.fillAmount = 0;
        }
    }
}
