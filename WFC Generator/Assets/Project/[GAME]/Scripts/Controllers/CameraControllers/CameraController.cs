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

        transform.rotation = rotation;
    }

}