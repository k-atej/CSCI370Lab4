using System.Collections.Generic;
using UnityEngine;

public class SetBalloons : MonoBehaviour
{
    public GameObject balloon;
    public GameObject PauseMenu;
    public Vector3 position;
    private bool active = false;
    private List<Vector3> positions = new List<Vector3>();
    
    [Header("Game State")]
    [Tooltip("Total number of balloons that will be spawned")]
    public int totalBalloons = 0;
    [Tooltip("Number of balloons that have been popped")]
    public int poppedBalloons = 0;

    public TrackDarts track;
    
    // Event to notify other components when all balloons are popped
    public delegate void AllBalloonsPopped();
    public static event AllBalloonsPopped OnAllBalloonsPopped;
    
    void Start()
    {   
        // Generates list of positions to put balloons at
        int count = 0;
        for (int i = 0; i < 5; i++){
            float tempx = position.x - (0.45f*i);
            for (int j = 0; j < 3; j++){
                float tempy = position.y - 0.42f*j;
                Vector3 temp = new Vector3(tempx, tempy, position.z);
                positions.Add(temp);
                count += 1;
            } 
        }
        for (int i = 1; i < 5; i++){
            float tempx = position.x + (0.45f*i);
            for (int j = 0; j < 3; j++){
                float tempy = position.y - 0.42f*j;
                Vector3 temp = new Vector3(tempx, tempy, position.z);
                positions.Add(temp);
                count += 1;
            } 
        }
        
        // Store the total number of balloon positions
        totalBalloons = positions.Count;
        
        // Initialize game state
        poppedBalloons = 0;
        active = false;
        
        Debug.Log($"SetBalloons initialized with {totalBalloons} possible balloon positions");
    }

    void Update()
    {
        if (PauseMenu.activeSelf != true)
        {
            if (Input.GetKeyDown(KeyCode.E)){ // can be changed in the future to be a start button or something
                if (!active){
                    Debug.Log("Starting new round");
                    Clear();
                    Generate();
                    poppedBalloons = 0;
                    active = true;
                }
            }
        }
    }

    void Generate(){
        Debug.Log("Instantiating balloons!");

        int number = positions.Count;
        for (int i = 0; i < number; i++){
            GameObject newBalloon = Instantiate(balloon, positions[i], Quaternion.Euler(270,0,0));
            
            // Add a BalloonTracker component to each balloon
            BalloonTracker tracker = newBalloon.AddComponent<BalloonTracker>();
            tracker.balloonManager = this;
        }
        
        // Reset the popped balloon counter
        poppedBalloons = 0;
        
        // Set active to true to indicate a round is in progress
        active = true;
    }

    void Clear(){ // Removes existing balloons and darts
        Debug.Log("Clearing balloons and darts");
        
        GameObject[] taggedDarts = GameObject.FindGameObjectsWithTag("Dart");
        foreach (GameObject drt in taggedDarts)
        {
            Destroy(drt);
        }

        GameObject[] taggedBalloons = GameObject.FindGameObjectsWithTag("Balloon");
        foreach (GameObject blln in taggedBalloons)
        {
            Destroy(blln);
        }
        track.Reset();
        active = false;
        poppedBalloons = 0;
    }
    
    // Called by each balloon when it's popped
    public void BalloonPopped()
    {
        // Increment popped balloon counter
        poppedBalloons++;
        Debug.Log("Balloon popped! " + poppedBalloons + "/" + totalBalloons);
        
        // Check if all balloons are popped
        if (poppedBalloons >= totalBalloons)
        {
            Debug.Log("All balloons popped! Game won!");
            
            // Set active to false to indicate round is complete
            active = false;
            
            // Trigger the event for any listeners
            if (OnAllBalloonsPopped != null)
            {
                OnAllBalloonsPopped();
            }
        }
        
        // Double check with balloon count
        int remainingBalloons = GameObject.FindGameObjectsWithTag("Balloon").Length;
        if (remainingBalloons == 0 && active)
        {
            Debug.Log("No balloons remain! Triggering win condition.");
            
            // Set active to false to indicate round is complete
            active = false;
            
            // Trigger the event for any listeners
            if (OnAllBalloonsPopped != null)
            {
                OnAllBalloonsPopped();
            }
        }
    }
    
    // Public method to check if a round is currently active
    public bool IsRoundActive()
    {
        return active;
    }
}