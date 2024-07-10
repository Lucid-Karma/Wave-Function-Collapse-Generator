using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class LobbyManager : NetworkBehaviour/*Singleton<LobbyManager>*/
{
    [HideInInspector] public static UnityEvent OnPlayersReady = new();

    //private NetworkVariable<int> currentPlayerCount = new NetworkVariable<int>(0, 
    //                                                    NetworkVariableReadPermission.Everyone);
    private int currentPlayerCount = 0;
    public const int maxPlayers = 2;

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnConnectionEvent += OnClientConnected;
        GameManager.OnMultiplayerGameFinish.AddListener(UnsubscribeOnConnectionEvent);
    }
    //private void OnEnable()
    //{
    //    NetworkManager.Singleton.OnConnectionEvent += OnClientConnected;
    //}
    //private void OnDestroy()
    //{
    //    NetworkManager.Singleton.OnConnectionEvent -= OnClientConnected;
    //}

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

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            UnsubscribeOnConnectionEvent();
        }

        GameManager.OnMultiplayerGameFinish.RemoveListener(UnsubscribeOnConnectionEvent);
    }
    private void UnsubscribeOnConnectionEvent()
    {
        NetworkManager.Singleton.OnConnectionEvent -= OnClientConnected;
    }
}

