using UnityEngine;

// Struct to hold the positional data for a detected AprilTag
[System.Serializable]
public struct AprilTagInfo
{
    public int id;
    public Vector3 position;
    public Quaternion rotation;
    public float tagSize;
}