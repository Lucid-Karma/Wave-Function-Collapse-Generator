using Unity.Netcode;
using UnityEngine;

public class GiveUpButton : NetworkBehaviour
{
    public void GiveUpMatch()
    {
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
