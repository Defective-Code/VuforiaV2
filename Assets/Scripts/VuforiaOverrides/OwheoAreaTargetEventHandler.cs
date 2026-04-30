/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.
 
Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/

using UnityEngine;
using Vuforia;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// A custom handler that inherits from the DefaultObserverEventHandler class.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom area target event handler behavior, consider inheriting from this class instead.
/// </summary>
public class OwheoAreaTargetEventHandler : DefaultAreaTargetEventHandler
{
    //public float disableTimer = 10f;

    //bool countingDown = false; // tracks whether we are currently counting down the tracking
    //Coroutine countdown;

    //private float timerCount = 0f;

    //public GameObject trackingPanel;
    //Slider trackingBar;
    //TMP_Text trackingStatus; // text label to inform user if tracking has been lost

    //public ObserverManager om;

    //private bool firstLoad = true;

    //private bool isSceneChanging = false;

    // Checks to see if the scene is being changed so none of the observer logic attempts to fire after switching the scene
    //void OnEnable()
    //{
    //    SceneManager.activeSceneChanged += OnSceneChanged;
    //}

    //void OnDisable()
    //{
    //    SceneManager.activeSceneChanged -= OnSceneChanged;
    //}

    //void OnSceneChanged(Scene oldScene, Scene newScene)
    //{
    //    isSceneChanging = true;
    //}

    //void Start()
    //{
    //    trackingBar = trackingPanel.transform.GetChild(0);
    //    trackingStatus = trackingPanel.transform.GetChild(1);
    //}


    protected override void OnTrackingFound()
    {
        //// if this observer is currently counting down and the stored target is the target of this observer, then we have restablished tracking of the same target and we stop the timer
        //if (countingDown && ObserverManager.instance.currentTargetName == mObserverBehaviour.TargetName)
        //{
        //    ResetCoroutine();// stop a currently running countdown if we reestablish tracking
        //}
        //// If the currently stored target has a different name from what this observer is tracking (the target this observer is attached to) then we want to disable the UI of the countdown
        //else if (ObserverManager.instance.currentTargetName != mObserverBehaviour.TargetName)
        //{
        //    Debug.Log("Target has changed");
        //    Disable();
        //}

        //ToggleUIElements(false);

        //trackingStatus.enabled = true;
        //trackingStatus.text = "Tracking was found";

        ObserverManager.instance.Found(mObserverBehaviour.TargetName, () =>
        {
            SetAugmentationRendering(true); // set the child components to false if the tracking is lost
            OnTargetFound?.Invoke();
        });

        //SetAugmentationRendering(true);
        //OnTargetFound?.Invoke();
    }

    // When tracking is lost depends on what you have set the status filter as in the Editor.  TRACKING, TRACKING_EXTENDED Tracked etc
    protected override void OnTrackingLost()
    {

        //if (isSceneChanging) return; // check if the scene is being changed and stop the execution of any of this

        //if (!firstLoad)
        //{
        //    //SetAugmentationRendering(true);
        //    trackingStatus.text = "Tracking was lost, please scan the device around to reestablish tracking";

        //    ToggleUIElements(true);

        //    countingDown = true;
        //    countdown = StartCoroutine(DisableAfterTimer());
        //    //if (mObserverBehaviour.Status == TargetStatus. )
        //}
        //else
        //{
        //    firstLoad = false;
        //    Disable();
        //}

        ObserverManager.instance.Lost(mObserverBehaviour.TargetName, () =>
        {
            SetAugmentationRendering(false); // set the child components to false if the tracking is lost
            OnTargetLost?.Invoke();
        });
    }

    public void SetAugmentationRendering(bool value)
    {
        for (var i = 0; i < transform.childCount; i++)
            SetEnabledOnChildComponents(transform.GetChild(i), value);
        SetVuforiaRenderingComponents(value);
    }

    void SetEnabledOnChildComponents(Transform augmentationTransform, bool value)
    {
        var augmentationRenderer = augmentationTransform.GetComponent<VuforiaAugmentationRenderer>();
        if (augmentationRenderer != null)
        {
            augmentationRenderer.SetActive(value);
            return;
        }

        if (mObserverBehaviour)
        {
            var rendererComponent = augmentationTransform.GetComponent<Renderer>();
            if (rendererComponent != null)
                rendererComponent.enabled = value;
            var canvasComponent = augmentationTransform.GetComponent<Canvas>();
            if (canvasComponent != null)
                canvasComponent.enabled = value;
            var colliderComponent = augmentationTransform.GetComponent<Collider>();
            if (colliderComponent != null)
                colliderComponent.enabled = value;
        }

        for (var i = 0; i < augmentationTransform.childCount; i++)
            SetEnabledOnChildComponents(augmentationTransform.GetChild(i), value);
    }

    void SetVuforiaRenderingComponents(bool value)
    {
        var augmentationRendererComponents = mObserverBehaviour.GetComponentsInChildren<VuforiaAugmentationRenderer>(false);
        foreach (var component in augmentationRendererComponents)
            component.SetActive(value);
    }

    //IEnumerator DisableAfterTimer()
    //{
    //    while (timerCount < disableTimer)
    //    {
    //        timerCount += Time.deltaTime;
    //        //trackingStatus.text = $"Tracking was lost, please scan the device around to reestablish tracking : {disableTimer - timerCount}";
    //        trackingBar.value = Mathf.Clamp01((disableTimer - timerCount) * (1f / disableTimer));
    //        yield return null;
    //    }

    //    //timerCount = 0f;

    //    ToggleUIElements(false);

    //    SetAugmentationRendering(false); // set the render to false if the tracking is lost for more than whatever disableTimer is
    //    OnTargetLost?.Invoke();

    //    ResetCoroutine(); // once the timer has completed we want to reset everything
    //}

    //void Disable()
    //{
    //    ToggleUIElements(false);

    //    SetAugmentationRendering(false); // set the child components to false if the tracking is lost
    //    OnTargetLost?.Invoke();

    //    ResetCoroutine();
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
    //    trackingPanel.SetActive(toggle);
    //}




}
