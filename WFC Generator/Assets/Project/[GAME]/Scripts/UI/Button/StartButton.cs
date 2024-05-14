using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartButton : Button
{
    protected override void OnEnable()
    {
        base.OnEnable();
        onClick.AddListener(StartGame);
    }

    protected override void OnDisable()
    {
        base.OnEnable();
        onClick.RemoveListener(StartGame);
    }

    private void StartGame()
    {
        GameManager.Instance.StartGame();
        LevelManager.Instance.StartLevel();
        EventManager.OnButtonClick.Invoke();
    }
}
