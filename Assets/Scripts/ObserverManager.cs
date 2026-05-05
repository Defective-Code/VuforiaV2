using System;
using System.Collections;
using UnityEngine;
using Vuforia;
using TMPro;
using UnityEngine.UI;

// Class to manage the behaviour of the countdown, singleton design pattern
public class ObserverManager : MonoBehaviour
{

    public static ObserverManager instance { get; private set; }

    string currentTargetName;

    // Properties to manage the timer
    public float disableTimer = 10f;
    private float timerCount = 0f;
    bool countingDown = false; // tracks whether we are currently counting down the tracking
    Coroutine countdown;

    // references to the UI elements
    public GameObject trackingPanel;
    public Slider trackingBar;
    public TMP_Text trackingStatus; // text label to inform user if tracking has been lost

    private GameObject ModelTargets;
    private GameObject ImageTargets;

    public bool isSceneChanging = false; // check for if the scene is changing between outside and inside

    private void Awake()
    {
        // if an instance of me already exists and its not me then destroy myself
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        ImageTargets = GameObject.Find("ImageTargets");
        ModelTargets = GameObject.Find("ModelTargets");
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //trackingBar = trackingPanel.transform.GetChild(0).gameObject.GetComponent<Slider>();
        //trackingStatus = trackingPanel.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
    }

    //public void Found(string targetName, Action onComplete)
    public void Found(GameObject target, Action onComplete)
    {

        string targetName = target.name;

        // here given a target, I want to disable the other set of targets due to positioning issues caused by multple types of targets used together
        GameObject parent = target.transform.parent.gameObject;
        if (parent.name == ImageTargets.name)
        {
            Debug.Log("Disabling Model Targets");
            ModelTargets.SetActive(false);
        } else
        {
            Debug.Log("Disabling Image Targets");
            ImageTargets.SetActive(false);
        }

        // if this observer is currently counting down and the stored target is the target of the observer that sent the message, then we have restablished tracking of the same target and we stop the timer
        if (countingDown && currentTargetName == targetName)
        {
            Disable(onComplete);// stop a currently running countdown if we reestablish tracking
        }
        // If the currently stored target has a different name from what the observer that sent the message is tracking then we want to disable the UI of the countdown as the user has tracked a different target
        else if (currentTargetName != targetName)
        {
            Debug.Log($"Target has changed to {targetName}");
            Disable(onComplete);
        }
        // if neither case triggers than that means this is the first target we are scanning, in which case we disable all the countdown elements as a sanity check
        else
        {
            Disable(onComplete);
        }
        currentTargetName = targetName;

        //ImageTargets.SetActive(true);
        //ModelTargets.SetActive(true);
    }

    public void Lost(GameObject target, Action onComplete)
    {

        Debug.Log("Enabling Targets");
        ImageTargets.SetActive(true);
        ModelTargets.SetActive(true);

        string targetName = target.name;
        //if (!firstLoad)
        // if we lost tracking of the currently observed target and not due to acquiring tracking of a new target, then we want to start the timer
        if (currentTargetName == targetName) 
        {

            Debug.Log($"Tracking of {targetName} was lost, setting true!");

            ToggleUIElements(true);

            trackingStatus.text = "Tracking was lost, please scan the device around to reestablish tracking";


            // if for some reason we are interupting an existing countdown with a new one, we want to reset the currently running one first
            if (countingDown)
            {
                ResetCoroutine();
                ToggleUIElements(false);
            } 
            countingDown = true;
            countdown = StartCoroutine(DisableAfterTimer(onComplete));
                

            //if (mObserverBehaviour.Status == TargetStatus. )
        } 
        // otherwise we want to immediately disable the "lost" target without a timer as we found a new target to track
        else {
            Debug.Log($"Tracking of {targetName} was lost and we found {currentTargetName}, setting true!");

            Disable(onComplete);
        }

        //else
        //{
        //    firstLoad = false;
        //    //Disable(onComplete);
        //}
    }

    // if we are changing scene then we call this method to stop any countdowns
    public void OnSceneChange()
    {
        ResetCoroutine();
    }

    // Coroutine that disables the UI elements after a given amount of time
    private IEnumerator DisableAfterTimer(Action onComplete)
    {
        while (timerCount < disableTimer)
        {
            timerCount += Time.deltaTime;
            //trackingStatus.text = $"Tracking was lost, please scan the device around to reestablish tracking : {disableTimer - timerCount}";
            trackingBar.value = Mathf.Clamp01((disableTimer - timerCount) * (1f / disableTimer));
            yield return null;
        }

        Disable(onComplete);

        
    }

    // onComplete is a function we want to trigger afterward, usually it is a reference to the disable function within the EventHandler classes to disable the child prefabs
    private void Disable(Action onComplete)
    {
        //Debug.Log("Disabling after scene switchy");

        ToggleUIElements(false);

        //SetAugmentationRendering(false); // set the child components to false if the tracking is lost
        //OnTargetLost?.Invoke();

        ResetCoroutine();

        //OnTimerFinished?.Invoke();
        onComplete?.Invoke();
        //yield return null;
    }

    // reset all the coroutine stuff
    private void ResetCoroutine()
    {
        if (countingDown) StopCoroutine(countdown);
        countingDown = false;
        countdown = null;
        timerCount = 0f;
    }

    private void ToggleUIElements(bool toggle)
    {
        trackingStatus.gameObject.SetActive(toggle);
        trackingPanel.SetActive(toggle);
    }
}
