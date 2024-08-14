
public class MultiplayerPanels : Panel
{
    public Panel WaitingPanel;
    public Panel MatchPanel;
    public Panel DrawPanel;
    public Panel HowToPlayPanel;

    private void OnEnable()
    {
        //EventManager.OnLevelStart.AddListener(HideLevelPanels);
        //EventManager.OnLevelFinish.AddListener(() => StartCoroutine(InitializeLevelSuccessPanel()));
        //SuccessAnimController.OnSuccessWent.AddListener(InitializeLevelCompletedPanel);
        StartMatchmakingButton.OnMatchmakingRequest += InitializeWaitingPanel;
        LobbyManager.OnPlayersReady.AddListener(InitializeMatchPanel);
        MultiplayerTurnManager.OnMatchStart.AddListener(HideAllPanels);
        HelpButton.OnHelpRequest.AddListener(() => HowToPlayPanel.ShowPanel());
    }

    private void OnDisable()
    {
        //EventManager.OnLevelStart.RemoveListener(HideLevelPanels);
        //EventManager.OnLevelFinish.RemoveListener(() => StartCoroutine(InitializeLevelSuccessPanel()));
        //SuccessAnimController.OnSuccessWent.RemoveListener(InitializeLevelCompletedPanel);
        StartMatchmakingButton.OnMatchmakingRequest -= InitializeWaitingPanel;
        LobbyManager.OnPlayersReady.RemoveListener(InitializeMatchPanel);
        MultiplayerTurnManager.OnMatchStart.RemoveListener(HideAllPanels);
        HelpButton.OnHelpRequest.RemoveListener(() => HowToPlayPanel.ShowPanel());
    }

    private void Start()
    {
        HideAllPanels();
    }

    private void InitializeWaitingPanel()
    {
        WaitingPanel.ShowPanel();
    }

    private void InitializeMatchPanel()
    {
        WaitingPanel.HidePanel();
        MatchPanel.ShowPanel();
        DrawPanel.ShowPanel();
    }

    private void HideAllPanels()
    {
        WaitingPanel.HidePanel();
        //MatchPanel.HidePanel();
        DrawPanel.HidePanel();
        HowToPlayPanel.HidePanel();
    }
}
