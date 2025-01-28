using UnityEngine;
using UnityEngine.Events;

public class SsOpenButton : MonoBehaviour
{
    [HideInInspector] public static UnityEvent OnSS_Request = new();
    public void OpenSS_Panel()
    {
        OnSS_Request.Invoke();
        EventManager.OnButtonClick.Invoke();
    }
}
