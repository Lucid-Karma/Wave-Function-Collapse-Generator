using System.Collections;
using UnityEngine;

public class TrafficLightsController : MonoBehaviour
{
    [SerializeField] private TrafficLight[] trafficLights;
    [SerializeField] private float greenLightDuration = 5f;

    private void OnEnable()
    {
        if (trafficLights.Length != 4) return;

        StartCoroutine(ControlTrafficLights());
    }

    private IEnumerator ControlTrafficLights()
    {
        while (true)
        {
            for (int i = 0; i < trafficLights.Length; i++)
            {
                SetTrafficLightState(i);
                yield return new WaitForSeconds(greenLightDuration);
            }
        }
    }

    private void SetTrafficLightState(int activeIndex)
    {
        for (int i = 0; i < trafficLights.Length; i++)
        {
            if (i == activeIndex)
                trafficLights[i].SetTrafficLight(true);
            else
                trafficLights[i].SetTrafficLight(false);
        }
    }
}
