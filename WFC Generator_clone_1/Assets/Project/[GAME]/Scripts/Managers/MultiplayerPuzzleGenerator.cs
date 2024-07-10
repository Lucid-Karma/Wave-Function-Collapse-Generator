using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Sirenix.Utilities;

public class MultiplayerPuzzleGenerator : WfcGeneratorStates
{
    public override void CreatePuzzle(WfcGenerator fsm)
    {
        GameModeManager.Instance.StartMultiplayer();

        if (fsm.IsHost || fsm.IsServer)
        {
            fsm.StartCoroutine(StartChallenge(fsm));
            NetworkSpawnPuzzle(fsm);
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
        foreach (var piece in generator.moduleObjects)
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
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton is null in RegisterObject.");
            return;
        }
        if (NetworkManager.Singleton.SpawnManager == null)
        {
            Debug.LogError("NetworkManager.Singleton.SpawnManager is null in RegisterObject.");
            return;
        }

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

