using Unity.Netcode;
using UnityEngine;

public class GiveUpButton : NetworkBehaviour
{
    public void GiveUpMatch()
    {
        MultiplayerTurnManager.Instance.SwitchPlayer();

        if(IsHost || IsServer)
        {
            RotateCells.Instance.EndMultiplayerMatch();
        }
        else if (IsClient)
        {
            EndMatchServerRpc();
        }
    }

    [ServerRpc]
    private void EndMatchServerRpc()
    {
        EndMatchClientRpc();
    }

    [ClientRpc]
    private void EndMatchClientRpc()
    {
        RotateCells.Instance.EndMultiplayerMatch();
    }
}
