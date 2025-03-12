using Unity.Mathematics;
using UnityEngine;

public class Stick : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public GameObject dart;
    public float throwwidth;
    public float throwdepth;

    void Start()
    {
        
    }

    Rigidbody rb;
    private bool stick = false;
    // Update is called once per frame
    void Update()
    {
        Vector3 bar = dart.transform.position;
        if ((bar.x <= throwwidth) && (bar.x >= -1*throwwidth)) {
            if ((bar.z >= 4.75f)&& (bar.z <= throwdepth)){
                if (!stick){
                    stick = true;
                    //Debug.Log("CALLING:" + bar);
                    rb = GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.linearVelocity = Vector3.zero;
                }
            }
        }
        
    }


   
}