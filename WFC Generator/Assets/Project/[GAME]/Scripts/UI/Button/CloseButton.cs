using UnityEngine;
using UnityEngine.Events;

public class CloseButton : MonoBehaviour
{
    [HideInInspector] public static UnityEvent OnHelpClose = new();

    public void ClosePanel()
    {
        OnHelpClose.Invoke();
        EventManager.OnButtonClick.Invoke();
    }
}
