using UnityEngine;

public class Pop : MonoBehaviour
{
    public GameObject balloon;
    
    // Optional: Add particle effects or sounds when balloon pops
    [Header("Pop Effects")]
    public GameObject popEffect;
    public AudioClip popSound;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Check if the balloon reference exists
        if (balloon != null)
        {
            // Create pop effect if assigned
            if (popEffect != null)
            {
                Instantiate(popEffect, balloon.transform.position, Quaternion.identity);
            }
            
            // Play pop sound if assigned
            if (popSound != null)
            {
                AudioSource.PlayClipAtPoint(popSound, balloon.transform.position, 1f);
            }
            
            // Destroy the balloon
            Destroy(balloon);
        }
    }
}