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
    
    // References
    private Transform playerTransform;
    private DialogSystem dialogSystem;
    private bool playerInRange = false;
    private bool hasInteracted = false; // Track if player has already interacted
    
    void Start()
    {
        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
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
    }
    
    void Update()
    {
        // Check if player is in range
        CheckPlayerRange();
        
        // Handle interaction input
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
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
            
            // Show/hide interaction indicator (only if player hasn't interacted yet)
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(playerInRange && !hasInteracted);
            }
            
            // Make NPC look at player when in range
            if (playerInRange)
            {
                // Look at player but keep y position the same
                Vector3 targetPosition = new Vector3(
                    playerTransform.position.x,
                    transform.position.y,
                    playerTransform.position.z
                );
                transform.LookAt(targetPosition);
                
                // Trigger "notice" animation
                if (animator != null)
                {
                    animator.SetTrigger("Notice");
                }
            }
        }
    }
    
    void TriggerDialog()
    {
        if (dialogSystem != null && dialogLines.Length > 0)
        {
            // Mark that player has interacted
            hasInteracted = true;
            
            // Send the dialog lines to the dialog system
            dialogSystem.ShowDialog(dialogLines);
            
            // Hide interaction indicator while dialog is active
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(false);
            }
            
            // Play talking animation
            if (animator != null)
            {
                animator.SetBool("Talking", true);
            }
        }
    }
    
    // Called from DialogSystem when dialog is closed
    public void OnDialogClosed()
    {
        // Stop talking animation
        if (animator != null)
        {
            animator.SetBool("Talking", false);
        }
        
        // Show interaction indicator again if player is still in range AND hasn't interacted
        if (playerInRange && !hasInteracted && interactionIndicator != null)
        {
            interactionIndicator.SetActive(true);
        }
    }
    
    // Visualize the interaction range in the editor
    // Public method to reset interaction state (call this if you want to enable re-interaction)
    public void ResetInteraction()
    {
        hasInteracted = false;
        
        // Update indicator visibility based on player range
        if (interactionIndicator != null && playerInRange)
        {
            interactionIndicator.SetActive(true);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}