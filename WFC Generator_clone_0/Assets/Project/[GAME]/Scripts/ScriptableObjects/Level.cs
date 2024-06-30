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
    public List<DifficultyData> DifficultyData = new();

    [Header("Reward")]
    public int point;


    public Texture GetColorAtlas(Theme theme)
    {
        try
        {
            for (int i = 0; i < ThemeDatas.Count; i++)
            {
                if (theme == ThemeDatas[i].Theme)
                {
                    return ThemeDatas[i].ColorAtlasTexture;
                }
            }
            Debug.LogError("Theme Numbers are null");
            return null;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Can't find a theme Numbers " + ex.ToString());
            return null;
        }
    }
}
