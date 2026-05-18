using UnityEngine;

public class SpawnVisualizer : MonoBehaviour
{
    //public Vector3 spawnPosition;
    //public float radius = 1f;

    public Mesh toDraw;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        //int scale = 0;

        Gizmos.DrawMesh(toDraw, position, rotation);
    }
}   