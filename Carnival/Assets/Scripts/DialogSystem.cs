using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    // Static property to let other scripts know if dialog is active
    public static bool IsDialogActive { get; private set; }
    [Header("UI References")]
    [Tooltip("The main dialog panel that contains all UI elements")]
    public GameObject dialogPanel;
    
    [Tooltip("The Text component to display dialog")]
    public TMP_Text dialogText;
    
    [Tooltip("Button to advance to the next line")]
    public Button continueButton;
    
    [Tooltip("Text on the continue button")]
    public TMP_Text continueButtonText;
    
    [Header("Dialog Settings")]
    [Tooltip("How fast the text appears (characters per second)")]
    public float textSpeed = 50f;
    
    [Tooltip("Delay after dialog is completed before disappearing")]
    public float closeDelay = 1.5f;
    
    [Tooltip("Sound to play when characters appear")]
    public AudioClip typingSound;
    
    [Tooltip("How often to play the typing sound (in characters)")]
    public int typingSoundFrequency = 5;
    
    // Private variables
    private string[] currentLines;
    private int currentLineIndex;
    private AudioSource audioSource;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private NPCController currentNPC;
    private PauseMenu pauseMenu;
    
    void Start()
    {
        // Set up audio source if needed
        if (typingSound != null && GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        // Get pause menu reference
        pauseMenu = FindObjectOfType<PauseMenu>();
        
        // Initialize dialog panel as inactive
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
        
        // Add click event to the continue button
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueDialog);
        }
    }
    
    public void ShowDialog(string[] lines)
    {
        // Don't start dialog if game is paused
        if (pauseMenu != null && PauseMenu.IsPaused)
            return;
            
        // Set dialog active flag
        IsDialogActive = true;
        
        // Set the current NPC
        currentNPC = FindObjectOfType<NPCController>();
        
        // Store dialog lines and reset index
        currentLines = lines;
        currentLineIndex = 0;
        
        // Activate dialog panel
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(true);
            
            // Show cursor and unlock it
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // Start displaying the first line
            DisplayLine(currentLines[currentLineIndex]);
            
            // Update continue button text
            UpdateContinueButtonText();
        }
    }
    
    void DisplayLine(string line)
    {
        // Cancel any existing typing coroutine
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        // Start new typing coroutine
        typingCoroutine = StartCoroutine(TypeLine(line));
    }
    
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        
        // Clear the text first
        dialogText.text = "";
        
        // Type each character one by one
        int charCount = 0;
        foreach (char c in line.ToCharArray())
        {
            dialogText.text += c;
            charCount++;
            
            // Play sound at intervals
            if (typingSound != null && audioSource != null && charCount % typingSoundFrequency == 0)
            {
                audioSource.PlayOneShot(typingSound, 0.5f);
            }
            
            // Wait for the next character
            yield return new WaitForSeconds(1f / textSpeed);
        }
        
        isTyping = false;
    }
    
    public void ContinueDialog()
    {
        // If currently typing, finish the line immediately
        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            dialogText.text = currentLines[currentLineIndex];
            isTyping = false;
            UpdateContinueButtonText();
            return;
        }
        
        // Move to the next line
        currentLineIndex++;
        
        // Check if we've reached the end of the dialog
        if (currentLineIndex >= currentLines.Length)
        {
            // Close the dialog after a delay
            StartCoroutine(CloseDialogAfterDelay());
        }
        else
        {
            // Display the next line
            DisplayLine(currentLines[currentLineIndex]);
            
            // Update continue button text
            UpdateContinueButtonText();
        }
    }
    
    void UpdateContinueButtonText()
    {
        if (continueButtonText != null)
        {
            // Set button text based on whether it's the last line
            bool isLastLine = currentLineIndex >= currentLines.Length - 1;
            continueButtonText.text = isLastLine ? "Close" : "Continue";
        }
    }
    
    IEnumerator CloseDialogAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);
        CloseDialog();
    }
    
    void CloseDialog()
    {
        // Hide the dialog panel
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
        
        // Lock cursor again for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Notify the NPC that the dialog is closed
        if (currentNPC != null)
        {
            currentNPC.OnDialogClosed();
        }
        
        // Clear current lines
        currentLines = null;
        
        // Set dialog inactive flag
        IsDialogActive = false;
    }
    
    void Update()
    {
        // Allow closing dialog with Escape key
        if (Input.GetKeyDown(KeyCode.Escape) && dialogPanel != null && dialogPanel.activeSelf)
        {
            CloseDialog();
        }
        
        // Also allow advancing dialog with 'F' key
        if (Input.GetKeyDown(KeyCode.F) && dialogPanel != null && dialogPanel.activeSelf)
        {
            ContinueDialog();
        }
    }
}