using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SetBalloons : MonoBehaviour
{

    public GameObject balloon;
    public Vector3 position;
    private bool active = false;
    private List<Vector3> positions = new List<Vector3>();

    public TrackDarts track;
    void Start()
    {   // generates list of positions to put balloons at
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
        
  
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)){ // can be changed in the future to be a start button or something
            if (!active){
                Clear();
                Generate();
            }
            
        }
        
    }

    void Generate(){
        Debug.Log("instantiating balloons!");

        int number = positions.Count;
        for (int i = 0; i < number; i++){
            Instantiate(balloon, positions[i], Quaternion.Euler(270,0,0));
        }
        // generates a 9 column, 3 row array of balloons
    }

    void Clear(){ //removes existing balloons and darts
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
    }


}