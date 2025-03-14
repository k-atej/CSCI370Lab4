using UnityEngine;

public class KeepActive : MonoBehaviour
{
    void Awake()
    {
        // Ensure this GameObject is active
        gameObject.SetActive(true);
    }
}
