using UnityEngine;

public class PrefabMove : MonoBehaviour
{

    public Vector3 offsetToMoveTo;

    public float speed = 0.1f;

    private int count = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //transform.Rotate(0f, -90, 0f, Space.Self);
    }

    // Update is called once per frame
    void Update()
    {
        //float step = speed * Time.deltaTime;

        //transform.position = Vector3.MoveTowards(transform.position, offsetToMoveTo, step);
        Debug.Log($"{transform.localPosition.x} | {offsetToMoveTo.x}");
        if (transform.localPosition.x*(-1) < offsetToMoveTo.x*(-1))
        {
            transform.localPosition += offsetToMoveTo / 10 * Time.deltaTime;
            count++;
        }

    }
}
