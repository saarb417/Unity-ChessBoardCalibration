using UnityEngine;
public class CameraParametersExtractor : MonoBehaviour
{
    void Start()
    {
        Camera cam = GetComponent<Camera>(); // Get the camera projection matrix
        Matrix4x4 projMatrix = cam.projectionMatrix;
        // Extract intrinsic parameters (fx, fy, cx, cy) from the projection matrix 
        float fx = projMatrix[0, 0];
        float fy = projMatrix[1, 1];
        float cx = projMatrix[0, 2];
        float cy = projMatrix[1, 2]; // Use the intrinsic parameters for further processing or calibration 


        Debug.Log(fx);
        Debug.Log(fy);
        Debug.Log(cx);
        Debug.Log(cy);
    }
}