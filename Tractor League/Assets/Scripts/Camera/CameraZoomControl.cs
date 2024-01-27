using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoomControl : MonoBehaviour
{
    public Rigidbody2D target; // The target object (e.g., your vehicle)
    public float minZoom = 5f; // Minimum zoom level
    public float maxZoom = 10f; // Maximum zoom level
    public float zoomSensitivity = 0.5f; // Sensitivity of zoom relative to velocity
    public float lerpRate = 5f; // Rate at which camera zooms in/out

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        target = FindObjectOfType<Vehicle>()?.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (target != null)
        {
            // Calculate zoom factor based on velocity in local Y axis
            float zoomFactor = Mathf.Clamp(target.velocity.magnitude * zoomSensitivity, minZoom, maxZoom);

            // Smoothly transition to the new zoom level
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomFactor, lerpRate * Time.deltaTime);
        }
    }
}
