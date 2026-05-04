using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;

// We need a custom UnityEvent for passing on the 
// ImageTargets transform reference
[Serializable]
public class TransformEvent : UnityEvent<Transform> { }

public class PermanentObserverEventHandler : DefaultObserverEventHandler
{

    public TransformEvent onTargetFound;
    public TransformEvent whileTargetTracked;
    public TransformEvent onTargetLost;

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

        onTargetFound.Invoke(transform);

        StopAllCoroutines();
        StartCoroutine(WhileTracked());
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();

        onTargetLost.Invoke(transform);

        StopAllCoroutines();
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
}
