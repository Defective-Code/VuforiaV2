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

    //public ObserverManager om;

    private bool firstLoad = true;

    //public event Action OnTimerFinished;

    //private bool isSceneChanging = false;

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

        //DontDestroyOnLoad(this);
    }

    private void Start()
    {
        //trackingBar = trackingPanel.transform.GetChild(0).gameObject.GetComponent<Slider>();
        //trackingStatus = trackingPanel.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
    }

    public void Found(string targetName, Action onComplete)
    {

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
        else
        {
            Disable(onComplete);
        }
        currentTargetName = targetName;
    }

    public void Lost(string targetName, Action onComplete)
    {
        if (!firstLoad)
        {

            Debug.Log($"Tracking of {targetName} was lost, setting true!");

            ToggleUIElements(true);

            trackingStatus.text = "Tracking was lost, please scan the device around to reestablish tracking";

            countingDown = true;
            countdown = StartCoroutine(DisableAfterTimer(onComplete));

            //if (mObserverBehaviour.Status == TargetStatus. )
        }
        else
        {
            firstLoad = false;
            Disable(onComplete);
        }
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

    private void Disable(Action onComplete)
    {
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
