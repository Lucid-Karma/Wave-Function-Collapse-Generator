using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MultiplayerPuzzleGenerator : WfcGeneratorStates
{
    public override void CreatePuzzle(WfcGenerator fsm)
    {
        Debug.Log("multiplayer");
        GameManager.OnMultiplayerGameStart.Invoke();

        if (fsm.IsHost || fsm.IsServer)
        {
            Debug.Log("this is host creating");
            fsm.StartCoroutine(StartChallenge(fsm));
            //fsm.SpawnClientPuzzleClientRpc();
            NetworkSpawnPuzzle(fsm);
            
        }
        else if (fsm.IsClient)
        {
            Debug.Log("this is client creating");
            //fsm.SpawnClientPuzzle();
            //fsm.Invoke("AnnounceMapReady", 2f);
        }
    }

    private IEnumerator StartChallenge(WfcGenerator generator)
    {
        generator.RecreateLevel();

        yield return GenerateNewPuzzle(generator);
    }

    private IEnumerator GenerateNewPuzzle(WfcGenerator generator)
    {
        generator.GenerateWFC();

        yield return null;
        generator.Invoke("AnnounceMapReady", 2f);
    }

    private void NetworkSpawnPuzzle(WfcGenerator generator)
    {
        foreach (var piece in generator.multiplayerPieces)
        {
            var networkObject = piece.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                RegisterObject(networkObject);
            }
            else
                Debug.Log(networkObject.ToString() + " is null");
        }
    }
    private void RegisterObject(NetworkObject obj)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(obj.NetworkObjectId))
        {
            obj.Spawn();
            //Debug.Log(obj.ToString() + " pos is: " + obj.gameObject.transform.position);
        }
        else
        {
            Debug.LogWarning("Object with same NetworkObjectId already exists.");
        }
    }

    [ClientRpc]
    private void SpawnClientPuzzleClientRpc(WfcGenerator generator)
    {
        //generator.SpawnClientPuzzle();
        generator.Invoke("AnnounceMapReady", 2f);
        Debug.Log("inside ClientRpc");
    }


    private void EndChallenge(bool playerWon)
    {
        if (playerWon)
        {
            Debug.Log("Player won the challenge!");
        }
        else
        {
            Debug.Log("Player lost or opponent disconnected.");
        }

        //NetworkManager.Singleton.Shutdown();
    }

    public override void ExitState(WfcGenerator fsm)
    {
        throw new System.NotImplementedException();
    }

    public override void EnterState(WfcGenerator fsm)
    {
        if (fsm.IsHost || fsm.IsServer)
            Debug.Log("Entered Multiplayer (host)");
        else
            Debug.Log("Entered Multiplayer (client)");

        CreatePuzzle(fsm);
    }
}

