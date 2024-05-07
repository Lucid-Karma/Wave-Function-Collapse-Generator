using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineRotateController : MonoBehaviour
{
    private float rotateSpeed;

    private void OnEnable()
    {
        EventManager.OnGameStart.AddListener(() => Destroy(this));
    }
    private void OnDisable()
    {
        EventManager.OnGameStart.RemoveListener(() => Destroy(this));
    }

    private void Start()
    {
        int randomSpeed = Random.Range(50, 150);
        int randomNumber = Random.Range(0, 2); 
        rotateSpeed = (randomNumber == 0) ? -1 * randomSpeed : randomSpeed;
    }

    private void Update()
    {
        RotateImage();
    }

    private void RotateImage()
    {
        transform.Rotate(Vector3.forward, Time.deltaTime * rotateSpeed);
    }

    
}
