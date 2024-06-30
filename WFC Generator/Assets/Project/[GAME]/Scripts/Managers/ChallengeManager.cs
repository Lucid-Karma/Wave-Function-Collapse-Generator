using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEditor.PackageManager;
using System;

public class ChallengeManager : Button
{
    //public PuzzleManager puzzleManager; // Reference to your puzzle manager script
    [HideInInspector] public static Action OnPreChallenge;

    protected override void OnEnable()
    {
        base.OnEnable();
        //onClick.AddListener(() => { OnPreChallenge.Invoke(); });
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        //onClick.AddListener(() => OnPreChallenge.Invoke());
    }

    //void OnChallengeButtonPressed()
    //{
    //    if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
    //    {
    //        StartChallenge();
    //    }
    //    else
    //    {
    //        RequestChallengeServerRpc();
    //    }
    //}

    //[ServerRpc(RequireOwnership = false)]
    //void RequestChallengeServerRpc()
    //{
    //    StartChallenge();
    //}

    //void StartChallenge()
    //{
    //    // Logic to sync puzzle and start the challenge
    //    puzzleManager.GenerateNewPuzzle(); // Assuming this method generates and syncs the puzzle across clients
    //    puzzleManager.StartChallengeMode(); // Puts puzzle manager into challenge mode
    //    NotifyClientsChallengeStartedClientRpc();
    //}

    //[ClientRpc]
    //void NotifyClientsChallengeStartedClientRpc()
    //{
    //    puzzleManager.StartChallengeMode();
    //}
}
