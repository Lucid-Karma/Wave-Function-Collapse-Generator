using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class MatchmakingManager : Singleton<MatchmakingManager>
{
    private Queue<ulong> playerQueue = new Queue<ulong>();

    public void JoinQueue(ulong clientId)
    {
        playerQueue.Enqueue(clientId);
        CheckForMatch();
    }

    private void CheckForMatch()
    {
        Debug.Log("checking for players count");
        if (playerQueue.Count >= 2)
        {
            ulong player1 = playerQueue.Dequeue();
            ulong player2 = playerQueue.Dequeue();

            StartMatch(player1, player2);
        }
        else
            Debug.Log("waiting for player");
    }

    private void StartMatch(ulong player1, ulong player2)
    {
        Debug.Log("match can be started");
        NetworkManager.Singleton.ConnectedClients[player1].PlayerObject.GetComponent<PlayerController>().StartHostClient(player1 == NetworkManager.Singleton.LocalClientId);
        NetworkManager.Singleton.ConnectedClients[player2].PlayerObject.GetComponent<PlayerController>().StartHostClient(player2 == NetworkManager.Singleton.LocalClientId);
    }
}
