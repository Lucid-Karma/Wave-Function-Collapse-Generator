using DG.Tweening;
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
            rotationX += Input.GetAxis("Mouse Y") * -1 * _rotationSpeed;

            transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
        }
    }

    //private void OnEnable()
    //{
    //    //EventManager.OnGameStart.AddListener(LookPuzzle);
    //    EventManager.OnLevelFinish.AddListener(LookCity);
    //    RotateCells.OnModulesRotate.AddListener(LookPuzzle);
    //}
    //private void OnDisable()
    //{
    //    //EventManager.OnGameStart.RemoveListener(LookPuzzle);
    //    EventManager.OnLevelFinish.RemoveListener(LookCity);
    //    RotateCells.OnModulesRotate.RemoveListener(LookPuzzle);
    //}

    //void LookCity()
    //{
    //    transform.DORotate(new Vector3(-72.5f, 29f, 0f), 1f, RotateMode.WorldAxisAdd)
    //            .SetEase(Ease.OutQuad);
    //}

    //void LookPuzzle()
    //{
    //    transform.DORotate(new Vector3(72.5f, -29f, 0f), 1f, RotateMode.WorldAxisAdd)
    //            .SetEase(Ease.OutQuad);
    //}
}