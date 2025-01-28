using UnityEngine;
using UnityEngine.Events;

public class MenuOpenButton : Panel
{
    [HideInInspector] public static UnityEvent OnMainMenuOpen = new();

    private void OnEnable()
    {
        MenuCloseButton.OnMenuClose.AddListener(() => ShowPanel());
    }
    private void OnDisable()
    {
        MenuCloseButton.OnMenuClose.RemoveListener(() => ShowPanel());
    }

    public void OpenMainMenu()
    {
        OnMainMenuOpen.Invoke();
        HidePanel();
        EventManager.OnButtonClick.Invoke();
    }
}
