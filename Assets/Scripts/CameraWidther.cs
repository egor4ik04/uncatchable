using UnityEngine;

public class CameraWidther : MonoBehaviour
{
    public float targetWidth = 10.0f;

    void Start()
    {
        Camera camera = GetComponent<Camera>();
        if (camera.orthographic)
        {
            float aspectRatio = (float)Screen.width / (float)Screen.height;
            float cameraHeight = targetWidth / aspectRatio;
            camera.orthographicSize = cameraHeight / 2.0f;
        }
        else
        {
            Debug.LogWarning("Camera is not orthographic. This script works only with orthographic cameras.");
        }
    }
}
