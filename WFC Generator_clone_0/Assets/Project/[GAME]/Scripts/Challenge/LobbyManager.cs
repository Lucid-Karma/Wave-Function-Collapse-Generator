using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class LobbyManager : NetworkBehaviour
{
    [HideInInspector] public static UnityEvent OnPlayersReady = new();

    private NetworkVariable<int> currentPlayerCount = new NetworkVariable<int>(0, 
                                                        NetworkVariableReadPermission.Everyone);
    public int maxPlayers = 2;

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnConnectionEvent += OnClientConnected;
    }

    private void OnClientConnected(NetworkManager manager, ConnectionEventData data)
    {
        if(data.EventType == ConnectionEvent.ClientConnected)
        {
            Debug.Log("inside onclient connect");
            if(NetworkManager.Singleton.IsHost)
            {
                currentPlayerCount.Value++;
                CheckForMaxPlayers();
                Debug.Log("new player added " + currentPlayerCount.Value);
            }
        }
    }

    private void CheckForMaxPlayers()
    {
        if (currentPlayerCount.Value == maxPlayers)
        {
            Debug.Log("at max player...");
            OnPlayersReady?.Invoke();
            StartMultiplayerClientRpc();
        }
    }

    [ClientRpc]
    private void StartMultiplayerClientRpc()
    {
        OnPlayersReady?.Invoke();
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnConnectionEvent -= OnClientConnected;
        }  
    }
}

