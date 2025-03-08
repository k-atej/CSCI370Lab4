using UnityEngine;

public class SetBalloons : MonoBehaviour
{

    public GameObject balloon;
    public Vector3 position;
    private bool active = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)){
            if (!active){
                Generate();
            }
            
        }
        
    }

    void Generate(){
        Debug.Log("instantiating balloons!");
        //Instantiate(balloon, position, Quaternion.identity);
        // edit this so that it works, lol
    }


}