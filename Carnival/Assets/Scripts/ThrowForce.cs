using UnityEngine;

public class ThrowForce : MonoBehaviour
{
    // Force to apply to the dart
    public float force = 20f;
    
    // Reference to the rigidbody (will be found automatically if not assigned)
    private Rigidbody rigidbody_m;
    
    // Reference to the main camera (will be found automatically if not assigned)
    private Camera mainCam;
    
    void Start()
    {
        // Try to get the Rigidbody component
        rigidbody_m = GetComponent<Rigidbody>();
        
        // If no Rigidbody, add one
        if (rigidbody_m == null)
        {
            Debug.LogWarning("No Rigidbody found on dart, adding one automatically.");
            rigidbody_m = gameObject.AddComponent<Rigidbody>();
            
            // Configure the rigidbody for a dart
            rigidbody_m.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rigidbody_m.useGravity = true;
        }
        
        // Try to find the main camera
        mainCam = Camera.main;
        
        // If no main camera is found, try to find any camera
        if (mainCam == null)
        {
            Debug.LogWarning("No camera tagged as 'MainCamera' found. Trying to find any camera...");
            Camera[] cameras = FindObjectsOfType<Camera>();
            if (cameras.Length > 0)
            {
                mainCam = cameras[0];
                Debug.Log("Using camera: " + mainCam.name);
            }
            else
            {
                Debug.LogError("No cameras found in scene. Dart won't be thrown correctly.");
                return; // Exit early to avoid null reference
            }
        }
        
        // Apply the force
        AddF();
    }
    
    void AddF()
    {
        // Safety check to make sure we have what we need
        if (rigidbody_m != null && mainCam != null)
        {
            // Apply force in the direction the camera is facing
            rigidbody_m.AddForce(mainCam.transform.forward * force, ForceMode.Impulse);
            
            // Add a small amount of torque for realistic dart flight
            rigidbody_m.AddTorque(new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), ForceMode.Impulse);
        }


    }
}