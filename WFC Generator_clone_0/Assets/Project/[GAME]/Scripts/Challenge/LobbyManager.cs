using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class LobbyManager : NetworkBehaviour
{
    [HideInInspector] public static UnityEvent OnPlayersReady = new();
    [HideInInspector] public static UnityEvent OnClientDisconnect = new();

    private static bool isDisconnect;
    public static bool IsDisconnect { get => isDisconnect; private set => isDisconnect = value; }
    //private NetworkVariable<int> currentPlayerCount = new NetworkVariable<int>(0, 
    //                                                    NetworkVariableReadPermission.Everyone);
    private int currentPlayerCount = 0;
    public const int maxPlayers = 2;


    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnConnectionEvent += OnClientConnected;
        GameManager.OnMultiplayerGameFinish.AddListener(UnsubscribeOnConnectionEvent);
        RotateCells.OnNetworkShutdown.AddListener(ResetPlayerCount);
        GameManager.OnSingleplayerGameStart.AddListener(() => IsDisconnect = false);
    }
    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null)
        {
            UnsubscribeOnConnectionEvent();
        }

        GameManager.OnMultiplayerGameFinish.AddListener(UnsubscribeOnConnectionEvent);
        RotateCells.OnNetworkShutdown.RemoveListener(ResetPlayerCount);
        GameManager.OnSingleplayerGameStart.RemoveListener(() => IsDisconnect = false);
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
            IsDisconnect = true;
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

    private void UnsubscribeOnConnectionEvent()
    {
        NetworkManager.Singleton.OnConnectionEvent -= OnClientConnected;
    }
}

