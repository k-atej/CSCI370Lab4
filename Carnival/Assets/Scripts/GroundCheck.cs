using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Fall Protection")]
    public float minimumYPosition = -1f; // Adjust based on your world's lowest valid point
    public Vector3 respawnPosition = new Vector3(0, 0.5f, 0); // Position to teleport to if falling
    
    [Header("Collision Assistance")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer = -1; // Default to all layers
    
    private CharacterController controller;
    private Vector3 lastValidPosition;
    private float timeSinceGrounded = 0f;
    private const float MAX_TIME_UNGROUNDED = 0.5f; // Max time to allow being ungrounded before repositioning
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        lastValidPosition = transform.position + new Vector3(0, 0.5f, 0); // Start with a valid position above ground
    }
    
    void Update()
    {
        // Check if we're currently grounded
        bool isGrounded = CheckGrounded();
        
        // If grounded, store current position as last valid position
        if (isGrounded)
        {
            lastValidPosition = transform.position;
            timeSinceGrounded = 0f;
        }
        else
        {
            timeSinceGrounded += Time.deltaTime;
        }
        
        // Emergency cases to handle falling through
        HandleEmergencyCases();
    }
    
    bool CheckGrounded()
    {
        // Extra ground check with raycasting (more reliable than CharacterController.isGrounded)
        if (Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer))
        {
            return true;
        }
        
        return controller.isGrounded;
    }
    
    void HandleEmergencyCases()
    {
        // Case 1: Fallen below minimum Y position
        if (transform.position.y < minimumYPosition)
        {
            Debug.Log("Character fell below minimum Y position. Repositioning...");
            RepositionCharacter(respawnPosition);
            return;
        }
        
        // Case 2: Not grounded for too long and falling (potential fall-through)
        if (timeSinceGrounded > MAX_TIME_UNGROUNDED && controller.velocity.y < -10f)
        {
            // Do an extra check to make sure we're not just jumping
            if (!Physics.Raycast(transform.position, Vector3.down, 5f))
            {
                Debug.Log("Character appears to be falling through. Repositioning...");
                RepositionCharacter(lastValidPosition);
                return;
            }
        }
    }
    
    void RepositionCharacter(Vector3 position)
    {
        // Disable controller temporarily to reposition
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;
    }
}