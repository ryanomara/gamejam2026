using UnityEngine;

/// <summary>
/// Level Exit Trigger for MASK // LUMIN
/// Triggers win condition when player reaches the exit.
/// </summary>
public class LevelExit : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Tooltip("Scene to load on exit (leave empty to just trigger win)")]
    private string nextSceneName = "";
    
    [Header("Visual")]
    [SerializeField, Tooltip("Glow color for exit")]
    private Color exitColor = Color.green;
    
    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnExitReached;
    
    private bool triggered = false;
    
    private void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = exitColor;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        
        // Check if player
        if (other.GetComponent<PlayerMovement>() != null || other.CompareTag("Player"))
        {
            triggered = true;
            OnExitReached?.Invoke();
            
            Debug.Log("[LevelExit] Player reached exit!");
            
            // Load next scene if specified
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = exitColor;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
