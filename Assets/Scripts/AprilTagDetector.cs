using UnityEngine;
using System;
using System.Collections;
using Vuforia;

public class AprilTagDetector : MonoBehaviour
{
    AprilTag.TagDetector detector;

    RenderTexture rt;
    Texture2D tex;

    private Texture2D debugTex;

    private Texture2D cameraTexture;
    private Vuforia.Image image;
    private bool vuforiaReady = false;

    private Color32[] colorBuffer;

    [SerializeField]
    private UnityEngine.UI.RawImage debugImage;

    Camera mainCamera;

    float tagSize = 0.02f;

    [SerializeField]
    SerializableDictionary tagObjects;

    TagDrawer _drawer;
    [SerializeField] Material _tagMaterial = null;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("No main camera found");
            enabled = false;
            return;
        }

        //int width = 1620;
        //int height = 1080;

        //detector = new AprilTag.TagDetector(width, height, 4);

        //rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        //tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;

        Debug.Log("Startup complete");
    }

    void OnDestroy()
    {
        if (detector != null)
            detector.Dispose();
    }

    void OnVuforiaStarted()
    {
        StartCoroutine(InitializeCamera());
    }
    
    IEnumerator InitializeCamera()
    {
        yield return null; // wait a frame

        var cameraDevice = VuforiaBehaviour.Instance.CameraDevice;
        //bool success =
        //    VuforiaBehaviour.Instance.CameraDevice
        //        .SetFrameFormat(PixelFormat.RGBA8888, true);
        bool ok =
            cameraDevice.SetFrameFormat(PixelFormat.RGBA8888, true);

        if (!ok)
        {
            ok =
                cameraDevice.SetFrameFormat(PixelFormat.RGB888, true);
        }

        if (!ok)
        {
            ok =
                cameraDevice.SetFrameFormat(PixelFormat.GRAYSCALE, true);
        }

        vuforiaReady = true;
    }

    void Update()
    {
        // check to make sure Vuforia is initialized first
        if (vuforiaReady)
        {
            var cameraDevice = VuforiaBehaviour.Instance.CameraDevice;

            if (cameraDevice.GetCameraImage(PixelFormat.RGBA8888) != null)
            {
                var latestImage = cameraDevice.GetCameraImage(PixelFormat.RGBA8888);
                if (detector == null)
                {
                    detector = new AprilTag.TagDetector(latestImage.Width, latestImage.Height, 4);
                }
                ProcessRGBA(latestImage);
            }
            else if (cameraDevice.GetCameraImage(PixelFormat.RGB888) != null)
            {
                var latestImage = cameraDevice.GetCameraImage(PixelFormat.RGB888);
                if (detector == null)
                {
                    detector = new AprilTag.TagDetector(latestImage.Width, latestImage.Height, 4);
                }
                ProcessRGB(latestImage);
            }
            else if (cameraDevice.GetCameraImage(PixelFormat.GRAYSCALE) != null)
            {
                var latestImage = cameraDevice.GetCameraImage(PixelFormat.GRAYSCALE);
                if (detector == null)
                {
                    detector = new AprilTag.TagDetector(latestImage.Width, latestImage.Height, 4);
                }
                ProcessGray(latestImage);
            }
        }
        
    }

    void ProcessRGBA(Vuforia.Image image)
    {
        //Debug.Log("Processing");

        int width = image.Width;
        int height = image.Height;


        if (tex == null)
        {
            tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        }
        tex.LoadRawTextureData(image.Pixels);
        tex.Apply();

        var nativeArray = tex.GetRawTextureData<Color32>();
        ReadOnlySpan<Color32> span = nativeArray.AsReadOnlySpan();

        ProcessImage(span, image);
    }

    void ProcessRGB(Vuforia.Image image)
    {
        int width = image.Width;
        int height = image.Height;
        int pixelCount = width * height;

        var pixels = image.Pixels;

        if (pixels.Length < pixelCount * 3)
        {
            Debug.LogError("RGB888 buffer too small for expected image size.");
            return;
        }


        //// Debug using UI Element
        //if (tex == null)
        //{
        //    tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        //}
        //tex.LoadRawTextureData(image.Pixels);
        //tex.Apply();
        //debugImage.texture = tex; // debugging the camera texture to make sure we are retrieving them correctly

        
        
        
        Color32[] output = new Color32[pixelCount];

        for (int i = 0, j = 0; i < pixelCount; i++, j += 3)
        {
            output[i] = new Color32(
                pixels[j],       // R
                pixels[j + 1],   // G
                pixels[j + 2],   // B
                255
            );
        }
        
        ProcessImage(output, image);
    }

    void ProcessGray(Vuforia.Image image)
    {
        int width = image.Width;
        int height = image.Height;
        int pixelCount = width * height;

        var pixels = image.Pixels;

        if (pixels.Length < pixelCount)
        {
            Debug.LogError("Grayscale buffer too small for expected image size.");
            return;
        }

        //// Debug using UI Element
        //if (tex == null)
        //{
        //    tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        //}
        //tex.LoadRawTextureData(image.Pixels);
        //tex.Apply();
        //debugImage.texture = tex; // debugging the camera texture to make sure we are retrieving them correctly


        Color32[] output = new Color32[pixelCount];

        for (int i = 0; i < pixelCount; i++)
        {
            byte v = pixels[i];

            output[i] = new Color32(v, v, v, 255);
        }

        ProcessImage(output, image);
    }


    void ProcessImage(ReadOnlySpan<Color32> span, Vuforia.Image image)
    {

        var cameraDevice = VuforiaBehaviour.Instance.CameraDevice;
        //if (tex == null)
        //{
        //    tex = new Texture2D(width, height,
        //                        TextureFormat.RGBA32,
        //                        false);
        //}

        //tex.SetPixels32(colorBuffer);
        //tex.Apply();

        //ReadOnlySpan<Color32> span = input_span;

        //float fov = mainCamera.fieldOfView * Mathf.Deg2Rad;
        float fov = cameraDevice.GetCameraFieldOfViewRads().y; // get vertial FOV

        TestSpan(span, image);

        detector.ProcessImage(span, fov, tagSize);

        foreach (var detected in detector.DetectedTags)
        {
            Debug.Log("Detected!");
            GameObject cube =
                GameObject.CreatePrimitive(PrimitiveType.Cube);

            cube.transform.position = detected.Position;
            cube.transform.rotation = detected.Rotation;
            //cube.transform.
        }
    }

    void TestSpan(ReadOnlySpan<Color32> span, Vuforia.Image image)
    {
        int width = image.Width;
        int height = image.Height;

        Color32[] temp = span.ToArray();
        //Debug.Log(temp.Length);

        if (debugTex == null ||
            debugTex.width != width ||
            debugTex.height != height)
        {
            debugTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        }

        debugTex.SetPixels32(temp);
        debugTex.Apply();
        debugImage.texture = tex;
    }

    //void Update()
    //{
    //    // Temporarily render into RT
    //    mainCamera.targetTexture = rt;
    //    mainCamera.Render();
    //    mainCamera.targetTexture = null;

    //    // Get the rendertexure of the camera feed, convert to a texture and get the span of the raw color information
    //    RenderTexture.active = rt;
    //    tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
    //    tex.Apply();
    //    var nativeArray = tex.GetRawTextureData<Color32>();
    //    ReadOnlySpan<Color32> span = nativeArray.AsReadOnlySpan();



    //    var fov = mainCamera.fieldOfView * Mathf.Deg2Rad;

    //    detector.ProcessImage(span, fov, tagSize);

    //    //ResetToDisabled(); // reset all the tags associated prefabs to disabled

    //    foreach (var tag in detector.DetectedTags)
    //    {
    //        Debug.Log($"{tag.ID} {tag.Position} {tag.Rotation}");



    //        Vector3 localPos = tag.Position;

    //        // OpenCV RH -> Unity LH
    //        localPos.z *= -1;

    //        Vector3 worldPos =
    //            mainCamera.transform.TransformPoint(localPos);

    //        Quaternion worldRot =
    //            mainCamera.transform.rotation * tag.Rotation;

    //        GameObject cube =
    //            GameObject.CreatePrimitive(PrimitiveType.Cube);

    //        cube.transform.position = worldPos;
    //        cube.transform.rotation = worldRot;
    //        cube.transform.localScale = Vector3.one * 0.05f;

    //        //EnablePrefab(tag.ID, tag.Position, tag.Rotation);

    //        GameObject child = tagObjects.Get(tag.ID);
    //        //Debug.Log($"{child.transform.position} {child.transform.rotation}");
    //    }

    //    RenderTexture.active = null;
    //}

    //private void EnablePrefab(int id, Vector3 position, Quaternion rotation)
    //{

    //    // if a given AprilTag id has an associated prefab and it exists then activate it
    //    if (tagObjects.Contains(id))
    //    {
    //        GameObject child = tagObjects.Get(id);

    //        if (child != null)
    //        {
    //            child.SetActive(true);
                
    //            child.transform.position = position;
    //            //child.transform.rotation = rotation;
    //            child.transform.localScale = new Vector3(1, 1, 1);
    //        }
    //    }
    //}

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
    //    foreach(int id in tagObjects.GetKeys())
    //    {
    //        GameObject child = tagObjects.Get(id);
    //        child.SetActive(false);

    //        child.transform.position = new Vector3(0,0,0);
    //        child.transform.rotation = new Quaternion(0,0,0,0);
    //    }

        
    //}
}