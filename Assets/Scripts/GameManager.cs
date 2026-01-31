using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// Game Manager for MASK // LUMIN
/// Handles game state, win/lose conditions, and restart.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField, Tooltip("Reference to the player (auto-finds if null)")]
    private GameObject player;
    
    [Header("Spawn Settings")]
    [SerializeField, Tooltip("Player spawn point")]
    private Transform spawnPoint;
    
    [Header("UI Panels")]
    [SerializeField, Tooltip("Game Over panel")]
    private GameObject gameOverPanel;
    
    [SerializeField, Tooltip("Victory panel")]
    private GameObject victoryPanel;
    
    [Header("Events")]
    public UnityEvent OnGameStart;
    public UnityEvent OnGameOver;
    public UnityEvent OnVictory;
    public UnityEvent OnRestart;
    
    // State
    private bool isGameOver;
    private bool isVictory;
    
    // Singleton (optional, useful for UI access)
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        // Simple singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // Auto-find player
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
                if (pm != null)
                {
                    player = pm.gameObject;
                }
            }
        }
        
        // Subscribe to player death
        if (player != null)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.OnDeath.AddListener(HandlePlayerDeath);
            }
        }
        
        // Hide UI panels
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        
        // Ensure game is running
        Time.timeScale = 1f;
        isGameOver = false;
        isVictory = false;
        
        OnGameStart?.Invoke();
    }
    
    private void Update()
    {
        // Quick restart with R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        
        // Quit with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
    
    /// <summary>
    /// Handle player death
    /// </summary>
    public void HandlePlayerDeath()
    {
        if (isGameOver || isVictory) return;
        
        isGameOver = true;
        
        // Show game over UI
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        OnGameOver?.Invoke();
        
        Debug.Log("[GameManager] Game Over!");
    }
    
    /// <summary>
    /// Handle victory condition
    /// </summary>
    public void HandleVictory()
    {
        if (isGameOver || isVictory) return;
        
        isVictory = true;
        
        // Show victory UI
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
        
        OnVictory?.Invoke();
        
        Debug.Log("[GameManager] Victory!");
    }
    
    /// <summary>
    /// Restart the current level
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        OnRestart?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    /// <summary>
    /// Respawn player at spawn point
    /// </summary>
    public void RespawnPlayer()
    {
        if (player == null || spawnPoint == null) return;
        
        player.transform.position = spawnPoint.position;
        
        // Reset player state
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.ResetHealth();
        }
        
        MaskSystem mask = player.GetComponent<MaskSystem>();
        if (mask != null)
        {
            mask.SetMask(MaskType.Runner);
        }
        
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.ResetModifiers();
        }
    }
    
    /// <summary>
    /// Load a specific scene
    /// </summary>
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
    
    private void OnDestroy()
    {
        // Cleanup singleton
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
