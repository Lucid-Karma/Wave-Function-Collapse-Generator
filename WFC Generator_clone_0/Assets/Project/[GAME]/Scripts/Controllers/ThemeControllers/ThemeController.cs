using System.Collections.Generic;
using UnityEngine;

public class ThemeController : MonoBehaviour
{
    public Material moduleMaterial;
    public Material grassMaterial;
    public Material roadMaterial;
    public Material roadBorderMaterial;

    public ThemeData MultiplayerThemeData;


    void OnEnable()
    {
        EventManager.OnLevelStart.AddListener(ChangeLevelTheme);
        GameManager.OnMultiplayerGameStart.AddListener(ImplementMultiplayerTheme);
    }
    void OnDisable()
    {
        EventManager.OnLevelStart.RemoveListener(ChangeLevelTheme);
        GameManager.OnMultiplayerGameStart.RemoveListener(ImplementMultiplayerTheme);
    }

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
            moduleMaterial.SetTexture("_MainTex", LevelManager.Instance.CurrentThemeData.ColorAtlasTexture);
    }

    void ImplementMultiplayerTheme()
    {
        if (MultiplayerThemeData == null) return;

        if (grassMaterial != null && roadMaterial != null && roadBorderMaterial != null)
        {
            grassMaterial.color = MultiplayerThemeData.ThemeColor;
            roadMaterial.color = MultiplayerThemeData.roadColor;
            roadBorderMaterial.color = MultiplayerThemeData.roadBorderColor;
        }

        if (moduleMaterial != null)
            moduleMaterial.SetTexture("_MainTex", MultiplayerThemeData.ColorAtlasTexture);
    }
}
