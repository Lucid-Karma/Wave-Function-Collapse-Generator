using System;
using System.Collections;
using System.Collections.Generic;
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
    public Panel HowToPlayPanel;
    //public Panel ChallengePanel;
    public Panel MultiplayerPanel;
    public Panel SinglePlayerPanel;
    public Panel DisconnectPanel;
    public Panel MatchRequestClosedPanel;

    private void OnEnable()
    {
        EventManager.OnLevelStart.AddListener(HideLevelPanels);
        EventManager.OnLevelFinish.AddListener(() => StartCoroutine(InitializeLevelSuccessPanel()));
        SuccessAnimController.OnSuccessWent.AddListener(InitializeLevelCompletedPanel);
        RequestChallengeButton.OnChallengeRequest += InitializeChallengeRequestPanel;
        StartMatchmakingButton.OnMatchmakingRequest += InitializeMultiplayerPanel;
        LobbyManager.OnClientDisconnect.AddListener(InitializeDisconnectPanel);
        HelpButton.OnHelpRequest.AddListener(InitializeHowToPlayPanel);
        CloseButton.OnHelpClose.AddListener(() => HowToPlayPanel.HidePanel());
        //MultiplayerTurnManager.OnMatchStart.AddListener(InitializeMultiplayerPanel);
    }

    private void OnDisable()
    {
        EventManager.OnLevelStart.RemoveListener(HideLevelPanels);
        EventManager.OnLevelFinish.RemoveListener(() => StartCoroutine(InitializeLevelSuccessPanel()));
        SuccessAnimController.OnSuccessWent.RemoveListener(InitializeLevelCompletedPanel);
        RequestChallengeButton.OnChallengeRequest -= InitializeChallengeRequestPanel;
        StartMatchmakingButton.OnMatchmakingRequest -= InitializeMultiplayerPanel;
        LobbyManager.OnClientDisconnect.RemoveListener(InitializeDisconnectPanel);
        HelpButton.OnHelpRequest.RemoveListener(InitializeHowToPlayPanel);
        CloseButton.OnHelpClose.RemoveListener(() => HowToPlayPanel.HidePanel());
        //MultiplayerTurnManager.OnMatchStart.RemoveListener(InitializeMultiplayerPanel);
    }

    private void Start()
    {
        bonusLevelIndex = LevelManager.Instance.LevelData.Levels.Count - 2;
        BonusLevelPanel.HidePanel();
        DisconnectPanel.HidePanel();
        MatchRequestClosedPanel.HidePanel();
        ProcessLostNoticePanel.HidePanel();
        HowToPlayPanel.HidePanel();
        MultiplayerPanel.HidePanel();
    }

    private IEnumerator InitializeLevelSuccessPanel()
    {
        yield return new WaitForSeconds(1f);

        if (RotateCells.Instance.isMapSucceed && !LobbyManager.IsDisconnect)
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
        {
            if(!RotateCells.Instance.isMismatch) 
            {
                InitializeLevelCompletedPanel();
            }
            else
            {
                StartCoroutine(InitializeSinglePlayerPanel());
            }
        }

        if(GameModeManager.Instance.CurrentGameMode == GameModeManager.GameMode.Multiplayer)
        {
            MultiplayerPanel.HidePanel();
            SinglePlayerPanel.ShowPanel();
        }

        RotateCells.Instance.ResetMapSuccess();
    }

    [HideInInspector] public static Action OnBonusShowedUp;

    [HideInInspector] public static Action OnLevelCShowed;
    private void InitializeLevelCompletedPanel()
    {
        DisconnectPanel.HidePanel();    

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
    private void InitializeHowToPlayPanel()
    {
        ProcessLostNoticePanel.HidePanel();
        HowToPlayPanel.ShowPanel();
    }
    private void InitializeDisconnectPanel()
    {
        if(!RotateCells.Instance.isMismatch && RotateCells.Instance.isDrawCompleted && !RotateCells.Instance.isMapSucceed)
        {
            DisconnectPanel.ShowPanel();
        }
    }
    private void InitializeMultiplayerPanel()
    {
        SinglePlayerPanel.HidePanel();
        ProcessLostNoticePanel.HidePanel();
        MultiplayerPanel.ShowPanel();
    }

    private IEnumerator InitializeSinglePlayerPanel()
    {
        MultiplayerPanel.HidePanel();

        yield return new WaitForSeconds(1f);

        MatchRequestClosedPanel.ShowPanel();

        yield return new WaitForSeconds(1f);

        MatchRequestClosedPanel.HidePanel();
        SinglePlayerPanel.ShowPanel();

        EventManager.OnLevelInitialize.Invoke();
        RotateCells.Instance.isMismatch = false;
    }

    private void HideLevelPanels()
    {
        BonusWinPanel.HidePanel();
        LevelSuccessPanel.HidePanel();
        LevelCompletedPanel.HidePanel();
        BonusLevelPanel.HidePanel();
        DisconnectPanel.HidePanel();

        //Debug.Log("level started"); ;

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
