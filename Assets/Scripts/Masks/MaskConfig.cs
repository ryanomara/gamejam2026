using UnityEngine;

/// <summary>
/// Mask Types for MASK // LUMIN
/// </summary>
public enum MaskType
{
    None,       // No mask (emergency state)
    Runner,     // Faster movement, longer jump, no shooting
    Hunter,     // Slower movement, shooting enabled
    Ghost       // Special traversal, faster depletion
}

/// <summary>
/// Mask configuration data
/// </summary>
[System.Serializable]
public class MaskConfig
{
    [Header("Identity")]
    public MaskType maskType;
    public string maskName;
    public Color glowColor = Color.cyan;
    
    [Header("Timer")]
    [Tooltip("How long this mask lasts in seconds")]
    public float duration = 20f;
    
    [Tooltip("Multiplier for depletion speed (higher = faster drain)")]
    public float depletionMultiplier = 1f;
    
    [Header("Movement Modifiers")]
    [Tooltip("Speed multiplier when wearing this mask")]
    public float speedModifier = 1f;
    
    [Tooltip("Jump multiplier when wearing this mask")]
    public float jumpModifier = 1f;
    
    [Header("Abilities")]
    [Tooltip("Can shoot while wearing this mask")]
    public bool canShoot = false;
    
    [Tooltip("Can pass through certain obstacles")]
    public bool canPhase = false;
}
