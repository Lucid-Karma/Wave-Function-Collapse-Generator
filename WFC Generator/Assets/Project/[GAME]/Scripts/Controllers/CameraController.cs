using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float _rotationSpeed = 5.0f;

    float rotationX, rotationY;

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            rotationY += Input.GetAxis("Mouse X") * _rotationSpeed;
            rotationX += Input.GetAxis("Mouse Y") * -1 *  _rotationSpeed;

            transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
        }
    }
}