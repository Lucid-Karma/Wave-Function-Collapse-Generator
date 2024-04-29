using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "Puzzle Level", menuName = "Puzzle/Level Data")]
public class Level : ScriptableObject
{
    [Header("Theme Data")]
    public List<ThemeData> ThemeDatas = new();
    [Header("Difficulity Data")]
    [InlineEditor(InlineEditorModes.GUIOnly)]
    public List<DifficulityData> DifficulityData = new();


    public Vector2 GetTilingNum(Theme theme)
    {
        try
        {
            for (int i = 0; i < ThemeDatas.Count; i++)
            {
                if (theme == ThemeDatas[i].Theme)
                {
                    return ThemeDatas[i].Tiling;
                }
            }
            Debug.LogError("Theme Numbers are null");
            return Vector2.zero;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Can't find a theme Numbers " + ex.ToString());
            return Vector2.zero;
        }
    }
}
