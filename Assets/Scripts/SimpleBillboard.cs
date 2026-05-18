using UnityEngine;

public class SimpleBillboard : MonoBehaviour
{
    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        transform.LookAt(cam.transform);
    }
}