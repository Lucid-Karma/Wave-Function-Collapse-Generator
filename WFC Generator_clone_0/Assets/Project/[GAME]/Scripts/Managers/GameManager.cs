using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    private bool isGameStarted;
    [ShowInInspector]
    [ReadOnly]
    public bool IsGameStarted { get { return isGameStarted; } private set { isGameStarted = value; } }

    [HideInInspector] public static UnityEvent OnMultiplayerGameStart = new();
    [HideInInspector] public static UnityEvent OnMultiplayerGameFinish = new();

    void Awake()
    {
        PlayerPrefs.SetInt("LastLevel", 0);
        PlayerPrefs.SetInt("LevelCount", 0);
        PlayerPrefs.SetInt("RewardAmount", 0);
        PlayerPrefs.SetInt("SolveCount", 0);
        PlayerPrefs.SetInt("LoginCount", 0);
        PlayerPrefs.SetInt("LastLevelDifficultyIndex", 0);
    }

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
}
