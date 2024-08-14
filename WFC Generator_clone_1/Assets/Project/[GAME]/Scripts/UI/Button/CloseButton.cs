using UnityEngine;

public class CloseButton : MonoBehaviour
{
    [SerializeField] private Panel panel;

    public void ClosePanel()
    {
        panel.HidePanel();
    }
}
