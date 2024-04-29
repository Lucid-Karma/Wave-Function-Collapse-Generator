using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum Theme { Purple = 0, Blue = 1, Orange = 2, NavyBlue = 3, Green = 4, Red = 5, Black = 6, Yellow = 7}
public enum LevelObjectType { House, Tree, Car}


[System.Serializable]
public class LevelObjectData
{
    public LevelObjectType LevelObjectType;

    public List<GameObject> ObjectsToCreate = new List<GameObject>();
}

[System.Serializable]
public class ThemeData
{
    public Theme Theme;
    public Vector2 Tiling;
}

[CreateAssetMenu(fileName = "Puzzle Level Data", menuName = "Puzzle/Level Data")]
public class LevelData : ScriptableObject
{
    [InlineEditor(InlineEditorModes.GUIOnly)]
    public List<Level> Levels = new();
}
