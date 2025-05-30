using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;

    public float minX, maxX, minY, maxY;

    public Canvas miniMapCanvas; // Reference to the mini-map canvas

    void Start()
    {
        // Calculate the initial offset between the camera and the target
        offset = transform.position - target.position;

        // Ensure the mini-map is initially visible or hidden as needed
        miniMapCanvas.enabled = false;
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Toggle the mini-map canvas
            miniMapCanvas.enabled = !miniMapCanvas.enabled;
        }

        // Calculate the new position based on target and offset
        Vector3 targetPosition = target.position + offset;

        // Clamp the position within the defined bounds
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        transform.position = targetPosition;
    }
}
