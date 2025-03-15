using UnityEngine;

// Attach this to each balloon to track when it's popped
public class BalloonTracker : MonoBehaviour
{
    // Reference to the main balloon manager
    [HideInInspector]
    public SetBalloons balloonManager;
    
    // Flag to ensure we only count each balloon once
    private bool hasBeenPopped = false;
    
    // Called by the Pop.cs script when a collision happens
    void OnDestroy()
    {
        // Only count it if it hasn't been counted yet and the balloon manager exists
        if (!hasBeenPopped && balloonManager != null)
        {
            hasBeenPopped = true;
            balloonManager.BalloonPopped();
        }
    }
}