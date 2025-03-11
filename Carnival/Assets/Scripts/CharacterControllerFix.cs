using UnityEngine;

// Add this component to your player object that has the CharacterController
public class CharacterControllerFix : MonoBehaviour
{
    [Header("Collision Settings")]
    [Tooltip("A larger skin width helps prevent falling through colliders")]
    public float skinWidth = 0.08f;
    
    [Tooltip("How far to check below the player for ground")]
    public float groundedCheckDistance = 0.2f;
    
    [Tooltip("Layer mask for ground checks")]
    public LayerMask groundLayers = -1; // Default to "Everything"
    
    // Cache component references
    private CharacterController characterController;
    
    void Start()
    {
        // Get the character controller
        characterController = GetComponent<CharacterController>();
        
        if (characterController != null)
        {
            // Apply optimized settings
            characterController.skinWidth = skinWidth;
            
            // Log the settings being applied
            Debug.Log($"CharacterController optimized: Skin Width = {skinWidth}");
        }
        else
        {
            Debug.LogError("No CharacterController found on this GameObject!");
        }
    }
    
    void Update()
    {
        // Extra ground check with raycasting (more reliable than CharacterController.isGrounded)
        bool isActuallyGrounded = Physics.Raycast(
            transform.position + new Vector3(0, 0.1f, 0), // Start slightly above to avoid self-detection
            Vector3.down,
            groundedCheckDistance, 
            groundLayers
        );
        
        // Debug visualization of the ground check
        Debug.DrawRay(
            transform.position + new Vector3(0, 0.1f, 0),
            Vector3.down * groundedCheckDistance,
            isActuallyGrounded ? Color.green : Color.red
        );
        
        // Additional early detection to prevent falling
        PreventFallthrough();
    }
    
    void PreventFallthrough()
    {
        // This extra raycast helps catch falling through BEFORE it happens
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.0f, groundLayers))
        {
            // If we're going to hit ground very soon and moving fast downward
            if (hit.distance < 0.3f && characterController.velocity.y < -10)
            {
                // Debug message to track when this happens
                Debug.Log("Prevented potential fall-through!");
                
                // Manually move the controller to the hit point plus a small offset
                characterController.enabled = false;
                transform.position = new Vector3(
                    transform.position.x,
                    hit.point.y + characterController.height/2,
                    transform.position.z
                );
                characterController.enabled = true;
            }
        }
    }
}