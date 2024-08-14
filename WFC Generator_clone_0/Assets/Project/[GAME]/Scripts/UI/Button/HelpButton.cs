using UnityEngine;
using UnityEngine.Events;

public class HelpButton : MonoBehaviour
{
    [HideInInspector] public static UnityEvent OnHelpRequest = new();

    public void Help()
    {
        OnHelpRequest.Invoke();
    }
}
