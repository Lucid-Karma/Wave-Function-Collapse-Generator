using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DifficulityMode { Easy, Medium, Hard, OnBoarding, Boss, Reward }
[CreateAssetMenu(fileName = "Difficulty Data", menuName = "Puzzle/Difficulty Data")]
public class DifficultyData : ScriptableObject
{   
    [SerializeField]
    public DifficulityMode DifficulityMode = DifficulityMode.Easy;
    public string DifficultyKey;

    public int mO_CountToRotate;
    public int MO_CountToRotate
    {
        get
        {
            if (GameModeManager.Instance.CurrentGameMode == GameModeManager.GameMode.SinglePlayer)
                return PlayerPrefs.GetInt(DifficultyKey, 0);
            else
                return MultiplayerTurnManager.Instance._drawResult;
        }
        private set
        {
            if (value > RotateCells.Instance.rotatableCount)
            {
                value = RotateCells.Instance.rotatableCount;
                PlayerPrefs.SetInt(DifficultyKey, value);
            }
            else
                PlayerPrefs.SetInt(DifficultyKey, value);
        }
    }

    public int DefaultCountToRotate;

    private void OnEnable()
    {
        LevelManager.OnLoopComplete.AddListener(() => MO_CountToRotate += 2);
        LoginManager.OnFirstLogin.AddListener(() => PlayerPrefs.SetInt(DifficultyKey, mO_CountToRotate));
    }
    private void OnDisable()
    {
        LevelManager.OnLoopComplete.RemoveListener(() => MO_CountToRotate += 2);
        LoginManager.OnFirstLogin.RemoveListener(() => PlayerPrefs.SetInt(DifficultyKey, mO_CountToRotate));
    }
}
