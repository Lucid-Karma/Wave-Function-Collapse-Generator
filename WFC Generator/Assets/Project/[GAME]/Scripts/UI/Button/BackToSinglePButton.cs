using UnityEngine;
using UnityEngine.Events;

public class BackToSinglePButton : Panel
{
    public void BackToSinglePlayer()
    {
        if (GameModeManager.Instance.CurrentGameMode == GameModeManager.GameMode.SinglePlayer)
        {
            EventManager.OnButtonClick.Invoke();
            RotateCells.Instance.EndMatchDueToMismatch();
        }
    }
}
