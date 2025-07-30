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

    public float zoomSpeed = 0.1f;
    public float panSpeed = 0.01f;
    public float minZoom = 2f;
    public float maxZoom = 20f;
    public float returnSmoothSpeed = 5f; // Higher = faster return
    public float zoomLerpSpeed = 10f; // For smooth zooming

    private Camera cam;
    private Vector3 originalPosition;
    private Vector3 currentPanOffset = Vector3.zero;
    private float targetZoom;

    void Start()
    {
        cam = Camera.main;
        if (!cam.orthographic)
        {
            Debug.LogWarning("Camera must be orthographic for this controller to work correctly.");
        }

        originalPosition = cam.transform.position;
        targetZoom = cam.orthographicSize;
    }

    void Update()
    {
        float zoomFactor = Mathf.InverseLerp(maxZoom, minZoom, targetZoom); // 0 at maxZoom, 1 at minZoom

        // -------- PAN WITH ONE FINGER --------
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && zoomFactor > 0f)
        {
            Vector2 delta = Input.GetTouch(0).deltaPosition;
            Vector3 move = new Vector3(-delta.x * panSpeed, -delta.y * panSpeed, 0f);
            cam.transform.Translate(move * zoomFactor, Space.Self);
            currentPanOffset = cam.transform.position - originalPosition;
        }

        // -------- ZOOM WITH TWO FINGERS --------
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0Prev = touch0.position - touch0.deltaPosition;
            Vector2 touch1Prev = touch1.position - touch1.deltaPosition;

            float prevMag = (touch0Prev - touch1Prev).magnitude;
            float currentMag = (touch0.position - touch1.position).magnitude;

            float deltaMag = prevMag - currentMag;

            targetZoom += deltaMag * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        // -------- SMOOTH ZOOM TRANSITION --------
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);

        // -------- SMOOTH RETURN TO ORIGINAL POSITION WHEN ZOOMED OUT --------
        if (zoomFactor <= 0.01f)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, originalPosition, Time.deltaTime * returnSmoothSpeed);
            currentPanOffset = cam.transform.position - originalPosition; // Keep tracking the offset
        }
    }

}