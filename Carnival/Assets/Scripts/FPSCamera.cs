using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform playerBody;
    public float mouseSensitivity = 100f;
    public bool lockCursor = true;
    
    [Header("Height Settings")]
    public float cameraHeight = 0.7f; // Height from player center
    
    private float xRotation = 0f;
    private float yRotation = 0f; // Added to track horizontal rotation
    private Vector3 cameraOffset;
    
    void Start()
    {
        // Lock cursor to game window and hide it
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        // Set initial offset
        cameraOffset = new Vector3(0, cameraHeight, 0);
        
        // Store initial rotation
        if (playerBody != null)
            yRotation = playerBody.eulerAngles.y;
    }
    
    void LateUpdate()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        // Calculate camera rotation (look up/down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit to prevent over-rotation
        
        // Update horizontal rotation
        yRotation += mouseX;
        
        // Apply rotation to camera for vertical look
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Apply rotation to player body for horizontal look
        if (playerBody != null)
        {
            playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
            
            // Maintain camera position relative to player
            transform.position = playerBody.position + cameraOffset;
        }
        else
        {
            Debug.LogWarning("Player body reference is missing!");
        }
    }
}