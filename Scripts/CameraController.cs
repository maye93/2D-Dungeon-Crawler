using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        AdjustCameraSize();
    }

    void Update()
    {
        AdjustCameraSize();
    }

    void AdjustCameraSize()
    {
        float aspectRatio = (float)Screen.width / Screen.height;

        // Calculate the desired orthographic size based on the screen height
        float targetOrthographicSize = 5f; // Adjust this value as needed
        float targetOrthographicSizeWidth = targetOrthographicSize * aspectRatio;

        // Set the camera's orthographic size
        mainCamera.orthographicSize = Mathf.Max(targetOrthographicSize, targetOrthographicSizeWidth);
    }
}
