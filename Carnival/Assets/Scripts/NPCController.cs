using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("How close the player needs to be to interact")]
    public float interactionRange = 3f;
    
    [Tooltip("GameObject with the dialog UI elements")]
    public GameObject dialogUI;
    
    [Tooltip("Key to press for interaction")]
    public KeyCode interactionKey = KeyCode.F;
    
    [Header("Visual Indicator")]
    [Tooltip("Visual indicator that shows when player is in range")]
    public GameObject interactionIndicator;
    
    [Header("Animation")]
    [Tooltip("Reference to the Animator component")]
    public Animator animator;
    
    [Header("Dialog")]
    [TextArea(3, 5)]
    public string[] dialogLines;
    
    [Header("Options")]
    [Tooltip("Whether the NPC should rotate to face the player")]
    public bool shouldRotateToPlayer = false;
    
    // References
    private Transform playerTransform;
    private DialogSystem dialogSystem;
    private bool playerInRange = false;
    private bool hasInteracted = false; // Track if player has already interacted
    private bool isWaving = false; // Track if the NPC is currently waving
    private Vector3 originalRotation; // Store original rotation
    
    void Start()
    {
        // Store original rotation
        originalRotation = transform.eulerAngles;
        
        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure your player has the 'Player' tag.");
        }
        
        // Get the dialog system
        dialogSystem = FindObjectOfType<DialogSystem>();
        if (dialogSystem == null)
        {
            Debug.LogError("DialogSystem not found in scene! Make sure to add it.");
        }
        
        // Disable the interaction indicator at start
        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(false);
        }
        
        // Ensure dialog UI is initially hidden
        if (dialogUI != null)
        {
            dialogUI.SetActive(false);
        }
        
        // Auto-find animator if not set
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                // Try to find in children
                animator = GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    Debug.LogError("No Animator found on this GameObject or its children!");
                }
            }
        }
    }
    
    void Update()
    {
        // Check if player is in range
        CheckPlayerRange();
        
        // Handle interaction input
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            Debug.Log("Player pressed F key near NPC");
            TriggerDialog();
        }
    }
    
    void CheckPlayerRange()
    {
        // Skip if no player reference
        if (playerTransform == null) return;
        
        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        // Check if player is within interaction range
        bool inRange = distanceToPlayer <= interactionRange;
        
        // If the player's range status changed
        if (inRange != playerInRange)
        {
            playerInRange = inRange;
            Debug.Log("Player in range changed: " + playerInRange);
            
            // Show/hide interaction indicator (only if player hasn't interacted yet)
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(playerInRange && !hasInteracted);
            }
            
            if (playerInRange)
            {
                // Look at player but keep y position the same - ONLY if rotation is enabled
                if (!hasInteracted && shouldRotateToPlayer)
                {
                    LookAtPlayer();
                }
                
                // Start waving when player enters range and hasn't interacted yet
                if (!hasInteracted && !isWaving && animator != null)
                {
                    StartWaving();
                }
            }
            else
            {
                // Player left range, stop waving if they haven't interacted yet
                if (!hasInteracted && isWaving && animator != null)
                {
                    StopWaving();
                }
                
                // Return to original rotation when player leaves range (if rotation is enabled)
                if (!hasInteracted && shouldRotateToPlayer)
                {
                    ReturnToOriginalRotation();
                }
            }
        }
    }
    
    void LookAtPlayer()
    {
        // Look at player but keep y position the same
        Vector3 targetPosition = new Vector3(
            playerTransform.position.x,
            transform.position.y,
            playerTransform.position.z
        );
        
        // Calculate direction to player
        Vector3 directionToPlayer = targetPosition - transform.position;
        directionToPlayer.y = 0; // Keep on same vertical plane
        
        // Only rotate if we have a valid direction
        if (directionToPlayer != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }
    }
    
    void ReturnToOriginalRotation()
    {
        transform.eulerAngles = originalRotation;
    }
    
    void TriggerDialog()
    {
        if (dialogSystem != null && dialogLines.Length > 0)
        {
            Debug.Log("Triggering dialog with " + dialogLines.Length + " lines");
            
            // Mark that player has interacted
            hasInteracted = true;
            
            // Stop waving animation
            if (isWaving && animator != null)
            {
                StopWaving();
            }
            
            // Send the dialog lines to the dialog system
            dialogSystem.ShowDialog(dialogLines);
            
            // Hide interaction indicator while dialog is active
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Dialog system or dialog lines not set up properly!");
            if (dialogSystem == null) Debug.LogError("DialogSystem is null");
            if (dialogLines == null || dialogLines.Length == 0) Debug.LogError("No dialog lines defined");
        }
    }
    
    // Called from DialogSystem when dialog is closed
    public void OnDialogClosed()
    {
        Debug.Log("Dialog closed callback received");
    }
    
    // Helper methods for waving animation
    private void StartWaving()
    {
        Debug.Log("Starting wave animation");
        try {
            animator.SetBool("Waving", true);
            isWaving = true;
        }
        catch (System.Exception e) {
            Debug.LogError("Error setting Waving parameter: " + e.Message);
        }
    }
    
    private void StopWaving()
    {
        Debug.Log("Stopping wave animation");
        try {
            animator.SetBool("Waving", false);
            isWaving = false;
        }
        catch (System.Exception e) {
            Debug.LogError("Error clearing Waving parameter: " + e.Message);
        }
    }
    
    // Public method to reset interaction state (call this if you want to enable re-interaction)
    public void ResetInteraction()
    {
        hasInteracted = false;
        
        // Update indicator visibility based on player range
        if (interactionIndicator != null && playerInRange)
        {
            interactionIndicator.SetActive(true);
            
            // Start waving again if player is in range
            if (!isWaving && animator != null)
            {
                StartWaving();
            }
        }
    }
    
    // Visualize the interaction range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}