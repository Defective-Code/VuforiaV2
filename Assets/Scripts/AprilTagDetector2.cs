using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Vuforia;

/// <summary>
/// Class to setup AprilTagDetection to operate alongside Vuforia. Focuseed entirely on detecting and storing the pose of AprilTags. Access the public tags list to access them and do anything further
/// </summary>
public class AprilTagDetector2 : MonoBehaviour
{

    const PixelFormat PIXEL_FORMAT = PixelFormat.RGB888;
    const TextureFormat TEXTURE_FORMAT = TextureFormat.RGB24;

    //Test fields for the Z line
    GameObject debugObject = null;

    private bool isTablet = true;

    private AprilTag.TagDetector detector;

    private bool vuforiaReady = false; // bool to check if Vuforia has been initialized

    /// <summary>
    /// Fields for managing the detection object provided by the package AprilTag by Keijiro. Modify these values in the editor for this class
    /// </summary>
    [SerializeField]
    float tagSize = 0.055f; // the physical size of the tag in meters. the default here is 0.055m, or 55mm. 
    [SerializeField]
    int decimation = 4; // or downsampling. This value controls the resolution used to detect the AprilTags, and is the scale factor we scale the image feed down by. Higher values will run faster, but struggle to detect small or far away tags. Smaller ones run slower but have a better detection rate

    private Vuforia.Image latestImage; // field to store the latest image from the camera;
    private Color32[] colorBuffer; // the array of pixel values that represents the image
    private Color32[] rotatedBuffer = new Color32[1]; // pre-allocated buffer for the rotated version of the color buffer

    [SerializeField]
    private UnityEngine.UI.RawImage debugImage;
    private Texture2D debugTex; // debug texture that stores the re-converted Span of pixels used to detect for AprilTags

    // List of detected tags for use in other areas
    //public List<AprilTagInfo> tags = new List<AprilTagInfo>();
    public Queue<AprilTagInfo> tags = new Queue<AprilTagInfo>();

    //SerializableDictionary tagObjects;

    void Start()
    {

        var mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("No main camera found");
            enabled = false;
            return;
        }

        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;

        Debug.Log("Startup complete");
    }

    void OnDestroy()
    {
        if (detector != null)
            detector.Dispose();
        if (debugTex != null)
            Destroy(debugTex); // add this
    }

    void OnVuforiaStarted()
    {
        StartCoroutine(InitializeCamera());
    }
    
    /// <summary>
    /// Coroutine to setup the Vuforia camera
    /// </summary>
    /// <returns></returns>
    IEnumerator InitializeCamera()
    {
        yield return null; // wait a frame

        //debugTex = new Texture2D(1, 1, TEXTURE_FORMAT, false);

        CameraDevice cameraDevice = VuforiaBehaviour.Instance.CameraDevice;

        //PixelFormat pixelFormat = PixelFormat.RGB888;
        bool success = VuforiaBehaviour.Instance.CameraDevice.SetFrameFormat(PIXEL_FORMAT, true);

        // Vuforia has started, now register camera image format
        if (success)
        {
            Debug.Log("Successfully registered pixel format " + PIXEL_FORMAT.ToString());
        }
        else
        {
            Debug.LogError(
                "Failed to register pixel format " + PIXEL_FORMAT.ToString() +
                "\n the format may be unsupported by your device;" +
                "\n consider using a different pixel format.");
        }

        vuforiaReady = true;
    }

    void Update()
    {
        // check to make sure Vuforia is initialized first
        if (vuforiaReady)
        {

            var cameraDevice = VuforiaBehaviour.Instance.CameraDevice;

            // make sure that the camera image isn't null since it can take a few frames for the image to become available after registering for an image format.
            if (cameraDevice.GetCameraImage(PIXEL_FORMAT) != null)
            {
                

                latestImage = cameraDevice.GetCameraImage(PIXEL_FORMAT);

                int width = latestImage.Width;
                int height = latestImage.Height;

                if (detector == null)
                {
                    switch (isTablet)
                    {
                        case false:
                            detector = new AprilTag.TagDetector(width, height, decimation); // default orientation, as in the intially captured image is either the right way up, or is 180 degrees upside down. 
                            break;
                        case true:
                            detector = new AprilTag.TagDetector(height, width, decimation); // swap the height and width if a roation is applied for different devices, as in if the initial captured camera image is the wrong orientation and needs to be rotated 90degrees some way, then we need to swap the height and length values to match this
                            break;
                    }

                    //detector = new AprilTag.TagDetector(width, height, 4); // default orientation, as in the intially captured image is either the right way up, or is 180 degrees upside down. 
                }

                ProcessRGB(latestImage); // convert the Vuforia image to a Color32 span
                NormalizeBuffer(width, height, isTablet);


                //ReadOnlySpan<Color32> imageSpan = new ReadOnlySpan<Color32>(colorBuffer);
                // Conditionally set imageSpan to either rotatedBuffer or colorBuffer depending on if we have rotated the image
                ReadOnlySpan<Color32> imageSpan = isTablet
                    ? new ReadOnlySpan<Color32>(rotatedBuffer)
                    : new ReadOnlySpan<Color32>(colorBuffer);

                //bool rotated = false; // indicate if the the texture has been rotated. Only important for testing purposes so the output texture displays properly
                TestSpan(imageSpan, latestImage, isTablet); // debug the colorBuffer by converting it back to a texture and outputting to a UI element. Rotated should be true if the image has been rotated

                ProcessImage(imageSpan); // process the converted Vuforia image

            } 
            else
            {
                Debug.Log("GetCameraImage was NULL");
            }
        }
        
    }

    /// <summary>
    /// Method to convert a Vuforia Image taken from the ARCamera into an array of Pixels
    /// </summary>
    /// <param name="image"></param>
    void ProcessRGB(Vuforia.Image image)
    {
        int width = image.Width;
        int height = image.Height;
        int pixelCount = width * height;

        var pixels = image.Pixels;

        if (colorBuffer == null || colorBuffer.Length != pixelCount)
            colorBuffer = new Color32[pixelCount];

        for (int i = 0, j = 0; i < pixelCount; i++, j += 3)
        {
            colorBuffer[i] = new Color32(
                pixels[j],
                pixels[j + 1],
                pixels[j + 2],
                255
            );
        }

        //Debug.Log($"Device Image size: {image.Width}x{image.Height}");

        //Debug.Log($"Span length: {colorBuffer.Length}");
        //Debug.Log($"Expected: {image.Width * image.Height}");
        // Convert the array of Color32 to a readonly array
        //imageBuffer = new ReadOnlySpan<Color32> (colorBuffer);
        //return new ReadOnlySpan<Color32>(colorBuffer);
        //return colorBuffer;

        //ProcessImage(new ReadOnlySpan<Color32>(colorBuffer), image);
    }

    /// <summary>
    /// Method to process an Image taken from the camera in the form of a span of pixels, and then pass it to the detector to see if there are any AprilTags in the image.
    /// </summary>
    /// <param name="span"></param>
    void ProcessImage(ReadOnlySpan<Color32> span)
    {
        var cameraDevice = VuforiaBehaviour.Instance.CameraDevice;

        Vector2 fov = cameraDevice.GetCameraFieldOfViewRads(); // Vector2 of both horizontal and vertical FOV

        // Use fov.x i.e the "horizontal" fov because when we use the tablet, the output is technically 1920x1080, but we rotate it by 90degrees so that its 1080x1920
        // however when we want the now vertical FOV, we must use the "horizontal" fov as it now corresponds to the vertical FOV
        detector.ProcessImage(span, fov.x, tagSize);

        foreach (var detectedTag in detector.DetectedTags)
        {
            Debug.Log("Detected!");

            //if (debugObject == null)
            //{
            //    debugObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    debugObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //}

            //Debug.Log($"Keijiro pos: {detectedTag.Position}");
            //Debug.Log($"Keijiro rot: {detectedTag.Rotation.eulerAngles}");

            Vector3 worldPosition = Camera.main.transform.TransformPoint(detectedTag.Position);
            Quaternion worldRotation = Camera.main.transform.rotation * detectedTag.Rotation;
            //Debug.Log($"World pos: {worldPosition}");

            //Debug.Log($"Device FOV H: {fov.x * Mathf.Rad2Deg}, V: {fov.y * Mathf.Rad2Deg}");

            //Debug.Log($"Z at 10cm: {detectedTag.Position.z}");
            //Debug.Log($"Square pos: {debugObject.transform.position}");
            //debugObject.transform.position = detectedTag.Position;
            //debugObject.transform.rotation = detectedTag.Rotation;

            //tags.Add(new AprilTagInfo { id=detectedTag.ID, position=worldPosition, rotation=worldRotation, tagSize=tagSize }); // save the Unity world position of the detected tags in the list
            tags.Enqueue(new AprilTagInfo { id = detectedTag.ID, position = worldPosition, rotation = worldRotation, tagSize = tagSize });

            //EnablePrefab(detectedTag.ID, worldPosition, worldRotation);

            //EnablePrefab(detectedTag.ID, detectedTag.Position, detectedTag.Rotation);
            //DrawZVector(detectedTag.Position, detectedTag.Rotation);
        }
    }

    /// <summary>
    /// Method to display a given span of pixels, convert it back into a texture and display it on a UI element for visualization and testing purposes
    /// </summary>
    /// <param name="span"></param>
    /// <param name="image"></param>
    /// <param name="rotated"></param>
    void TestSpan(ReadOnlySpan<Color32> span, Vuforia.Image image, bool rotated)
    {
        int width = rotated ? image.Height : image.Width;
        int height = rotated ? image.Width : image.Height;

        if (debugTex == null || debugTex.width != width || debugTex.height != height)
        {
            if (debugTex != null)
                Destroy(debugTex); // release the old GPU texture first

            debugTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        }

        debugTex.SetPixels32(span.ToArray());
        debugTex.Apply();
        debugImage.texture = debugTex;
    }

    /// <summary>
    /// Method to encompass any alterations made to the Colorbuffer
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="tablet"></param>
    void NormalizeBuffer(int width, int height, bool tablet)
    {


        // for the tablet
        if (tablet)
        {
            FlipHorizontal(colorBuffer, width, height);
            Rotate90(colorBuffer, width, height, true); // false for anticlockwise and true for clockwise, this returns the rotated image in the rotatedBuffer field
            //colorBuffer = new Color32[](rotatedBuffer); //Rotate90 updates the rotateBuffer, so we need to set this equal to colorBuffer.
        }
        else
        {
            FlipVertical(colorBuffer, width, height); // for the webcam
        }

    }

    /// <summary>
    /// Method to flip an array of Color32 RGBA "horizontally", essentially take the image that is represented by the array of pixels and flip it horizontally
    /// </summary>
    /// <param name="pixels"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    void FlipHorizontal(Color32[] pixels, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            int row = y * width;

            for (int x = 0; x < width / 2; x++)
            {
                int left = row + x;
                int right = row + (width - 1 - x);

                (pixels[left], pixels[right]) =
                    (pixels[right], pixels[left]);
            }
        }
        //return new ReadOnlySpan<Color32>(pixels);
    }


    /// <summary>
    /// Method to take the Image represented by the colorBuffer and flip it "vertically", flip it around the halfway line
    /// </summary>
    /// <param name="pixels"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    void FlipVertical(Color32[] pixels, int width, int height)
    {
        for (int y = 0; y < height / 2; y++)
        {
            int top = y * width;
            int bottom = (height - 1 - y) * width; 

            for (int x = 0; x < width; x++)
            {
                (pixels[top + x], pixels[bottom + x]) =
                    (pixels[bottom + x], pixels[top + x]);
            }
        }

        //return new ReadOnlySpan<Color32>(pixels);
    }

    /// <summary>
    /// Method that takes an input Array of pixels, and updates a buffer that represents the same array but rotated 90degrees. This is stored in rotateBuffer.
    /// </summary>
    /// <param name="pixels"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="clockwise"></param>
    /// <returns></returns>
    void Rotate90(Color32[] pixels, int width, int height, bool clockwise)
    {
        // if the image size has changed and doesn't match the pre-allocated memory for the rotated ColorBuffer, then we want to update it to the correct size
        if (rotatedBuffer.Length !=  pixels.Length)
        {
            rotatedBuffer = new Color32[pixels.Length];
        }

        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color32 pixel = pixels[y * width + x]; // y * width gives us a "row", while x gives us the index into that "row". So this inner loop loops over columns, while the outer one loops over rows.
                if (clockwise) // clockwise
                {
                    rotatedBuffer[(height - 1 - y) + x * height] = pixel; //the part in brackets decrements through the "row" i.e it represents the x value in the rotated 2d array, while the x*height decrements through "columns" and represents the y value
                }
                else // anticlockwise
                {
                    rotatedBuffer[(width - 1 - x) * height + y] = pixel; // Counter-clockwise rotation
                }
                
            }
        }

    }


    //Test method that draws a green line following the z axis
    private void DrawZVector(Vector3 position, Quaternion rotation)
    {

        Camera arCamera = Camera.main;

        
        //Debug.Log($"Camera: {arCamera?.name} | Raw pos: {position} | World pos: {worldPosition}");

        //cameraSpacePos.x *= -1

        if (debugObject == null)
        {
            debugObject = new GameObject("AprilTag_ZAxis");
            debugObject.transform.SetParent(null);
        }

        Vector3 forward = rotation * Vector3.forward;

        Vector3 endPoint = position + forward * 5;

        LineRenderer lineRenderer = debugObject.GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            lineRenderer = debugObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));

            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.green;

            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;

            lineRenderer.positionCount = 2;

            //lineRenderer.useWorldSpace = true;
        }



        lineRenderer.SetPosition(0, position);
        lineRenderer.SetPosition(1, endPoint);
        
        //LineRenderer(position, endPoint, Color.green);


    }

    /// <summary>
    /// Method to move the associated prefabs to the detected AprilTag. 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    //private void EnablePrefab(int id, Vector3 position, Quaternion rotation)
    //{
    //    Debug.Log($"Tag position {position}");
    //    Debug.Log($"Tag rotation {rotation}");
    //    //Debug.Log($"Camera position {Camera.main.transform.position}");


    //    // Converting the coordinates relative to the camera into worldspace position and rotation
    //    Transform cameraTransform = Camera.main.transform;

    //    // if a given AprilTag id has an associated prefab and it exists then activate it
    //    if (tagObjects.Contains(id))
    //    {
    //        GameObject prefab = tagObjects.Get(id);

    //        if (prefab != null)
    //        {
    //            prefab.SetActive(true);

    //            prefab.transform.position = position;
    //            prefab.transform.rotation = rotation;
    //            prefab.transform.localScale = new Vector3(5, 5, 5);

    //            prefab.transform.Rotate(new Vector3(45, 0, -90), Space.Self);// rotate the object to face "forwards"
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
    //    foreach (int id in tagObjects.GetKeys())
    //    {
    //        GameObject child = tagObjects.Get(id);
    //        child.SetActive(false);

    //        child.transform.position = new Vector3(0, 0, 0);
    //        child.transform.rotation = new Quaternion(0, 0, 0, 0);
    //    }


    //}
}