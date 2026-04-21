using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Vuforia;
using TMPro;

public class InstanceManager : MonoBehaviour
{
    GameObject ground;
    GameObject firstFloor;
    GameObject secondFloor;

    public bool gps;
    RetrieveLocationData gpsInstance;
    Vector3 separateCoordinates = new Vector3(-45.867010f, 170.518175f, 0f);

    public ObserverBehaviour posterBoardObserver;
    bool hasCaptured = false;

    public Transform arCamera;
    private Vector3 startPosition;

    private bool groundDisabled; //checks status of ground

    public bool debug;
    public TMP_Text flipstatus;
    public TMP_Text debugText;

    public float disableHeight = 2.0f;
    public float reenableHeight = 1.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // check what half is currently enabled
        if (debug)
        {
            flipstatus.enabled = true;
            flipstatus.text = $"{groundDisabled}";
        }

        ground = transform.GetChild(2).gameObject;
        firstFloor = transform.GetChild(3).gameObject;
        secondFloor = transform.GetChild(4).gameObject;

        // if gps is toggled we want to use the gps enable version, otherwise we track the status of the stairwell as a naive approach
        if (gps)
        {
            gpsInstance = RetrieveLocationData.Instance;

            StartCoroutine(CheckHalf());
        } 

    }

    void Update()
    {
        var status = posterBoardObserver.TargetStatus.Status;

        // Capture the position of the camera when the scultpure area target is tracked
        if (!hasCaptured && (status == Status.TRACKED || status == Status.EXTENDED_TRACKED))
        {
            startPosition = arCamera.position;
            hasCaptured = true;

            Debug.Log("Captured start position: " + startPosition);
        }

        // Now compare the current height of the device with the position it was in at when it detected the area target
        if (!hasCaptured) return;

        Vector3 delta = arCamera.position - startPosition;
        float verticalHeight = Vector3.Dot(delta, Vector3.up);

        if (debug)
        {
            flipstatus.text = $"{groundDisabled}";
            debugText.text = $"{verticalHeight}";
        }

        if (!groundDisabled && verticalHeight > (disableHeight - 0.2f))
        {
            Debug.Log("Reached disable threshold");

            //FlipActive(true);
            DisableGround();
        }

        if (groundDisabled && verticalHeight < (reenableHeight + 0.2f))
        {
            Debug.Log("Reached re-enable threshold");
            //FlipActive(false);
            EnableGround();
        }
    }

    IEnumerator CheckHalf()
    {

        // check if device location is "behind" doorway, meaning we are inside the building, less than because it goes more negative in this specific case
        if (gpsInstance.latitude < separateCoordinates[0])
        {
            //FlipActive(true);
            DisableGround();
        }
        else
        {
            //FlipActive(false);
            EnableGround();
        }
        yield return new WaitForSeconds(1); //pause for one sec inbetween
    }

    //// true to enable second half, false to enable first
    //private void FlipActive(bool flipWay)
    //{
    //    ground.SetActive(!flipWay);
    //    secondFloor.SetActive(flipWay);
    //    flip = flipWay;

    //    if (debugFlip) flipstatus.text = $"{flip}";
    //}

    void DisableGround()
    {
        Debug.Log("Disabling ground");

        ground.SetActive(false);
        secondFloor.SetActive(true);

        groundDisabled = true;
    }

    void EnableGround()
    {
        Debug.Log("Enabling ground");
        ground.SetActive(true);
        secondFloor.SetActive(false);

        groundDisabled = false;
        hasCaptured = false;
    }


    //private void OnTrackableStatusChanged(ObserverBehaviour observerbehavour, TargetStatus status)
    //{
    //    // if we detect the stairway AreaTarget at all, then we flip which half is active. However this is a one time thing
    //    if (status.Status == Status.LIMITED ||
    //        status.Status == Status.TRACKED ||
    //        status.Status == Status.EXTENDED_TRACKED)
    //    {
    //        FlipActive(true);
    //    }
    //}
}
