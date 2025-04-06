using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    [SerializeField] private GameObject redLight;
    [SerializeField] private GameObject greenLight;
    private enum LightColor
    {
        RED,
        GREEN,
    }
    private LightColor m_LightColor;

    private void Start()
    {
        greenLight.SetActive(false);
    }

    public bool CanGo()
    {
        return m_LightColor == LightColor.GREEN;
    }

    public void SetTrafficLight(bool isGreen)
    {
        m_LightColor = isGreen ? LightColor.GREEN : LightColor.RED;

        SetLights(isGreen);
    }

    Vehicle vehicle;
    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;

        if (other.CompareTag("Car"))
        {
            vehicle = other.GetComponent<Vehicle>();
            if(!CanGo())
            {
                vehicle.StopForRedLight(this);
            }
        }
    }

    private void SetLights(bool isGreen)
    {
        if (isGreen)
        {
            redLight.SetActive(false);
            greenLight.SetActive(true);
        }
        else
        {
            greenLight.SetActive(false);
            redLight.SetActive(true);
        }
    }
}
