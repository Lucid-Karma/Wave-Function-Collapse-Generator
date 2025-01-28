using System.Collections.Generic;
using UnityEngine;

public class ThemeController : MonoBehaviour
{
    public Material moduleMaterial;
    public Material grassMaterial;
    public Material roadMaterial;
    public Material roadBorderMaterial;


    //void OnEnable()
    //{
    //    EventManager.OnLevelStart.AddListener(ChangeLevelTheme);
    //}
    //void OnDisable()
    //{
    //    EventManager.OnLevelStart.RemoveListener(ChangeLevelTheme);
    //}

    void ChangeLevelTheme()
    {
        ChangeMaterialsColor();
        ChangeTexture();
    }

    void ChangeMaterialsColor()
    {
        if(grassMaterial != null && roadMaterial != null && roadBorderMaterial != null)
        {
            grassMaterial.color = LevelManager.Instance.CurrentThemeData.ThemeColor;
            roadMaterial.color = LevelManager.Instance.CurrentThemeData.roadColor;
            roadBorderMaterial.color = LevelManager.Instance.CurrentThemeData.roadBorderColor;
        }
        
    }

    void ChangeTexture()
    {
        if(moduleMaterial != null)
            moduleMaterial.SetTexture("_BaseMap", LevelManager.Instance.CurrentThemeData.ColorAtlasTexture);
    }
}
