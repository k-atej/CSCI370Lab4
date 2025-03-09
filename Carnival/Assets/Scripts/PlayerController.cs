using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -19.62f;
    
    [Header("References")]
    public Transform cameraHolder; // Reference to the camera holder object
    
    // Private variables
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Ensure there's a CharacterController component
        if (controller == null)
        {
            Debug.LogError("CharacterController component missing on the Player object!");
            controller = gameObject.AddComponent<CharacterController>();
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
        
        // Get input axis
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        // Create movement vector relative to camera direction
        Vector3 move = transform.right * x + transform.forward * z;
        
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