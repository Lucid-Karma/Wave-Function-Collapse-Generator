using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[System.Serializable]
public class PlayerData
{
    [SerializeField]
    private int moveCount;
    public int MoveCount { get { return moveCount; } set { moveCount = value; EventManager.OnPlayerDataUpdated.Invoke(this); } }
}
