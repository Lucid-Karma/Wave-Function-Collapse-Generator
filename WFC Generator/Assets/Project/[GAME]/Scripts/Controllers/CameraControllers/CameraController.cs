using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //private float _rotationSpeed = 5.0f;

    //float rotationX, rotationY;

    //void Update()
    //{
    //    if (Input.GetMouseButton(1))
    //    {
    //        rotationY += Input.GetAxis("Mouse X") * _rotationSpeed;
    //        rotationX += Input.GetAxis("Mouse Y") * -1 * _rotationSpeed;

    //        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
    //    }
    //}

    public Transform target;  // The object at the center of the sphere
    public float radius = 10f;  // Distance from the center of the sphere
    public float rotationSpeed = 1.0f;  // Speed of camera movement
    public float zoomSpeed = 2.0f;  // Speed of zooming in/out
    public float minRadius = 5f;  // Minimum zoom limit
    public float maxRadius = 20f;  // Maximum zoom limit

    private float theta = 0f;  // Longitude angle (horizontal movement)
    private float phi = 0.0f;  // Latitude angle (vertical movement)
    private float topThreshold = 0.15f;  // Threshold to consider the camera at the top

    void Start()
    {
        // Start at the top point with 90-degree straight-down view
        StartAtTopPoint();

        // Ensure the camera starts at the correct position
        UpdateCameraPosition();
    }

    void Update()
    {
        // Handle input for rotating the camera on the sphere
        HandleInput();

        // Handle zoom input
        HandleZoom();

        // Update camera position based on new angles and radius
        UpdateCameraPosition();

        // Adjust camera look direction
        UpdateCameraLookAt();
    }

    void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchDelta = touch.deltaPosition;

            // Modify angles based on touch input
            theta -= touchDelta.x * rotationSpeed * Time.deltaTime;
            phi -= touchDelta.y * rotationSpeed * Time.deltaTime;

            // Clamp phi to prevent moving below 0 on the Y axis (stay in the upper half of the sphere)
            phi = Mathf.Clamp(phi, 0.1f, Mathf.PI / 2f);
        }
    }

    void HandleZoom()
    {
        // For mobile devices, handle pinch zoom using two fingers
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (distance) between the touches in each frame
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Adjust the radius based on the pinch movement
            radius += deltaMagnitudeDiff * zoomSpeed * Time.deltaTime;

            // Clamp the radius to prevent zooming too close or too far
            radius = Mathf.Clamp(radius, minRadius, maxRadius);
        }
    }

    void StartAtTopPoint()
    {
        // Start the camera directly above the target, with 90-degree downward rotation
        phi = topThreshold;  // Near the top
        transform.position = target.position + new Vector3(0, radius, 0);  // Position directly above target
        transform.rotation = Quaternion.Euler(90, 0, 0);  // Set 90-degree downward rotation
    }

    void UpdateCameraPosition()
    {
        // Convert spherical coordinates (radius, theta, phi) to Cartesian coordinates (x, y, z)
        float x = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = radius * Mathf.Cos(phi);
        float z = radius * Mathf.Sin(phi) * Mathf.Sin(theta);

        // Update the camera's position relative to the target's position
        transform.position = target.position + new Vector3(x, y, z);
    }

    void UpdateCameraLookAt()
    {
        // If the camera is near the top (north pole), make it look straight down and set a 90-degree rotation
        if (phi <= topThreshold)
        {
            // Look directly down and set the camera's rotation to avoid any tilt
            transform.position = target.position + new Vector3(0, radius, 0); // Set the camera to be exactly above the target
            transform.rotation = Quaternion.Euler(90, 0, 0); // Set the camera's rotation to face directly down
        }
        else
        {
            // Otherwise, make the camera look at the target as normal
            transform.LookAt(target);
        }
    }




}