using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class LevelPanels : Panel
{
    [HideInInspector] public static UnityEvent OnSuccessTxtCome = new();
    private int bonusLevelIndex;

    public Panel BonusWinPanel;
    public Panel LevelSuccessPanel;
    public Panel LevelCompletedPanel;
    public Panel BonusLevelPanel;
    public Panel ProcessLostNoticePanel;
    public Panel MultiplayerPanel;

    private void OnEnable()
    {
        EventManager.OnLevelStart.AddListener(HideLevelPanels);
        EventManager.OnLevelFinish.AddListener(() => StartCoroutine(InitializeLevelSuccessPanel()));
        SuccessAnimController.OnSuccessWent.AddListener(InitializeLevelCompletedPanel);
        ChallengeManager.OnChallengeRequest += InitializeChallengeRequestPanel;
        StartMatchmakingButton.OnMatchmakingRequest += InitializeMultiplayerPanel;
    }

    private void OnDisable()
    {
        EventManager.OnLevelStart.RemoveListener(HideLevelPanels);
        EventManager.OnLevelFinish.RemoveListener(() => StartCoroutine(InitializeLevelSuccessPanel()));
        SuccessAnimController.OnSuccessWent.RemoveListener(InitializeLevelCompletedPanel);
        ChallengeManager.OnChallengeRequest -= InitializeChallengeRequestPanel;
        StartMatchmakingButton.OnMatchmakingRequest -= InitializeMultiplayerPanel;
    }

    private void Start()
    {
        bonusLevelIndex = LevelManager.Instance.LevelData.Levels.Count - 2;
        BonusLevelPanel.HidePanel();
        ProcessLostNoticePanel.HidePanel();
        MultiplayerPanel.HidePanel();
    }

    private IEnumerator InitializeLevelSuccessPanel()
    {
        yield return new WaitForSeconds(1f);
      
        if (RotateCells.Instance.isMapSucceed)
        {
            if (!IsBonusLevel())
            {
                LevelSuccessPanel.ShowPanel();
                OnSuccessTxtCome.Invoke();
            }
            else
            {
                InitializeBonusWinPanel();
            }
        }
        else
            InitializeLevelCompletedPanel();
        Debug.Log("level ended");
        RotateCells.Instance.ResetMapSuccess();
    }
    [HideInInspector] public static Action OnBonusShowedUp;

    [HideInInspector] public static Action OnLevelCShowed;
    private void InitializeLevelCompletedPanel()
    {
        LevelSuccessPanel.HidePanel();
        LevelCompletedPanel.ShowPanel();
        OnLevelCShowed.Invoke();
    }

    private void InitializeBonusWinPanel()
    {
        BonusLevelPanel.HidePanel();
        BonusWinPanel.ShowPanel();
        OnBonusShowedUp.Invoke();
    }

    private void InitializeBonusLevelPanel()
    {
        BonusLevelPanel.ShowPanel();
    }

    private void InitializeChallengeRequestPanel()
    {
        ProcessLostNoticePanel.ShowPanel();
    }
    private void InitializeMultiplayerPanel()
    {
        ProcessLostNoticePanel.HidePanel();
        MultiplayerPanel.ShowPanel();
    }

    private void HideLevelPanels()
    {
        BonusWinPanel.HidePanel();
        LevelSuccessPanel.HidePanel();
        LevelCompletedPanel.HidePanel();
        BonusLevelPanel.HidePanel();

        //ProcessLostNoticePanel.HidePanel();
        //MultiplayerPanel.HidePanel() ;
        Debug.Log("level started"); ;

        if (LevelManager.Instance.LevelIndex >= bonusLevelIndex)
        {
            InitializeBonusLevelPanel();
        }
    }

    private bool IsBonusLevel()
    {
        if (GameModeManager.Instance.CurrentGameMode == GameModeManager.GameMode.Multiplayer) return false;

        if (LevelManager.Instance.LevelIndex == 0)  // since the panels are initailized after bonus level ended, the index turns to 0 again.
            return true;

        return false;
    }
}
