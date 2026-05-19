using UnityEngine;
using System.Collections.Generic;

public class AprilTagObjects : MonoBehaviour
{

    [SerializeField]
    SerializableDictionary tagObjects;

    //[SerializeField]
    AprilTagDetector2 aprilTagDetector;

    Queue<AprilTagInfo> detectedTags;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //aprilTagDetector = gameObject;
        aprilTagDetector = gameObject.GetComponent<AprilTagDetector2>();
        if (aprilTagDetector != null)
        {
            detectedTags = aprilTagDetector.tags;
        } else
        {
            Debug.Log("AprilTagDetector was NULL");
        }

    }

    // Update is called once per frame
    void LateUpdate()
    {

        // loop over the collection of detected AprilTags
        //foreach (var detectedTag in aprilTagDetector.tags)
        //{
        //    MovePrefab(detectedTag.id, detectedTag.position, detectedTag.rotation);
        //}
        while (detectedTags.Count > 0)
        {
            var detectedTag = detectedTags.Dequeue();
            MovePrefab(detectedTag.id, detectedTag.position, detectedTag.rotation);
        }
    }

    /// <summary>
    /// Method to move the associated prefabs to the detected AprilTag. 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    private void MovePrefab(int key, Vector3 position, Quaternion rotation)
    {
        Debug.Log($"Tag position {position}");
        Debug.Log($"Tag rotation {rotation}");

        // if a given AprilTag id has an associated prefab and it exists then activate it
        if (tagObjects.Contains(key))
        {
            GameObject target = tagObjects.Get(key);
            GameObject prefab = target.transform.GetChild(0).gameObject;

            if (target != null && prefab != null)
            {

                // saving the position and rotation of the prefab set in the editor
                Vector3 prefabLocalPosition = prefab.transform.localPosition;
                Quaternion prefabLocalRotation = prefab.transform.localRotation;

                //prefab.SetActive(true);

                target.transform.position = position;
                target.transform.rotation = rotation;
                //prefab.transform.localScale = new Vector3(5, 5, 5);

                Debug.Log(target.transform.position);
                Debug.Log(target.transform.rotation);
                Debug.Log($"target scale : {target.transform.localScale}");
                Debug.Log($"prefab scale : {prefab.transform.localScale}");

                //target.transform.Rotate(new Vector3(45, 0, -90), Space.Self);// rotate the object to face "forwards"
            }
        }
    }

    //private void DisablePrefab(int id, Vector3 position, Quaternion rotation)
    //{
    //    if (tagObjects.Contains(id))
    //    {
    //        if (tagObjects.Get(id) != null)
    //        {
    //            tagObjects.Get(id).SetActive(false);
    //        }
    //    }
    //}

    //private void ResetToDisabled()
    //{
    //    foreach (int id in tagObjects.GetKeys())
    //    {
    //        GameObject child = tagObjects.Get(id);
    //        child.SetActive(false);

    //        child.transform.position = new Vector3(0, 0, 0);
    //        child.transform.rotation = new Quaternion(0, 0, 0, 0);
    //    }


    //}
}
