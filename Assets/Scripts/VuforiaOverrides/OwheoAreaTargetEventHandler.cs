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

// We need a custom UnityEvent for passing on the 
// ImageTargets transform reference
//[Serializable]
//public class TransformEvent : UnityEvent<Transform> { }


/// <summary>
/// A custom handler that inherits from the DefaultObserverEventHandler class.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom area target event handler behavior, consider inheriting from this class instead.
/// </summary>
public class OwheoAreaTargetEventHandler : DefaultAreaTargetEventHandler
{
    public TransformEvent onTargetFound;
    public TransformEvent whileTargetTracked;
    public TransformEvent onTargetLost;

    Coroutine whileTracked = null;

    protected override void OnTrackingFound()
    {

        ObserverManager.instance.Found(this.transform.gameObject, () =>
        {
            SetAugmentationRendering(true); // set the child components to false if the tracking is lost
            OnTargetFound?.Invoke();

            onTargetFound.Invoke(transform);

            if (whileTracked != null) StopCoroutine(whileTracked);
            whileTracked = StartCoroutine(WhileTracked());
        });
    }

    // When tracking is lost depends on what you have set the status filter as in the Editor.  TRACKING, TRACKING_EXTENDED Tracked etc
    protected override void OnTrackingLost()
    {


        ObserverManager.instance.Lost(this.transform.gameObject, () =>
        {
            SetAugmentationRendering(false); // set the child components to false if the tracking is lost
            OnTargetLost?.Invoke();

            //base.OnTrackingLost();

            onTargetLost.Invoke(transform);
            if (whileTracked != null) StopCoroutine(whileTracked); // 
        });
    }

    // For more information about Coroutines see
    // https://docs.unity3d.com/Manual/Coroutines.html
    private IEnumerator WhileTracked()
    {
        // looks dangerous but is ok inside a Coroutine 
        // as long as you yield somewhere
        while (true)
        {
            whileTargetTracked.Invoke(transform);
            yield return null;
        }
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


}
