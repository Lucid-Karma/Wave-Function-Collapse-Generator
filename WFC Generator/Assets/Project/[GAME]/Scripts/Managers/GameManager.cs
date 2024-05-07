using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : Singleton<GameManager>
{
    private bool isGameStarted;
    [ShowInInspector]
    [ReadOnly]
    public bool IsGameStarted { get { return isGameStarted; } private set { isGameStarted = value; } }

    //void Awake()
    //{
    //    PlayerPrefs.SetInt("LastLevel", 0);
    //    PlayerPrefs.SetInt("LevelCount", 0);
    //    PlayerPrefs.SetInt("RewardAmount", 0);
    //    PlayerPrefs.SetInt("SolveCount", 0);
    //    PlayerPrefs.SetInt("LoginCount", 0);
    //}

    public void StartGame()
    {
        if (IsGameStarted)
            return;

        IsGameStarted = true;
        EventManager.OnGameStart.Invoke();
    }

    public void EndGame()
    {
        if (!IsGameStarted)
            return;

        IsGameStarted = false;
        EventManager.OnGameEnd.Invoke();
    }


    private void OnEnable()
    {
        EventManager.OnRestart.AddListener(ContinueGame);
        EventManager.OnLevelFail.AddListener(PauseGame);
        //Timer.OnTimeOut += PauseGame;
    }
    private void OnDisable()
    {
        EventManager.OnRestart.RemoveListener(ContinueGame);
        EventManager.OnLevelFail.RemoveListener(PauseGame);
        //Timer.OnTimeOut -= PauseGame;
    }

    void PauseGame()
    {
        Time.timeScale = 0.5f;
    }

    void ContinueGame()
    {
        Time.timeScale = 1;
    }
}
