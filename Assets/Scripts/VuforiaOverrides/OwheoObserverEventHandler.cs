/*==============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
==============================================================================*/

using System;
using System.Collections;
using UnityEngine;
using Vuforia;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom event handler behavior, consider inheriting from this class instead.
/// </summary>
public class OwheoObserverEventHandler : DefaultObserverEventHandler
{

    public float disableTimer = 10f;

    bool countingDown = false; // tracks whether we are currently counting down the tracking
    Coroutine countdown;

    public TMP_Text trackingStatus; // text label to inform user if tracking has been lost

    private float timerCount = 0f;

    public GameObject countdownPanel;
    public Slider countdownBar;

    public ObserverManager om;

    private bool firstLoad = true; // variable to check if this is the first time the targets are loaded, because Vuforia falsely "tracks" them on initialization, meaning the tracking lost code executes. 

    private bool isSceneChanging = false; // boolean to check the state of scene switching, resets to false everytime a scene is loaded, then is set to true once the scene is switched

    // Checks to see if the scene is being changed so none of the observer logic attempts to fire after switching the scene
    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        isSceneChanging = true;
    }

    protected override void OnTrackingFound()
    {

        if (countingDown && om.currentTargetName == mObserverBehaviour.TargetName)
        {
            ResetCoroutine();// stop a currently running countdown if we reestablish tracking
        } 
        else if(om.currentTargetName != mObserverBehaviour.TargetName)
        {
            Debug.Log("Target has changed");
            DisableNotAfterTimer();
        }

        ToggleUIElements(false);

        if (mObserverBehaviour) SetComponentsEnabled(true);
        OnTargetFound?.Invoke();
    }

    // When tracking is lost depends on what you have set the status filter as in the Editor.  TRACKING, TRACKING_EXTENDED Tracked etc
    protected override void OnTrackingLost()
    {

        if (isSceneChanging) return; // check if the scene is being changed and stop the execution of any of this

        if (firstLoad)
        {
            Debug.Log("Tracking was lost, setting true!");

            ToggleUIElements(true);

            trackingStatus.text = "Tracking was lost, please scan the device around to reestablish tracking";

            countingDown = true;
            countdown = StartCoroutine(DisableAfterTimer());

            //if (mObserverBehaviour.Status == TargetStatus. )
        }
        else
        {
            firstLoad = false;
            DisableNotAfterTimer();
        }
    }

    IEnumerator DisableAfterTimer()
    {
        while (timerCount < disableTimer)
        {
            timerCount += Time.deltaTime;
            //trackingStatus.text = $"Tracking was lost, please scan the device around to reestablish tracking : {disableTimer - timerCount}";
            countdownBar.value = Mathf.Clamp01((disableTimer - timerCount) * (1f / disableTimer));
            yield return null;
        }

        Debug.Log("Timer Complete, Setting False!");
        ToggleUIElements(false);

        //timerCount = 0f;

        if (mObserverBehaviour)
            SetComponentsEnabled(false); // set the child components to false if the tracking is lost for more than whatever disableTimer is
        OnTargetLost?.Invoke();

        ResetCoroutine(); // once the timer has completed we want to reset everything
    }

    void DisableNotAfterTimer()
    {
        ToggleUIElements(false);

        if (mObserverBehaviour)
            SetComponentsEnabled(false); // set the child components to false if the tracking is lost for more than whatever disableTimer is
        OnTargetLost?.Invoke();

        ResetCoroutine();
    }

    // reset all the coroutine stuff
    void ResetCoroutine()
    {
        if (countingDown) StopCoroutine(countdown);
        countingDown = false;
        countdown = null;
        timerCount = 0f;
    }

    void ToggleUIElements(bool toggle)
    {
        trackingStatus.gameObject.SetActive(toggle);
        countdownPanel.SetActive(toggle);
    }

    public void SetComponentsEnabled(bool enable)
    {
        var components = VuforiaRuntimeUtilities.GetComponentsInChildrenExcluding<Component, DefaultObserverEventHandler>(gameObject);
        foreach (var component in components)
        {
            switch (component)
            {
                case Renderer rendererComponent:
                    rendererComponent.enabled = enable;
                    break;
                case Collider colliderComponent:
                    colliderComponent.enabled = enable;
                    break;
                case Canvas canvasComponent:
                    canvasComponent.enabled = enable;
                    break;
                case RuntimeMeshRenderingBehaviour runtimeMeshComponent:
                    runtimeMeshComponent.enabled = enable;
                    break;
            }
        }
    }

}