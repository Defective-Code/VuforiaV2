using System;
using System.Collections;
using UnityEngine;
using Vuforia;
using TMPro;
using UnityEngine.UI;

// Class to manage the behaviour of the countdown 
public class ObserverManager : MonoBehaviour
{

    //public GameObject countdownPanel;
    //public Slider countdownBar;

    //public float disableTimer = 5f;

    //bool countingDown = false; // tracks whether we are currently counting down the tracking
    //Coroutine countdown;

    //public TMP_Text trackingStatus; // text label to inform user if tracking has been lost

    //private float timerCount = 0f;

    public string currentTargetName { get; set; }

    //public void BeginCountdown(string targetName)
    //{
    //    if (countingDown && targetName == currentTargetName) // if we reestablish with the currently counting down target, we reset the counter
    //    {
    //        ResetCoroutine();
    //    }

    //    ToggleUIElements(false);


    //}

    //// reset all the coroutine stuff
    //void ResetCoroutine()
    //{
    //    if (countingDown) StopCoroutine(countdown);
    //    countingDown = false;
    //    countdown = null;
    //    timerCount = 0f;
    //}

    //void ToggleUIElements(bool toggle)
    //{
    //    trackingStatus.gameObject.SetActive(toggle);
    //    countdownPanel.SetActive(toggle);
    //}

    //IEnumerator DisableAfterTimer()
    //{
    //    while (timerCount < disableTimer)
    //    {
    //        timerCount += Time.deltaTime;
    //        //trackingStatus.text = $"Tracking was lost, please scan the device around to reestablish tracking : {disableTimer - timerCount}";
    //        countdownBar.value = Mathf.Clamp01((5 - timerCount) * 2 / .9f);
    //        yield return null;
    //    }

    //    Debug.Log("Setting False!");
    //    ToggleUIElements(false);

    //    ResetCoroutine(); // once the timer has completed we want to reset everything
    //}
}
