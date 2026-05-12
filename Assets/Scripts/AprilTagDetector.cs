using UnityEngine;
using System;

public class AprilTagDetector : MonoBehaviour
{
    AprilTag.TagDetector detector;

    RenderTexture rt;
    Texture2D tex;

    Camera mainCamera;

    float tagSize = 0.02f;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("No main camera found");
            enabled = false;
            return;
        }

        int width = 512;
        int height = 512;

        detector = new AprilTag.TagDetector(width, height, 1);

        rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);

        tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Debug.Log("Startup complete");
    }

    void Update()
    {
        // Temporarily render into RT
        mainCamera.targetTexture = rt;
        mainCamera.Render();
        mainCamera.targetTexture = null;

        RenderTexture.active = rt;

        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        var nativeArray = tex.GetRawTextureData<Color32>();

        ReadOnlySpan<Color32> span = nativeArray.AsReadOnlySpan();

        float fov = mainCamera.fieldOfView * Mathf.Deg2Rad;

        detector.ProcessImage(span, fov, tagSize);

        foreach (var tag in detector.DetectedTags)
        {
            Debug.Log($"Tag {tag.ID}");
        }

        RenderTexture.active = null;
    }
}