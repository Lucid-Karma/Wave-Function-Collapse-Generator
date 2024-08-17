using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class MultiplayerTurnManager : MultiplayerSingleton<MultiplayerTurnManager>
{
    public enum Turn
    {
        HostTurn,
        ClientTurn
    }
    public Turn currentPlayer;

    public TextMeshProUGUI modulecountTxt;
    [HideInInspector] public int moduleCountForEachTurn;
    [HideInInspector] public static UnityEvent OnMatchStart = new();
    [HideInInspector] public static Action<bool> OnTurnSwitch;

    private int numberOfMoves;
    public int NumberOfMoves { get { return numberOfMoves; } set { numberOfMoves = value; IncreaseMoveCount(value); } }

    private void IncreaseMoveCount(int value)
    {
        if (value == moduleCountForEachTurn)
        {
            numberOfMoves = 0;
            SwitchPlayer();
        }
    }

    private void Awake()
    {
        LobbyManager.OnPlayersReady.AddListener(DecideModuleCount);
    }
    //public override void OnNetworkSpawn()
    //{
    //    LobbyManager.OnPlayersReady.AddListener(DecideModuleCount);
    //}
    public override void OnDestroy()
    {
        LobbyManager.OnPlayersReady.RemoveListener(DecideModuleCount);
    }


    public void DecideModuleCount()
    {
        if (IsHost)
        {
            moduleCountForEachTurn = Draw();
            WriteRotateCountClientRpc(moduleCountForEachTurn);
            Invoke("StartFirstPlayer", 2f);
        }
    }
    [ClientRpc]
    public void WriteRotateCountClientRpc(int moduleCountToRotate)
    {
        modulecountTxt.text = moduleCountToRotate.ToString();
        moduleCountForEachTurn = moduleCountToRotate;
        NumberOfMoves = 0;
    }

    public void SwitchPlayer()
    {
        if (IsHost || IsServer)
        {
            ClientTurnClientRpc();
            CanPlay = false;
            currentPlayer = Turn.ClientTurn;
        }
        else if (IsClient)
        {
            CanPlay = false;
            ServerTurnServerRpc();
        }
        //Debug.Log($"SwitchPlayer: IsHost={IsHost}");
    }
    [ClientRpc]
    private void ClientTurnClientRpc()
    {
        CanPlay = true;
        Debug.Log("client's turn");
    }
    [ServerRpc(RequireOwnership = false)]
    private void ServerTurnServerRpc()
    {
        CanPlay = true;
        Debug.Log("server's turn");
        currentPlayer = Turn.HostTurn;  // !!!!
    }

    #region Starter Draw
    private bool canPlay;
    public bool CanPlay { get { return canPlay; } set { canPlay = value; OnTurnSwitch.Invoke(value); } }
    int _drawResult;

    [ClientRpc]
    private void AnnounceMatchStartClientRpc()
    {
        StartCoroutine(StartMatch());
    }
    private IEnumerator StartMatch()
    {
        yield return new WaitForSeconds(1.5f);
        OnMatchStart.Invoke();
    }

    public void StartFirstPlayer()
    {
        if (IsHostPlaysFirst())
        {
            PlayHost();
            currentPlayer = Turn.HostTurn;
        }
        else
        {
            PlayClient();
            currentPlayer = Turn.ClientTurn;
        }

        AnnounceMatchStartClientRpc();
    }

    public void PlayHost()
    {
        NegativeBooleanClientRpc();
        CanPlay = true;
        modulecountTxt.text = "You play first as host";
    }
    public void PlayClient()
    {
        PositiveBooleanClientRpc();
        CanPlay = false;
        modulecountTxt.text = "Other player plays first";
    }

    [ClientRpc]
    private void NegativeBooleanClientRpc()
    {
        CanPlay = false;
        modulecountTxt.text = "Other player plays first";
    }
    [ClientRpc]
    private void PositiveBooleanClientRpc()
    {
        CanPlay = true;
        modulecountTxt.text = "You play first as client";
    }

    private bool IsHostPlaysFirst()
    {
        if (Draw() == 1) return true;
        return false;
    }

    private int Draw()
    {
        _drawResult = UnityEngine.Random.Range(1, 3);
        return _drawResult;
    }
    #endregion
}

