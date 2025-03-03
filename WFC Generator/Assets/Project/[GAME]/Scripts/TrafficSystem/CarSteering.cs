using UnityEngine;

public class CarSteering : MonoBehaviour
{
    public float steerResponseSpeed = 4f;
    private float currentSteer;

    public void ApplySteer(float direction)
    {
        currentSteer = Mathf.Lerp(currentSteer, direction, Time.deltaTime * steerResponseSpeed);
        transform.Rotate(0, currentSteer * steerResponseSpeed, 0);
    }
}
