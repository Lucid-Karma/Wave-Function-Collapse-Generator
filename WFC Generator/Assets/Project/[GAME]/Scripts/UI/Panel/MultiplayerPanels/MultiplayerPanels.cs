
public class MultiplayerPanels : Panel
{
    public Panel WaitingPanel;
    public Panel BackToSinglePlayerPanel;
    public Panel MatchPanel;
    public Panel DrawPanel;
    public Panel HowToPlayPanel;

    private void OnEnable()
    {
        //EventManager.OnLevelStart.AddListener(HideLevelPanels);
        //EventManager.OnLevelFinish.AddListener(() => StartCoroutine(InitializeLevelSuccessPanel()));
        //SuccessAnimController.OnSuccessWent.AddListener(InitializeLevelCompletedPanel);
        StartMatchmakingButton.OnMatchmakingRequest += InitializeWaitingPanel;
        SimpleMatchmaking.OnLobbyCreate.AddListener(() => BackToSinglePlayerPanel.ShowPanel());
        LobbyManager.OnPlayersReady.AddListener(InitializeMatchPanel);
        MultiplayerTurnManager.OnMatchStart.AddListener(HideAllPanels);
        HelpButton.OnHelpRequest.AddListener(() => HowToPlayPanel.ShowPanel());
        CloseButton.OnHelpClose.AddListener(() => HowToPlayPanel.HidePanel());
    }

    private void OnDisable()
    {
        //EventManager.OnLevelStart.RemoveListener(HideLevelPanels);
        //EventManager.OnLevelFinish.RemoveListener(() => StartCoroutine(InitializeLevelSuccessPanel()));
        //SuccessAnimController.OnSuccessWent.RemoveListener(InitializeLevelCompletedPanel);
        StartMatchmakingButton.OnMatchmakingRequest -= InitializeWaitingPanel;
        SimpleMatchmaking.OnLobbyCreate.RemoveListener(() => BackToSinglePlayerPanel.ShowPanel());
        LobbyManager.OnPlayersReady.RemoveListener(InitializeMatchPanel);
        MultiplayerTurnManager.OnMatchStart.RemoveListener(HideAllPanels);
        HelpButton.OnHelpRequest.RemoveListener(() => HowToPlayPanel.ShowPanel());
        CloseButton.OnHelpClose.RemoveListener(() => HowToPlayPanel.HidePanel());
    }

    private void Start()
    {
        HideAllPanels();
    }

    private void InitializeWaitingPanel()
    {
        MatchPanel.HidePanel();
        HideAllPanels();
        WaitingPanel.ShowPanel();
    }

    private void InitializeMatchPanel()
    {
        BackToSinglePlayerPanel.HidePanel();
        WaitingPanel.HidePanel();
        MatchPanel.ShowPanel();
        DrawPanel.ShowPanel();
    }

    private void HideAllPanels()
    {
        BackToSinglePlayerPanel.HidePanel();
        WaitingPanel.HidePanel();
        DrawPanel.HidePanel();
        HowToPlayPanel.HidePanel();
    }
}
