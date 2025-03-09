using System.Numerics;
using UnityEngine;

public class Throw : MonoBehaviour
{
    public GameObject spheres; 
    public float throwForce = 10f;
    public Camera mainCamera;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Left mouse button click
        {
            Throws();
        }
    }

    void Throws()
    {
        UnityEngine.Vector3 trans = mainCamera.transform.position;
        UnityEngine.Quaternion rot = mainCamera.transform.rotation;

        Instantiate(spheres, trans, rot);
    }
}
