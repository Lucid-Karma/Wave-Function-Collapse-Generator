using UnityEngine;

public class TrafficSound : MonoBehaviour
{
    private AudioSource trafficSound;

    void Start()
    {
        trafficSound = gameObject.GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        EventManager.OnGameStart.AddListener(() => trafficSound.Play());
        CharacterBase.OnModulesRotate.AddListener(TurnOff);
        EventManager.OnLevelFinish.AddListener(TurnUp);
    }
    void OnDisable()
    {
        EventManager.OnGameEnd.RemoveListener(() => trafficSound.Play());
        CharacterBase.OnModulesRotate.RemoveListener(TurnOff);
        EventManager.OnLevelFinish.RemoveListener(TurnUp);
    }

    private void TurnUp()
    {
        trafficSound.volume = 0.5f;
    }
    private void TurnOff()
    {
        trafficSound.volume = 0.3f;
    }
}
