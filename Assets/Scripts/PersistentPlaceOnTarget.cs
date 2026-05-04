using UnityEngine;

public class PersistentPlaceOnTarget : MonoBehaviour
{
    // In the Inspector configure
    // if this object should be enabled or disabled at start
    public bool startEnabled;

    private void Awake()
    {
        gameObject.SetActive(startEnabled);
    }

    public void UpdatePosition(Transform imageTarget)
    {
        transform.position = imageTarget.position;
        transform.rotation = imageTarget.rotation;

        gameObject.SetActive(true);
    }
}
