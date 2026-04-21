using UnityEngine;

public class LandscapeParallax : MonoBehaviour
{

    GameObject TestLandscape;
    GameObject mainCamera;
    GameObject cameraAnchor;
    GameObject landscapeCamera;

    Pose original;

    void Awake()
    {
        TestLandscape = GameObject.Find("TestLandscape");
        cameraAnchor = TestLandscape.transform.GetChild(0).gameObject;
        landscapeCamera = cameraAnchor.transform.GetChild(0).gameObject;

        mainCamera = GameObject.Find("ARCamera");

        original = new Pose(landscapeCamera.transform.localPosition, landscapeCamera.transform.localRotation);

        //transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, 0);

    }

    void Start()
    {

        Debug.Log("Created the Window for landscape");
        // =======================
        // OFFSET CONFIG (meters)
        // =======================
        float behindOffset = 0.8f;   // how far behind the pose
        float upOffset = 0.4f;       // how far above the pose
        float x_rotationOffset = 90; // how far to rotate the 


        // Calculate offset position
        //Vector3 spawnPosition =
        //   transform.position
        //   + Vector3.forward * behindOffset   // world backward
        //   + Vector3.up * upOffset;            // world up

        //transform.position = spawnPosition;
        //transform.rotation = new Quaternion(90, 0, 0, 0); // make the prefab be straight up and down
        //transform.Rotate(90f, 0.0f, 0.0f, Space.Self);
    }

    // Update is called once per frame
    void Update()
    {

        //Optional: visualize placement
        Debug.DrawLine(mainCamera.transform.position, transform.position, Color.green, 6f);

        float horizontal_offset = mainCamera.transform.position.x - transform.position.x;
        //float vertical_offset = mainCamera.transform.position.y - transform.position.y;
        float vertical_offset = 0;

        landscapeCamera.transform.localPosition = new Vector3(original.position.x, original.position.y + 10*vertical_offset, original.position.z  + 10 * horizontal_offset); // the camera's "horizontal" ,movement is on the z axis, look at the TestLandscape prefab to see this
   

    }

    void OnDestroy()
    {
        //landscapeCamera.transform.position = cameraAnchor.transform.position;
        landscapeCamera.transform.localPosition = new Vector3(0, 0, 0); // reset the cameras position everytime it is destroyed so the position of the camera is consitent
    }
}
