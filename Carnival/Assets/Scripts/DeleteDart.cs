using UnityEngine;

public class DeleteDart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public GameObject dart;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        // this is super buggy, so turning it off for now
        // maybe better to delete all darts when balloons are reset
        //Destroy(dart);        
    }
}