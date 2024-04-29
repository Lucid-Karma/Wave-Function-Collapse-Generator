using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DifficulityMode { Easy, Medium, Hard }
[CreateAssetMenu(fileName = "Difficulity Data", menuName = "Puzzle/Difficulity Data")]
public class DifficulityData : ScriptableObject
{   
    public DifficulityMode DifficulityMode = DifficulityMode.Easy;
    public int MO_CountToRotate;

    [Tooltip("Determans at which point should this difficulity be activated.")]
    public float ActivateTimeTrashold;

}
