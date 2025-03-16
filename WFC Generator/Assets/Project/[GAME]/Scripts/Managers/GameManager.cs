using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : Singleton<GameManager>
{
    private bool isGameStarted;
    [ShowInInspector]
    [ReadOnly]
    public bool IsGameStarted { get { return isGameStarted; } private set { isGameStarted = value; } }

    public GameObject[] carPrefabs;

    void Awake()
    {
        //PlayerPrefs.SetInt("LastLevel", 0);
        //PlayerPrefs.SetInt("LevelCount", 0);
        ////PlayerPrefs.SetInt("RewardAmount", 0);
        ////PlayerPrefs.SetInt("SolveCount", 0);
        //PlayerPrefs.SetInt("LoginCount", 0);
        //PlayerPrefs.SetInt("LastLevelDifficultyIndex", 0);
        //PlayerPrefs.SetString("HexColor", "");
        //PlayerPrefs.SetFloat("CurrentTime", 0);
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
