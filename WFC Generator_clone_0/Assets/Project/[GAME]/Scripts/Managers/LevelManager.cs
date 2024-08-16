using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class LevelManager : Singleton<LevelManager>
{
    [HideInInspector] public static UnityEvent OnLoopComplete = new();
    [HideInInspector] public static UnityEvent OnBonusLevel = new();

    [InlineEditor(InlineEditorModes.GUIOnly)]
    public LevelData LevelData;
    public Theme CurrentTheme = Theme.Purple;


    //Public Properities about current Level
    public Level CurrentLevel { get { return (LevelData.Levels[LevelIndex]); } }
    public DifficultyData DifficultyData { get { return (LevelData.Levels[LevelIndex].DifficultyData[DifficultyIndex]); } }
    public ThemeData CurrentThemeData { get { return (LevelData.Levels[LevelIndex].ThemeDatas[(int)CurrentTheme]); } }


    private bool isLevelStarted;
    [ShowInInspector]
    [ReadOnly]
    public bool IsLevelStarted { get { return isLevelStarted; } private set { isLevelStarted = value; } }

    // from 0 to 6
    public int LevelIndex
    {
        get
        {
            return PlayerPrefs.GetInt("LastLevel", 0);
        }
        set
        {
            if (value == LevelData.Levels.Count - 2) // -2 because of bonus levels.
            {
                value = Random.Range(LevelData.Levels.Count - 2, LevelData.Levels.Count);
            }
            else if (value >= LevelData.Levels.Count - 1)
            {
                value = 0;
                OnLoopComplete.Invoke();
            }
                
            PlayerPrefs.SetInt("LastLevel", value);
        }
    }

    // real level count
    public int LevelCount
    {
        get
        {
            return PlayerPrefs.GetInt("LevelCount", 0);
        }
        set
        {
            PlayerPrefs.SetInt("LevelCount", value);
        }
    }

    public int DifficultyIndex 
    { 
        get
        {
            return PlayerPrefs.GetInt("LastLevelDifficultyIndex" , 0);
        }
        set 
        {
            if (value > LevelData.Levels[LevelIndex].DifficultyData.Count)
                value = LevelData.Levels[LevelIndex].DifficultyData.Count - 1;

            PlayerPrefs.SetInt("LastLevelDifficultyIndex", value);
        }
    }

    public void StartLevel()
    {
        if (IsLevelStarted)
            return;

        IsLevelStarted = true;
        EventManager.OnLevelStart.Invoke();
    }

    public void FinishLevel()
    {
        if (!IsLevelStarted)
            return;

        IsLevelStarted = false;
        var isMismatch = RotateCells.Instance.isMismatch;
        if (GameModeManager.Instance.CurrentGameMode == GameModeManager.GameMode.SinglePlayer && !isMismatch)
        {
            LevelCount++;
            Debug.Log(LevelCount.ToString());
        }
        //else if(isMismatch)
        //{
        //    RotateCells.Instance.isMismatch = false;
        //}
            
        //PlayerPrefs.SetInt("LevelCount", LevelCount);
        EventManager.OnLevelFinish.Invoke();
    }
    
}
