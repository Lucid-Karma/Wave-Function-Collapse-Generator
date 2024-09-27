using UnityEngine;
using UnityEngine.Events;

public class CloseButton : MonoBehaviour
{
    [HideInInspector] public static UnityEvent OnHelpClose = new();
    [SerializeField] private Panel panel;

    public void ClosePanel()
    {
        OnHelpClose.Invoke();

        panel.HidePanel();
        EventManager.OnButtonClick.Invoke();
    }
}
