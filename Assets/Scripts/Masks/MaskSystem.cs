using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Mask System for MASK // LUMIN
/// Handles mask timer, depletion, and state management.
/// </summary>
public class MaskSystem : MonoBehaviour
{
    [Header("Mask Configurations")]
    [SerializeField, Tooltip("Configuration for Runner mask")]
    private MaskConfig runnerMask = new MaskConfig
    {
        maskType = MaskType.Runner,
        maskName = "Runner",
        glowColor = Color.cyan,
        duration = 25f,
        depletionMultiplier = 1f,
        speedModifier = 1.5f,
        jumpModifier = 1.3f,
        canShoot = false,
        canPhase = false
    };
    
    [SerializeField, Tooltip("Configuration for Hunter mask")]
    private MaskConfig hunterMask = new MaskConfig
    {
        maskType = MaskType.Hunter,
        maskName = "Hunter",
        glowColor = new Color(1f, 0.5f, 0f), // Orange
        duration = 20f,
        depletionMultiplier = 1f,
        speedModifier = 0.8f,
        jumpModifier = 0.9f,
        canShoot = true,
        canPhase = false
    };
    
    [SerializeField, Tooltip("Configuration for Ghost mask")]
    private MaskConfig ghostMask = new MaskConfig
    {
        maskType = MaskType.Ghost,
        maskName = "Ghost",
        glowColor = new Color(0.5f, 0f, 1f), // Purple
        duration = 15f,
        depletionMultiplier = 1.5f,
        speedModifier = 1f,
        jumpModifier = 1f,
        canShoot = false,
        canPhase = true
    };
    
    [Header("Warning Thresholds")]
    [SerializeField, Tooltip("Timer percentage when low warning triggers")]
    [Range(0f, 1f)]
    private float lowTimerThreshold = 0.25f;
    
    [SerializeField, Tooltip("Timer percentage when critical warning triggers")]
    [Range(0f, 1f)]
    private float criticalTimerThreshold = 0.1f;
    
    [Header("Events")]
    public UnityEvent<MaskType> OnMaskChanged;
    public UnityEvent<float> OnTimerChanged;
    public UnityEvent OnMaskDepleted;
    public UnityEvent OnLowTimerWarning;
    public UnityEvent OnCriticalTimerWarning;
    
    // Current state
    private MaskConfig currentMask;
    private float currentTimer;
    private bool isDepleted;
    private bool hasTriggeredLowWarning;
    private bool hasTriggeredCriticalWarning;
    
    // References
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    
    /// <summary>
    /// Current mask type
    /// </summary>
    public MaskType CurrentMaskType => currentMask?.maskType ?? MaskType.None;
    
    /// <summary>
    /// Current timer value
    /// </summary>
    public float CurrentTimer => currentTimer;
    
    /// <summary>
    /// Maximum timer for current mask
    /// </summary>
    public float MaxTimer => currentMask?.duration ?? 0f;
    
    /// <summary>
    /// Timer as normalized value (0-1)
    /// </summary>
    public float TimerNormalized => currentMask != null ? currentTimer / currentMask.duration : 0f;
    
    /// <summary>
    /// Is the current mask depleted?
    /// </summary>
    public bool IsDepleted => isDepleted;
    
    /// <summary>
    /// Is timer low? (below threshold)
    /// </summary>
    public bool IsTimerLow => TimerNormalized <= lowTimerThreshold;
    
    /// <summary>
    /// Is timer critical? (below critical threshold)
    /// </summary>
    public bool IsTimerCritical => TimerNormalized <= criticalTimerThreshold;
    
    /// <summary>
    /// Can player shoot with current mask?
    /// </summary>
    public bool CanShoot => currentMask?.canShoot ?? false;
    
    /// <summary>
    /// Can player phase through obstacles?
    /// </summary>
    public bool CanPhase => currentMask?.canPhase ?? false;
    
    /// <summary>
    /// Current glow color
    /// </summary>
    public Color CurrentGlowColor => currentMask?.glowColor ?? Color.white;
    
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
    }
    
    private void Start()
    {
        // Start with Runner mask by default
        SetMask(MaskType.Runner);
    }
    
    private void Update()
    {
        if (currentMask == null || isDepleted) return;
        
        // Countdown timer
        currentTimer -= Time.deltaTime * currentMask.depletionMultiplier;
        currentTimer = Mathf.Max(0, currentTimer);
        
        OnTimerChanged?.Invoke(currentTimer);
        
        // Check for warnings
        CheckWarnings();
        
        // Check for depletion
        if (currentTimer <= 0)
        {
            DepleteMask();
        }
    }
    
    private void CheckWarnings()
    {
        float normalized = TimerNormalized;
        
        // Low warning
        if (!hasTriggeredLowWarning && normalized <= lowTimerThreshold)
        {
            hasTriggeredLowWarning = true;
            OnLowTimerWarning?.Invoke();
        }
        
        // Critical warning
        if (!hasTriggeredCriticalWarning && normalized <= criticalTimerThreshold)
        {
            hasTriggeredCriticalWarning = true;
            OnCriticalTimerWarning?.Invoke();
        }
    }
    
    /// <summary>
    /// Set a new mask type
    /// </summary>
    public void SetMask(MaskType maskType)
    {
        MaskConfig newMask = GetMaskConfig(maskType);
        
        if (newMask == null)
        {
            Debug.LogWarning($"[MaskSystem] Unknown mask type: {maskType}");
            return;
        }
        
        currentMask = newMask;
        currentTimer = currentMask.duration;
        isDepleted = false;
        hasTriggeredLowWarning = false;
        hasTriggeredCriticalWarning = false;
        
        // Stop depletion damage if any
        if (playerHealth != null)
        {
            playerHealth.StopDepletionDamage();
        }
        
        // Apply movement modifiers
        ApplyMaskModifiers();
        
        OnMaskChanged?.Invoke(maskType);
        OnTimerChanged?.Invoke(currentTimer);
        
        Debug.Log($"[MaskSystem] Equipped {currentMask.maskName} mask");
    }
    
    /// <summary>
    /// Refill timer for current mask without changing type
    /// </summary>
    public void RefillTimer()
    {
        if (currentMask == null) return;
        
        currentTimer = currentMask.duration;
        isDepleted = false;
        hasTriggeredLowWarning = false;
        hasTriggeredCriticalWarning = false;
        
        if (playerHealth != null)
        {
            playerHealth.StopDepletionDamage();
        }
        
        ApplyMaskModifiers();
        
        OnTimerChanged?.Invoke(currentTimer);
    }
    
    private void DepleteMask()
    {
        isDepleted = true;
        OnMaskDepleted?.Invoke();
        
        // Start health drain
        if (playerHealth != null)
        {
            playerHealth.StartDepletionDamage();
        }
        
        // Apply heavy movement penalty
        if (playerMovement != null)
        {
            playerMovement.ApplyHeavyMovement(0.5f);
        }
        
        Debug.Log("[MaskSystem] Mask depleted! Health draining...");
    }
    
    private void ApplyMaskModifiers()
    {
        if (playerMovement == null || currentMask == null) return;
        
        playerMovement.SpeedMultiplier = currentMask.speedModifier;
        playerMovement.JumpMultiplier = currentMask.jumpModifier;
    }
    
    private MaskConfig GetMaskConfig(MaskType maskType)
    {
        return maskType switch
        {
            MaskType.Runner => runnerMask,
            MaskType.Hunter => hunterMask,
            MaskType.Ghost => ghostMask,
            _ => null
        };
    }
}
