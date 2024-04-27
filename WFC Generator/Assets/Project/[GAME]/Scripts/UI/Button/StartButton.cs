using UnityEngine;

public class StartButton : MonoBehaviour
{
    public void StartGame()
    {
        EventManager.OnGameStart.Invoke();
        EventManager.OnLevelStart.Invoke();
        gameObject.SetActive(false);
    }
}
