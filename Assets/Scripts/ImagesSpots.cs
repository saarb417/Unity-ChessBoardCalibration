using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImagesSpots : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera captureCamera;
    public float roll = 5.0f;
    public float yaw = 10f;
    public float pitch = 10f;

    [Header("Capture Settings")]
    public float dist = 1f;
    public int stages = 3;
    public Transform chessBoard;
    public float delayTime = 0.0f;
    public string imageSavePath = "CapturedImages/"; // Path to save the images
    public int imageWidth = 1920; // Image resolution width
    public int imageHeight = 1080; // Image resolution height

    private Vector3 initialPos;
    private List<Vector3> allPositions;

    void Start()
    {
        initialPos = chessBoard.position;
        allPositions = new List<Vector3>(stages * 8 * (stages + 1));

        if (captureCamera == null)
            captureCamera = GetComponent<Camera>();

        EmptyFolder(imageSavePath);
        StartCoroutine(GetPositions());
    }

    IEnumerator GetPositions()
    {
        for (int stage = 0; stage < stages; stage++)
        {
            initialPos.z += dist;
            initialPos.z += dist / (stage + 1);

            transform.LookAt(chessBoard);

            for (var line = 0; line < 8; line++)
            {
                int angle = line * 45;

                float modX = Mathf.Cos(angle * Mathf.Deg2Rad);
                float modY = Mathf.Sin(angle * Mathf.Deg2Rad);

                for (var spot = 0; spot < (stage + 1); spot++)
                {
                    Vector3 offset = new Vector3(
                        dist * (spot + 1) * modX,
                        dist * (spot + 1) * modY,
                        0f
                    );

                    Vector3 currentPos = initialPos + offset;
                    allPositions.Add(currentPos);
                    transform.position = currentPos;

                    transform.LookAt(chessBoard);

                    var _roll = Random.Range(-roll, roll);
                    var _yaw = Random.Range(-yaw, yaw);
                    var _pitch = Random.Range(-pitch, pitch);
                    transform.Rotate(_roll, _yaw, _pitch);

                    CaptureImage();

                    yield return new WaitForSeconds(delayTime);
                }
            }
        }
    }

    void CaptureImage()
    {
        if (captureCamera != null)
        {
            // Create a RenderTexture with custom resolution
            RenderTexture renderTexture = new RenderTexture(imageWidth, imageHeight, 24);
            captureCamera.targetTexture = renderTexture;

            // Render the camera's view into the RenderTexture
            captureCamera.Render();

            // Create a Texture2D from the RenderTexture
            Texture2D capturedTexture = new Texture2D(imageWidth, imageHeight);
            RenderTexture.active = renderTexture;
            capturedTexture.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
            capturedTexture.Apply();

            // Reset the camera's target texture
            captureCamera.targetTexture = null;

            // Optionally, save the Texture2D as an image file
            SaveTextureToFile(capturedTexture);
        }
    }

    // Method to check if a folder exists, and if it does, empty it
    void EmptyFolder(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath);
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
    }

    void SaveTextureToFile(Texture2D texture)
    {
        // Convert the Texture2D to bytes and save as an image file
        byte[] bytes = texture.EncodeToPNG();
        string fileName = imageSavePath + "/captured_image_" + Time.time + ".png";
        File.WriteAllBytes(fileName, bytes);
    }
}
