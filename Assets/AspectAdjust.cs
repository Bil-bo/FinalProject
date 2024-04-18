using UnityEngine;

public class AspectAdjust : MonoBehaviour
{
    public Camera mainCamera;
    public float targetAspectRatio = 16f / 9f; // You can change this to your desired aspect ratio


    private void Start()
    {
        AdjustCameraViewport();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
        {

            GL.Clear(true, true, Color.black);
            AdjustCameraViewport();
        }
    }

    private void AdjustCameraViewport()
    {
        // Calculate target width and height based on the aspect ratio
        float targetWidth = mainCamera.orthographicSize * 2 * targetAspectRatio;
        float targetHeight = mainCamera.orthographicSize * 2;

        // Current width and height
        float currentWidth = mainCamera.aspect * mainCamera.orthographicSize * 2;
        float currentHeight = mainCamera.orthographicSize * 2;

        // Calculate the desired viewport rect
        Rect viewportRect = new Rect(0, 0, 1, 1);

        if (currentWidth > targetWidth)
        {
            float ratio = targetWidth / currentWidth;
            viewportRect.width *= ratio;
            viewportRect.x = (1 - viewportRect.width) / 2;
        }
        else if (currentHeight > targetHeight)
        {
            float ratio = targetHeight / currentHeight;
            viewportRect.height *= ratio;
        }

        // Set the viewport rect
        mainCamera.rect = viewportRect;
    }
}