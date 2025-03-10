using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -19.62f;
    
    [Header("References")]
    public Transform cameraTransform; // Reference to the camera transform for direction
    
    // Private variables
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Camera mainCamera;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Ensure there's a CharacterController component
        if (controller == null)
        {
            Debug.LogError("CharacterController component missing on the Player object!");
            controller = gameObject.AddComponent<CharacterController>();
        }
        
        // Get camera reference if not set
        if (cameraTransform == null)
        {
            mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
                Debug.Log("Camera reference auto-assigned");
            }
            else
            {
                Debug.LogError("No camera found! Please assign a camera reference in the inspector.");
            }
        }
        
        // Lock cursor for FPS controls
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        HandleMovement();
        HandleJump();
        ApplyGravity();
    }
    
    void HandleMovement()
    {
        // Check if grounded
        isGrounded = controller.isGrounded;
        
        // Skip if no camera reference
        if (cameraTransform == null) return;
        
        // Get input axis
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        // Get camera forward and right, but ignore Y component for level movement
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        
        // Project vectors onto XZ plane to prevent tilting up/down from affecting movement speed
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        
        // Create camera-relative movement vector
        Vector3 move = right * x + forward * z;
        
        // Normalize the movement vector to prevent faster diagonal movement
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }
        
        // Apply movement, maintaining y velocity
        controller.Move(move * moveSpeed * Time.deltaTime);
    }
    
    void HandleJump()
    {
        // Reset velocity when grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to ensure grounding
        }
        
        // Jump when Space is pressed and player is grounded
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    
    void ApplyGravity()
    {
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        
        // Apply vertical movement
        controller.Move(velocity * Time.deltaTime);
    }
}