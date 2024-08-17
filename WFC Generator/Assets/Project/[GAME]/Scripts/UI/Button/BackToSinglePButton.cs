using UnityEngine;

public class BactToSinglePButton : Panel
{
    public void BackToSinglePlayer()
    {
        if (GameModeManager.Instance.CurrentGameMode == GameModeManager.GameMode.SinglePlayer)
        {
            RotateCells.Instance.EndMatchDueToMismatch();
        }
    }
}
