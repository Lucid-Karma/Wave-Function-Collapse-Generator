
public class SinglePlayerPanel : Panel
{
    private void OnEnable()
    {
        SsOpenButton.OnSS_Request.AddListener(() => HidePanel());
        MenuCloseButton.OnMenuClose.AddListener(() => ShowPanel());
    }

    private void OnDisable()
    {
        SsOpenButton.OnSS_Request.RemoveListener(() => HidePanel());
        MenuCloseButton.OnMenuClose.RemoveListener(() => ShowPanel());
    }
}
