using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum Theme { Purple = 0, Blue = 1, Orange = 2, NavyBlue = 3, Red = 4, Boss = 5, Reward = 6, OnBoarding = 7}
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
    public Texture ColorAtlasTexture;
    public Color ThemeColor;
    public Color roadColor;
    public Color roadBorderColor;
}

[CreateAssetMenu(fileName = "Puzzle Level Data", menuName = "Puzzle/Level Data")]
public class LevelData : ScriptableObject
{
    [InlineEditor(InlineEditorModes.GUIOnly)]
    public List<Level> Levels = new();

    public List<Level> FakeLevels = new();
}
