using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class LobbyManager : NetworkBehaviour
{
    [HideInInspector] public static UnityEvent OnPlayersReady = new();
    [HideInInspector] public static UnityEvent OnClientDisconnect = new();

    //private NetworkVariable<int> currentPlayerCount = new NetworkVariable<int>(0, 
    //                                                    NetworkVariableReadPermission.Everyone);
    private int currentPlayerCount = 0;
    public const int maxPlayers = 2;

    private void Awake()
    {
        NetworkManager.Singleton.OnConnectionEvent += OnClientConnected;
        GameManager.OnMultiplayerGameFinish.AddListener(UnsubscribeOnConnectionEvent);
        RotateCells.OnNetworkShutdown.AddListener(ResetPlayerCount);
    }

    private void OnClientConnected(NetworkManager manager, ConnectionEventData data)
    {
        if(data.EventType == ConnectionEvent.ClientConnected)
        {
            if(NetworkManager.Singleton.IsHost)
            {
                currentPlayerCount/*.Value*/++;
                Debug.Log("event triggered\n" + data.EventType + "\n" + data.ClientId);
                CheckForMaxPlayers();
            }
        }
        else if(data.EventType == ConnectionEvent.ClientDisconnected)
        {
            OnClientDisconnect.Invoke();
        }
    }

    private void CheckForMaxPlayers()
    {
        if (currentPlayerCount == maxPlayers)
        {
            Debug.Log("at max player...");
            StartMultiplayerClientRpc();
            currentPlayerCount = 0;
        }
    }

    [ClientRpc]
    private void StartMultiplayerClientRpc()
    {
        OnPlayersReady?.Invoke();
    }

    private void ResetPlayerCount()
    {
        currentPlayerCount = 0;
        Debug.Log("RESETTED");
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            UnsubscribeOnConnectionEvent();
        }

        GameManager.OnMultiplayerGameFinish.RemoveListener(UnsubscribeOnConnectionEvent);
        RotateCells.OnNetworkShutdown.RemoveListener(ResetPlayerCount);
    }
    private void UnsubscribeOnConnectionEvent()
    {
        NetworkManager.Singleton.OnConnectionEvent -= OnClientConnected;
    }
}

