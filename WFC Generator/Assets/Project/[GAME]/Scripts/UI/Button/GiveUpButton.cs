using Unity.Netcode;
using UnityEngine;

public class GiveUpButton : NetworkBehaviour
{
    public void GiveUpMatch()
    {
        if (GameModeManager.Instance.CurrentGameMode == GameModeManager.GameMode.SinglePlayer)
        {
            RotateCells.Instance.EndMatchDueToMismatch();
            return;
        }
        
        
        MultiplayerTurnManager.Instance.SwitchPlayer();

        if(IsHost || IsServer)
        {
            RotateCells.Instance.EndChallengeWithWinClientRpc();
        }
        else if (IsClient)
        {
            EndMatchServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void EndMatchServerRpc()
    {
        EndMatchClientRpc();
    }

    [ClientRpc]
    private void EndMatchClientRpc()
    {
        RotateCells.Instance.EndMatchClientRpc();
    }
}
