using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class MultiplayerButtonActions : MonoBehaviour
{
    [HideInInspector] public static UnityEvent OnChallenge = new();
    //public void StartChallenge()
    //{
    //    OnChallenge.Invoke();
    //}

    //public void StartHost()
    //{
    //    NetworkManager.Singleton.StartHost();

    //    GameModeManager.Instance.InitializeMultiplayerGame();
    //    Debug.Log("started Host");
    //}

    //public void StartClient()
    //{
    //    NetworkManager.Singleton.StartClient();
    //    EventManager.OnButtonClick.Invoke();
    //    RequestChallengeServerRpc();
    //    Debug.Log("started Client");
    //}

    //[ServerRpc(RequireOwnership = false)]
    //void RequestChallengeServerRpc()
    //{
    //    GameModeManager.Instance.InitializeMultiplayerGame();
    //}
}
