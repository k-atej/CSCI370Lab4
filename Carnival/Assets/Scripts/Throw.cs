using UnityEngine;
using UnityEngine.InputSystem;

public class Throw : MonoBehaviour
{
    // The dart prefab to instantiate
    public GameObject dartPrefab;
    
    // How fast the darts will be thrown
    public float throwForce = 20f;
    
    // Reference to the camera (will be found automatically if not assigned)
    public Camera mainCamera;
    public GameObject PauseMenu;
    
    // Cooldown to prevent spamming darts
    public float throwCooldown = 0.5f;
    private float lastThrowTime = 0f;

    public TrackDarts trackDarts;
    
    void Start()
    {
        // If no camera was assigned in the inspector, try to find the main camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            
            // If still null, try to find any camera
            if (mainCamera == null)
            {
                Camera[] cameras = FindObjectsOfType<Camera>();
                if (cameras.Length > 0)
                {
                    mainCamera = cameras[0];
                    Debug.Log("Using camera: " + mainCamera.name);
                }
                else
                {
                    Debug.LogError("No cameras found in the scene. Darts won't spawn correctly.");
                }
            }
        }
    }
    
    void Update()
    {
        // Check for left mouse button click and ensure cooldown has passed
        if (Input.GetMouseButtonDown(0) && Time.time > lastThrowTime + throwCooldown)
        {
            if (PauseMenu.activeSelf != true)
            {
                if (trackDarts.getCount() > 0) {
                ThrowDart();
                lastThrowTime = Time.time;
            }

            }
            
            
        }
    }
    
    void ThrowDart()
    {
        // Safety check to make sure we have a camera and dart prefab
        if (mainCamera == null || dartPrefab == null)
        {
            Debug.LogError("Missing camera or dart prefab reference!");
            return;
        }
        
        // Get spawn position and rotation from the camera
        Vector3 spawnPosition = mainCamera.transform.position;
        Quaternion spawnRotation = mainCamera.transform.rotation;
        
        // Instantiate the dart slightly in front of the camera to avoid collision
        Vector3 adjustedPosition = spawnPosition + mainCamera.transform.forward * 0.5f;
        
        // Instantiate the dart
        GameObject dart = Instantiate(dartPrefab, adjustedPosition, spawnRotation);

        // Ensure the dart has a ThrowForce component
        ThrowForce throwForceComponent = dart.GetComponent<ThrowForce>();
        if (throwForceComponent != null)
        {
            throwForceComponent.force = throwForce;
        }
        else
        {
            Debug.LogWarning("Dart prefab doesn't have a ThrowForce component!");
        }

        trackDarts.Increment();

    }
}