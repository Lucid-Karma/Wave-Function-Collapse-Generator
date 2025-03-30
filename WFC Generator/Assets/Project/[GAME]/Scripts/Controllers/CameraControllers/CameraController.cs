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

    /// <summary>
    /// current one
    /// </summary>
    public Transform target; // Kameran�n bakaca�� merkez (hedef)
    public float zoomSpeed = 2.0f; // Zoom h�z�
    public float rotationSpeed = 3.0f; // Rotasyon h�z�
    public float minYAngle = 60f; // Minimum y ekseni a��s� (k�renin alt�na inmemek i�in)
    public float maxYAngle = 90f; // Maksimum y ekseni a��s� (tam yukar� bakmak i�in)
    public float minZoomDistance = 5f; // Minimum zoom mesafesi
    public float maxZoomDistance = 40f; // Maksimum zoom mesafesi

    private float distance; // Hedefe olan mesafe (zoom)
    private float currentX = 0f; // X ekseni rotasyonu
    private float currentY = 45f; // Y ekseni rotasyonu (ba�lang�� a��s�)

    private Vector2 lastTouchPos; // Son dokunmatik pozisyonu

    void Start()
    {
        // Ba�lang��ta kameran�n hedefe olan mesafesini ayarla
        distance = Vector3.Distance(transform.position, target.position);

        // Kameray� ba�lang�� konumuna ayarla (90 derece merkezden yukar� bakacak)
        currentY = maxYAngle;
    }

    void Update()
    {
        // Tek dokunu�la kameran�n rotasyonunu kontrol et
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDelta = Input.GetTouch(0).deltaPosition;

            // Rotasyon hareketini uygula
            currentX += touchDelta.x * rotationSpeed * Time.deltaTime;
            currentY -= touchDelta.y * rotationSpeed * Time.deltaTime;

            // Y ekseni s�n�rlar�n� uygula (sadece �st yar�da kalacak �ekilde)
            currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
        }

        // �ki dokunu�la zoom kontrol� yap
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // �ki dokunu� aras�ndaki mesafe de�i�ikli�ine g�re zoom yap
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            distance += deltaMagnitudeDiff * zoomSpeed * Time.deltaTime;

            // Zoom mesafesini s�n�rla
            distance = Mathf.Clamp(distance, minZoomDistance, maxZoomDistance);
        }
    }

    void LateUpdate()
    {
        // Kameran�n yeni pozisyonunu ve rotasyonunu hesapla
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = new Vector3(0, 0, -distance);
        transform.position = target.position + rotation * direction;

        // Her zaman hedefe bak
        transform.rotation = rotation;
    }



    //public Transform pivot; // Grid merkezindeki pivot
    //public float rotateSpeed = 0.3f;
    //public float zoomSpeed = 0.01f;
    //public float minDistance = 30f;
    //public float maxDistance = 150f;
    //public float minVerticalAngle = 10f; // Yatay bak��� s�n�rla
    //public float maxVerticalAngle = 90f;

    //private Vector2 initialTouchPos;
    //private float currentDistance;
    //private float horizontalAngle = 225f; // Ba�lang�� a��s� (kamera (0,51,0)'da)
    //private float verticalAngle = 43.8f; // Ba�lang�� dikey a��

    //void Start()
    //{
    //    currentDistance = Vector3.Distance(transform.position, pivot.position);
    //    UpdateCameraPosition();
    //}

    //void Update()
    //{
    //    HandleTouchInput();
    //    UpdateCameraPosition();
    //    AdjustFarClip();
    //}

    //void HandleTouchInput()
    //{
    //    if (Input.touchCount == 1) // D�nd�rme
    //    {
    //        Touch touch = Input.GetTouch(0);
    //        if (touch.phase == TouchPhase.Moved)
    //        {
    //            horizontalAngle += touch.deltaPosition.x * rotateSpeed;
    //            verticalAngle -= touch.deltaPosition.y * rotateSpeed;
    //            verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
    //        }
    //    }
    //    else if (Input.touchCount == 2) // Zoom
    //    {
    //        Touch t1 = Input.GetTouch(0);
    //        Touch t2 = Input.GetTouch(1);

    //        Vector2 t1Prev = t1.position - t1.deltaPosition;
    //        Vector2 t2Prev = t2.position - t2.deltaPosition;

    //        float prevMag = (t1Prev - t2Prev).magnitude;
    //        float currentMag = (t1.position - t2.position).magnitude;

    //        currentDistance -= (currentMag - prevMag) * zoomSpeed;
    //        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
    //    }
    //}

    //void UpdateCameraPosition()
    //{
    //    Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
    //    Vector3 offset = rotation * new Vector3(0, 0, -currentDistance);
    //    transform.position = pivot.position + offset;
    //    transform.LookAt(pivot.position);
    //}

    //void AdjustFarClip()
    //{
    //    float gridDiagonal = 37.5f * Mathf.Sqrt(2) * 2; // Grid'in k��egen uzunlu�u
    //    GetComponent<Camera>().farClipPlane = currentDistance + gridDiagonal;
    //}

}