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

// We need a custom UnityEvent for passing on the 
// ImageTargets transform reference
//[Serializable]
//public class TransformEvent : UnityEvent<Transform> { }

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom event handler behavior, consider inheriting from this class instead.
/// </summary>
public class OwheoObserverEventHandler : DefaultObserverEventHandler
{
    /// <summary>
    /// These three events represent transforms to apply to a prefab on the specified event happening such as the target being found, tracked or lost.
    /// </summary>
    public TransformEvent onTargetFound;
    public TransformEvent whileTargetTracked;
    public TransformEvent onTargetLost;

    //public GameObject toSpawn; // the prefab to spawn on the target

    Coroutine whileTracked = null;

    protected override void OnTrackingFound()
    {
        //Debug.Log($"Target Position : {transform.position} | Rotation : {transform.rotation}"); // print the detected image targets transform


        ObserverManager.instance.Found(this.transform.gameObject, () =>
        {
            if (mObserverBehaviour) SetComponentsEnabled(true); 
            OnTargetFound?.Invoke();

            //base.OnTrackingFound(); // call the base class's OnTrackingFound method

            onTargetFound.Invoke(transform);

            if(whileTracked != null) StopCoroutine(whileTracked);
            whileTracked = StartCoroutine(WhileTracked());
        });

    }

    // When tracking is lost depends on what you have set the status filter as in the Editor.  TRACKING, TRACKING_EXTENDED Tracked etc
    protected override void OnTrackingLost()
    {
        ObserverManager.instance.Lost(this.transform.gameObject, () =>
        {
            if (mObserverBehaviour) SetComponentsEnabled(false); // set the child components to false if the tracking is lost for more than whatever disableTimer is
            OnTargetLost?.Invoke();

            //base.OnTrackingLost();

            onTargetLost.Invoke(transform);
            if(whileTracked != null) StopCoroutine(whileTracked); // 
        });


    }

    /// <summary>
    /// This Function runs while the Target is being tracked and constantly runs the method specified in the editor under the WhileTargetTracked in the ObserverHandler
    /// Only relevant for "persistent" prefabs, as in when I want to spawn a prefab on a target and then leave it there. 
    /// </summary>
    /// <returns></returns>
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