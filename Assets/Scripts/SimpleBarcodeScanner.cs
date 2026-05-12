using UnityEngine;
using Vuforia;

public class SimpleBarcodeScanner : MonoBehaviour
{
    BarcodeBehaviour mBarcodeBehaviour;
    void Start()
    {
        mBarcodeBehaviour = GetComponent<BarcodeBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mBarcodeBehaviour != null && mBarcodeBehaviour.InstanceData != null)
        {
            Debug.Log($"{mBarcodeBehaviour.InstanceData.Text}");
        } 
    }
}
