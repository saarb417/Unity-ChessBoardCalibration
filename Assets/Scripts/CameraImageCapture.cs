using System;
using System.Collections;
using System.IO; // Add this for file operations.
using UnityEngine;

public class CameraImageCapture : MonoBehaviour
{
    public PythonScriptManagment pythonScriptManagment; // Reference to the PythonScriptManagment script.

    [Header("Image Capture")]
    public int imageWidth = 1920; // Width of the captured image.
    public int imageHeight = 1080; // Height of the captured image.
    public string captureFolderPath = "CapturedImages"; // Folder to save the captured images.

    private Camera cameraComponent;
    private int frameCount = 0; // Track the number of frames.

    private bool captureStopped = false; // Flag to indicate if capturing should stop.

    public int imagesLimit = 200;

    void Start()
    {
        // Find the Camera component attached to this GameObject.
        cameraComponent = GetComponent<Camera>();

        // Ensure the capture folder exists.
        Directory.CreateDirectory(captureFolderPath);

        // Delete all files in the capture folder (empty the directory).
        string[] files = Directory.GetFiles(captureFolderPath);
        foreach (string file in files)
        {
            File.Delete(file);
        }
        
        StartCoroutine(RunCaptureAndPythonScript());
    }

    IEnumerator RunCaptureAndPythonScript()
    {
        // Start capturing images every 2 frames.
        yield return StartCoroutine(CaptureImage());

        pythonScriptManagment.StartPythonScript();
        pythonScriptManagment = null;
    }

    IEnumerator CaptureImage()
    {
        while (!captureStopped) // Continue capturing until captureStopped is true.
        {
            frameCount++;

            // Capture an image every 2 frames.
            if (frameCount % 2 == 0)
            {
                // Create a RenderTexture to capture the camera's view.
                RenderTexture renderTexture = new RenderTexture(imageWidth, imageHeight, 24);
                cameraComponent.targetTexture = renderTexture;

                // Render the camera's view.
                cameraComponent.Render();

                // Create a Texture2D and read the pixels from the RenderTexture.
                Texture2D screenshot = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
                RenderTexture.active = renderTexture;
                screenshot.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
                screenshot.Apply();

                // Encode the Texture2D to a PNG image and save it.
                byte[] bytes = screenshot.EncodeToPNG();
                string imageName = $"{captureFolderPath}/Capture_{frameCount}.png";
                System.IO.File.WriteAllBytes(imageName, bytes);

                // Clean up resources.
                cameraComponent.targetTexture = null;
                RenderTexture.active = null;
                Destroy(renderTexture);
                Destroy(screenshot);
            }

            // Increment the frame count.

            // Stop capturing after 200 frames.
            if (frameCount >= imagesLimit)
            {
                captureStopped = true; // Set the flag to stop capturing.
            }

            // Wait for the next frame.
            yield return null;
        }
    }
}
