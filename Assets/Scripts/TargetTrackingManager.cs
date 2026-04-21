using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TargetTrackingManager : MonoBehaviour
{
    // Dictionary to hold target names and their tracking status
    private Dictionary<string, bool> targetStatus = new Dictionary<string, bool>();

    bool flip; // this variable represents which "half" is active, either the ground portion or the indoor portion

    void Start()
    {
        // Find all Vuforia Trackables in the scene
        var observers = FindObjectsOfType<ObserverBehaviour>();
        foreach (var observer in observers)
        {
            targetStatus[observer.TargetName] = false; // Initially not tracked

            // Subscribe to tracking events
            observer.OnTargetStatusChanged += OnTrackableStatusChanged;
        }
    }

    private void OnTrackableStatusChanged(ObserverBehaviour observerbehavour, TargetStatus status)
    {
        string name = observerbehavour.TargetName;

        GameObject ground = transform.GetChild(2).gameObject;
        GameObject firstFloor = transform.GetChild(3).gameObject;
        GameObject secondFloor = transform.GetChild(4).gameObject;

        // Update tracking status
        if (status.Status == Status.LIMITED ||
            status.Status == Status.TRACKED ||
            status.Status == Status.EXTENDED_TRACKED)
        {
            targetStatus[name] = true;
            Debug.Log($"Target tracked: {name}");

            // Here I want to control when certain targets are disabled
            if(name == "StairwayTarget")
            {
                ground.SetActive(flip);
                secondFloor.SetActive(!flip);

                flip = !flip;
            }
        }
        else
        {
            targetStatus[name] = false;
            Debug.Log($"Target lost: {name}");
        }
    }

    // Optional: check which targets are currently tracked
    public List<string> GetTrackedTargets()
    {
        List<string> tracked = new List<string>();
        foreach (var kvp in targetStatus)
        {
            if (kvp.Value)
                tracked.Add(kvp.Key);
        }
        return tracked;
    }

    public List<string> GetUntrackedTargets()
    {
        List<string> untracked = new List<string>();
        foreach (var kvp in targetStatus)
        {
            if (!kvp.Value)
                untracked.Add(kvp.Key);
        }
        return untracked;
    }


}