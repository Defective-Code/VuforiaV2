using UnityEngine;

public class SpawnVisualizer : MonoBehaviour
{
    //public Vector3 spawnPosition;
    //public float radius = 1f;

    public Mesh toDraw;

    [SerializeField]
    Vector3 sizeOfTag = new Vector3(0.055f, 0.055f, 0.055f);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation * Quaternion.Euler(-90, 0, 0);
        //transform.Rotate(-90, 0, 0, Space.Self); // rotate is 90degrees anti-clockwise around the x-axis because of quirks with the way the AprilTag is detected, 
        //int scale = 0;

        Gizmos.DrawMesh(toDraw, position, rotation, sizeOfTag/10); // divide by 10 because the default size of a plane in Unity is 10mx10m when the scale is 1,1,1. So if we want a size matching the physical world at scale 1,1,1, we have to divide the visualization scale by 10.
    }
}   