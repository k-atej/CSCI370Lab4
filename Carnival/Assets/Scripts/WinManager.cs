using UnityEngine;
using TMPro;
using System.Collections;

public class WinManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text winCounterText;
    public GameObject winPanel;
    
    [Header("Win Settings")]
    [Tooltip("How long to display the win message before allowing restart")]
    public float winMessageDuration = 3f;
    
    // Win tracking variables
    private int totalWins = 0;
    private bool gameCompleted = false;
    
    // Reference to balloon spawner
    private SetBalloons balloonManager;
    
    // Track the number of balloons at start of round
    private int startingBalloonCount = 0;
    private bool roundActive = false;

    void Start()
    {
        // Find the balloon manager
        balloonManager = FindObjectOfType<SetBalloons>();
        
        if (balloonManager == null)
        {
            Debug.LogError("No SetBalloons component found in the scene!");
        }
        
        // Subscribe to balloon events
        SetBalloons.OnAllBalloonsPopped += HandleAllBalloonsPopped;
        
        // Always reset win counter at start (session-only counter)
        totalWins = 0;
        UpdateWinCounterText();
        
        // Hide win panel at start
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Win Panel not assigned in WinManager!");
        }
        
        // Delay initialization to ensure all components are loaded
        StartCoroutine(DelayedInit());
    }
    
    IEnumerator DelayedInit()
    {
        yield return new WaitForSeconds(0.5f);
        
        // Additional initialization if needed
        if (winCounterText == null)
        {
            Debug.LogError("Win Counter Text not assigned in WinManager!");
            // Try to find it automatically
            winCounterText = GameObject.Find("WinCounterText")?.GetComponent<TMP_Text>();
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        SetBalloons.OnAllBalloonsPopped -= HandleAllBalloonsPopped;
    }
    
    void Update()
    {
        // Check if the round has started (detect balloons being spawned)
        if (!roundActive && balloonManager != null && balloonManager.IsRoundActive())
        {
            StartRound();
        }
        
        // For debugging only
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log($"WinManager status: Wins={totalWins}, Round active={roundActive}, " +
                      $"Game completed={gameCompleted}, Starting balloons={startingBalloonCount}");
        }
    }
    
    void StartRound()
    {
        // Reset state for new round
        roundActive = true;
        gameCompleted = false;
        
        // Count the balloons at the start of the round
        startingBalloonCount = GameObject.FindGameObjectsWithTag("Balloon").Length;
        Debug.Log($"Round started with {startingBalloonCount} balloons");
        
        // Make sure win panel is hidden
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }
    
    // Event handler for when all balloons are popped
    void HandleAllBalloonsPopped()
    {
        Debug.Log("Received all balloons popped event");
        if (roundActive && !gameCompleted)
        {
            RegisterWin();
        }
    }
    
    void RegisterWin()
    {
        // Ensure we only register a win once per round
        if (gameCompleted) return;
        
        Debug.Log("Registering win!");
        gameCompleted = true;
        totalWins++;
        
        // Update UI
        UpdateWinCounterText();
        
        // Show win message
        DisplayWinMessage();
    }
    
    void DisplayWinMessage()
    {
        Debug.Log("Displaying win message");
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            
            // Find any text components in the win panel and update them if needed
            TMP_Text[] texts = winPanel.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text text in texts)
            {
                if (text.name.Contains("WinMessage") || text.name.Contains("Text"))
                {
                    text.text = "You Win!\nTotal Wins: " + totalWins;
                }
            }
            
            // Automatically hide the panel after delay
            StartCoroutine(HideWinPanelAfterDelay());
        }
        else
        {
            Debug.LogError("Win Panel not assigned!");
        }
    }
    
    IEnumerator HideWinPanelAfterDelay()
    {
        yield return new WaitForSeconds(winMessageDuration);
        
        // Hide win panel
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
        
        // Allow new round to be detected
        roundActive = false;
    }
    
    void UpdateWinCounterText()
    {
        if (winCounterText != null)
        {
            winCounterText.text = "Wins: " + totalWins;
            Debug.Log("Updated win counter to: " + totalWins);
        }
        else
        {
            Debug.LogError("Win Counter Text component is null!");
        }
    }
    
    // Public method to reset the win counter
    public void ResetWinCounter()
    {
        totalWins = 0;
        UpdateWinCounterText();
        Debug.Log("Win counter reset to 0");
    }
}