using UnityEngine;
using UnityEngine.Events;

public class MenuPanels : Panel
{
    [HideInInspector] public static UnityEvent OnSSPanel = new();
    public Panel MainMenuPanel;
    public Panel ScreenshotPanel;
    public Panel SsInfoPanel;

    private void OnEnable()
    {
        MenuOpenButton.OnMainMenuOpen.AddListener(() => MainMenuPanel.ShowPanel());
        SsOpenButton.OnSS_Request.AddListener(InitializeSSPanel);
        Screenshot.OnScreenshotEnd.AddListener(InitializeSSInfoPanel);
        MenuCloseButton.OnMenuClose.AddListener(HideAllPanels);
    }
    private void OnDisable()
    {
        MenuOpenButton.OnMainMenuOpen.RemoveListener(() => MainMenuPanel.ShowPanel());
        SsOpenButton.OnSS_Request.RemoveListener(InitializeSSPanel);
        Screenshot.OnScreenshotEnd.RemoveListener(InitializeSSInfoPanel);
        MenuCloseButton.OnMenuClose.RemoveListener(HideAllPanels);
    }

    private void Start()
    {
        HideAllPanels();
    }

    private void InitializeSSPanel()
    {
        MainMenuPanel.HidePanel();
        ScreenshotPanel.ShowPanel();
        OnSSPanel.Invoke();
    }
    private void InitializeSSInfoPanel()
    {
        ScreenshotPanel.HidePanel();
        SsInfoPanel.ShowPanel();
    }

    private void HideAllPanels()
    {
        MainMenuPanel.HidePanel();
        ScreenshotPanel.HidePanel();
        SsInfoPanel.HidePanel();
    }
}
