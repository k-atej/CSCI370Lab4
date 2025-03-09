using Unity.VisualScripting;
using UnityEngine;

public class ThrowForce : MonoBehaviour
{
    Rigidbody rigidbody_m;
    public float force;
    private Camera mainCam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     rigidbody_m = GetComponent<Rigidbody>();   
     mainCam = Camera.main;
     AddF();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddF()
    {
     rigidbody_m.AddForce(mainCam.transform.forward, ForceMode.Impulse);   
    }
}
