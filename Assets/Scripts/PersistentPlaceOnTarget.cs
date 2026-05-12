using UnityEngine;

public class PersistentPlaceOnTarget : MonoBehaviour
{
    // In the Inspector configure
    // if this object should be enabled or disabled at start
    //public bool startEnabled;

    //private GameObject ImageTargets;
    //private GameObject ModelTargets;

    //private void Awake()
    //{
    //    //gameObject.SetActive(startEnabled);

    //    ImageTargets = GameObject.Find("ImageTargets");
    //    ModelTargets = GameObject.Find("ModelTargets");
    //}

    public void UpdatePosition(Transform imageTarget)
    {
        //Debug.Log($"Prefab Position : {transform.position} | Rotation : {transform.rotation}"); // print the detected image targets transform

        transform.position = imageTarget.position;
        transform.rotation = imageTarget.rotation;

        //gameObject.SetActive(true);
    }

    //public void DisableOther(Transform imageTarget)
    //{
    //    GameObject parent = imageTarget.transform.parent.gameObject;

    //    if (parent.name == ImageTargets.name) 
    //    {
    //        ModelTargets.SetActive(false);
    //    } else
    //    {
    //        ImageTargets.SetActive(true);
    //    }
    //}

    //public void 
}
