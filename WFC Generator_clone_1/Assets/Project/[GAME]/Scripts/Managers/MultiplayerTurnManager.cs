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

    public override void OnNetworkSpawn()
    {
        LobbyManager.OnPlayersReady.AddListener(DecideModuleCount);
    }
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

    private void SwitchPlayer()
    {
        if (IsHost || IsServer)
        {
            ClientTurnClientRpc();
            canPlay = false;
            currentPlayer = Turn.ClientTurn;
        }
        else if (IsClient)
        {
            canPlay = false;
            ServerTurnServerRpc();
        }
        Debug.Log($"SwitchPlayer: IsHost={IsHost}");
    }
    [ClientRpc]
    private void ClientTurnClientRpc()
    {
        canPlay = true;
        Debug.Log("client's turn");
    }
    [ServerRpc(RequireOwnership = false)]
    private void ServerTurnServerRpc()
    {
        canPlay = true;
        Debug.Log("server's turn");
        currentPlayer = Turn.HostTurn;  // !!!!
    }

    #region Starter Draw
    [HideInInspector] public bool canPlay;
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
        canPlay = true;
        modulecountTxt.text = "You play first as host";
    }
    public void PlayClient()
    {
        PositiveBooleanClientRpc();
        canPlay = false;
        modulecountTxt.text = "Other player plays first";
    }

    [ClientRpc]
    private void NegativeBooleanClientRpc()
    {
        canPlay = false;
        modulecountTxt.text = "Other player plays first";
    }
    [ClientRpc]
    private void PositiveBooleanClientRpc()
    {
        canPlay = true;
        modulecountTxt.text = "You play first as client";
    }

    private bool IsHostPlaysFirst()
    {
        if (Draw() == 1) return true;
        return false;
    }

    private int Draw()
    {
        _drawResult = Random.Range(1, 3);
        return _drawResult;
    }
    #endregion
}

