using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelTextController : MonoBehaviour
{
    private TextMeshProUGUI levelText;
    public TextMeshProUGUI LevelText
    {
        get
        {
            if(levelText == null)
            levelText = GetComponent<TextMeshProUGUI>();

            return levelText;
        }
    }

    private void OnEnable()
    {
        EventManager.OnLevelStart.AddListener(UpdateLevelText);
    }

    private void OnDisable()
    {
        EventManager.OnLevelStart.RemoveListener(UpdateLevelText); 
    }

    private int levelCount;
    private void UpdateLevelText()
    {
        levelCount = LevelManager.Instance.LevelCount;
        LevelText.text = "Level " + levelCount;
    }
}
