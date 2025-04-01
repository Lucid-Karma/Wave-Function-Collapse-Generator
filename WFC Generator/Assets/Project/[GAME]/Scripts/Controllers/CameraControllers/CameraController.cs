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
    public Transform target; // Kameranýn bakacaðý merkez (hedef)
    public float zoomSpeed = 2.0f; // Zoom hýzý
    public float rotationSpeed = 3.0f; // Rotasyon hýzý
    public float minYAngle = 60f; // Minimum y ekseni açýsý (kürenin altýna inmemek için)
    public float maxYAngle = 90f; // Maksimum y ekseni açýsý (tam yukarý bakmak için)
    public float minZoomDistance = 5f; // Minimum zoom mesafesi
    public float maxZoomDistance = 40f; // Maksimum zoom mesafesi

    private float distance; // Hedefe olan mesafe (zoom)
    private float currentX = 0f; // X ekseni rotasyonu
    private float currentY = 45f; // Y ekseni rotasyonu (baþlangýç açýsý)

    private Vector2 lastTouchPos; // Son dokunmatik pozisyonu

    void Start()
    {
        // Baþlangýçta kameranýn hedefe olan mesafesini ayarla
        distance = Vector3.Distance(transform.position, target.position);

        // Kamerayý baþlangýç konumuna ayarla (90 derece merkezden yukarý bakacak)
        currentY = maxYAngle;
    }

    void Update()
    {
        // Tek dokunuþla kameranýn rotasyonunu kontrol et
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDelta = Input.GetTouch(0).deltaPosition;

            // Rotasyon hareketini uygula
            currentX += touchDelta.x * rotationSpeed * Time.deltaTime;
            currentY -= touchDelta.y * rotationSpeed * Time.deltaTime;

            // Y ekseni sýnýrlarýný uygula (sadece üst yarýda kalacak þekilde)
            currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
        }

        // Ýki dokunuþla zoom kontrolü yap
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Ýki dokunuþ arasýndaki mesafe deðiþikliðine göre zoom yap
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            distance += deltaMagnitudeDiff * zoomSpeed * Time.deltaTime;

            // Zoom mesafesini sýnýrla
            distance = Mathf.Clamp(distance, minZoomDistance, maxZoomDistance);
        }
    }

    void LateUpdate()
    {
        // Kameranýn yeni pozisyonunu ve rotasyonunu hesapla
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = new Vector3(0, 0, -distance);
        transform.position = target.position + rotation * direction;

        transform.rotation = rotation;
    }

}