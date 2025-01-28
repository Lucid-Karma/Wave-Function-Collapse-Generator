using UnityEngine;
using UnityEngine.Events;

public class MenuCloseButton : Panel
{
    [HideInInspector] public static UnityEvent OnMenuClose = new();

    private void OnEnable()
    {
        MenuOpenButton.OnMainMenuOpen.AddListener(() => ShowPanel());
    }
    private void OnDisable()
    {
        MenuOpenButton.OnMainMenuOpen.RemoveListener(() => ShowPanel());
    }

    private void Start()
    {
        HidePanel();
    }

    public void CloseMenuPanel()
    {
        OnMenuClose.Invoke();
        HidePanel();
        EventManager.OnButtonClick.Invoke();
    }
}
