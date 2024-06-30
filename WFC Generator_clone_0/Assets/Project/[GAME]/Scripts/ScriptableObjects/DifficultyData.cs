using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DifficulityMode { Easy, Medium, Hard, OnBoarding, Boss, Reward }
[CreateAssetMenu(fileName = "Difficulty Data", menuName = "Puzzle/Difficulty Data")]
public class DifficultyData : ScriptableObject
{   
    [SerializeField]
    public DifficulityMode DifficulityMode = DifficulityMode.Easy;
    public int mO_CountToRotate;
    public int MO_CountToRotate
    {
        get
        {
            return mO_CountToRotate;
        }
        set
        {
            if (value > RotateCells.rotatableCount)
                value = RotateCells.rotatableCount;
        }
    }

    public int DefaultCountToRotate;

    private void OnEnable()
    {
        LevelManager.OnLoopComplete.AddListener(() => mO_CountToRotate += 2);
    }
    private void OnDisable()
    {
        LevelManager.OnLoopComplete.RemoveListener(() => mO_CountToRotate += 2);
    }
}
