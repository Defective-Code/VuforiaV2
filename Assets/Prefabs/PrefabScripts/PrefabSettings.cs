using UnityEngine;
using System.Collections;
using System;
using Vuforia;

public class PrefabSettings : MonoBehaviour
{

    public bool fadeIn = false; // toggle true to make object fade in on first load
    private float fadeSpeed = 0.5f;

    public bool delay = false; // use this to delay the spawning of the associated prefab. This is so tracking can settle and become a bit better before spawning in the prefab

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {

        //var observer = GetComponent<ObserverBehaviour>();
        //observer.OnTargetStatusChanged += OnStatusChanged;


        // if the user wants the object to fade in, then make the object transparent and run the coroutine
        if (fadeIn)
        {

            //StartCoroutine(SetupNextFrame());

            //Debug.Log("Fading it out");
            //MakeObjectTransparent();

            //StartCoroutine(FadeInObject());
            //instance.GetComponent<MonoBehaviour>().StartCoroutine(FadeInObject(instance));
        }

    }

    //void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    //{
    //    if (status.Status == Status.TRACKED)
    //    {
    //        MakeObjectTransparent();
    //    }
    //}


    public void ApplySettings()
    {
        
        //Debug.Log("Enabling");
        GameObject child = transform.GetChild(0).gameObject;
        child.SetActive(true);

        Debug.Log($"On enable {child.transform.position}");

        //Debug.Log("After SetActive: " + child.activeSelf);

        if (fadeIn)
        {
            StartCoroutine(ApplyNextFrame());
        }
    }

    IEnumerator ApplyNextFrame()
    {
        //yield return null; // wait 1 frame
        //Debug.Log("Fading it out");
        MakeObjectTransparent();
        yield return null;
        StartCoroutine(FadeInObject());
    }


    void MakeObjectTransparent()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            Debug.LogError("No Renderer found!");
            return;
        }

        foreach (Material mat in rend.materials)
        {
            if (!mat.HasProperty("_BaseColor"))
            {
                Debug.LogError("No _BaseColor on material");
                continue;
            }

            Color c = mat.GetColor("_BaseColor");
            c.a = 0f;
            mat.SetColor("_BaseColor", c);

            //Debug.Log($"c.a is {c.a}");
        }
    }

    IEnumerator FadeInObject()
    {

        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            Debug.LogError("No Renderer found!");
            yield break;
        }

        foreach (Material mat in rend.materials)
        {
            if (!mat.HasProperty("_BaseColor"))
            {
                Debug.LogError("No _BaseColor on material");
                continue;
            }

            Color c = mat.GetColor("_BaseColor");
            //c.a = 0f;
            //mat.SetColor("_BaseColor", c);

            while (mat.GetColor("_BaseColor").a < 1) 
            {

                //Debug.Log($"c.a is {c.a}");

                yield return null; // this slows the fade in by a frame each iteration of the loop.

                float fadeAmount = c.a + (fadeSpeed * Time.deltaTime);

                c.a = fadeAmount;

                mat.SetColor("_BaseColor", c);
            }
        }

        GameObject child = transform.GetChild(0).gameObject;
        Debug.Log($" After transparent {child.transform.position}");


        //Color objectColor = transform.GetChild(0).GetComponent<Renderer>().material.color;
        //while (transform.GetChild(0).GetComponent<Renderer>().material.color.a < 1)
        //{
        //    float fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

        //    //objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
        //    //this.GetComponent<Renderer>().material.color = objectColor;
        //    objectColor.a = fadeAmount;
        //    transform.GetChild(0).GetComponent<Renderer>().material.color = objectColor;
        //}

    }
}
