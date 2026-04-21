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

/// <summary>
/// A custom handler that inherits from the DefaultObserverEventHandler class.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom area target event handler behavior, consider inheriting from this class instead.
/// </summary>
public class OwheoAreaTargetEventHandler : DefaultAreaTargetEventHandler
{
    public float disableTimer = 10f;

    public TMP_Text trackingStatus; // text label to inform user if tracking has been lost

    bool countingDown = false; // tracks whether we are currently counting down the tracking
    Coroutine countdown;

    private float timerCount = 0f;

    public GameObject countdownPanel;
    public Slider countdownBar;

    public ObserverManager om;

    private bool firstLoad = true;

    protected override void OnTrackingFound()
    {

        if (countingDown && om.currentTargetName == mObserverBehaviour.TargetName)
        {
            ResetCoroutine();// stop a currently running countdown if we reestablish tracking
        }
        else if (om.currentTargetName != mObserverBehaviour.TargetName)
        {
            Debug.Log("Target has changed");
            Disable();
        }

        ToggleUIElements(false);

        //trackingStatus.enabled = true;
        //trackingStatus.text = "Tracking was found";

        SetAugmentationRendering(true);
        OnTargetFound?.Invoke();
    }

    // When tracking is lost depends on what you have set the status filter as in the Editor.  TRACKING, TRACKING_EXTENDED Tracked etc
    protected override void OnTrackingLost()
    {
        if (!firstLoad)
        {
            //SetAugmentationRendering(true);
            trackingStatus.text = "Tracking was lost, please scan the device around to reestablish tracking";

            ToggleUIElements(true);

            countingDown = true;
            countdown = StartCoroutine(DisableAfterTimer());
            //if (mObserverBehaviour.Status == TargetStatus. )
        }
        else
        {
            firstLoad = false;
            Disable();
        }
    }

    public void SetAugmentationRendering(bool value)
    {
        for (var i = 0; i < transform.childCount; i++)
            SetEnabledOnChildComponents(transform.GetChild(i), value);
        SetVuforiaRenderingComponents(value);
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

        //timerCount = 0f;

        ToggleUIElements(false);

        SetAugmentationRendering(false); // set the render to false if the tracking is lost for more than whatever disableTimer is
        OnTargetLost?.Invoke();

        ResetCoroutine(); // once the timer has completed we want to reset everything
    }

    void Disable()
    {
        ToggleUIElements(false);

        SetAugmentationRendering(false); // set the child components to false if the tracking is lost for more than whatever disableTimer is
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
}
