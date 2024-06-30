using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanels : Panel
{
    public Panel InGamePanel;
    public Panel WelcomePanel;

    private void Start() 
    {
        InitializeWelcomePanel();
    }

    private void OnEnable()
    {
        EventManager.OnGameStart.AddListener(InitializeInGamePanel);
    }

    private void OnDisable()
    {
        EventManager.OnGameStart.RemoveListener(InitializeInGamePanel);
    }

    private void InitializeInGamePanel()
    {
        WelcomePanel.HidePanel();
        InGamePanel.ShowPanel();
    }

    private void InitializeWelcomePanel()
    {
        InGamePanel.HidePanel();
        WelcomePanel.ShowPanel();
    }
}
