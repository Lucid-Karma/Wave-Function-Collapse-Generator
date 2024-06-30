using UnityEngine;
using UnityEditor;

public class CustomWFCEditor : EditorWindow
{
    [MenuItem("WFC/WFC Algorithm")]
    public static void GenerateWorldWithWFC()
    {
        WfcGenerator generator = FindObjectOfType<WfcGenerator>();
        
        if (generator != null)
        {
            generator.GenerateWFC();
        }
        else
        {
            Debug.LogWarning("WfcGenerator not found ");
        }
    }
}
