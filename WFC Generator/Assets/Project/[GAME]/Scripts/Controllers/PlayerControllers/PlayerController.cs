using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    private void OnEnable()
    {
        GameManager.OnMultiplayerGameStart.AddListener(OnChallengeButtonClicked);
    }
    private void OnDisable()
    {
        GameManager.OnMultiplayerGameStart.RemoveListener(OnChallengeButtonClicked);
    }

    public void OnChallengeButtonClicked()
    {
        Debug.Log("gonna check if we are client");
        if (IsClient)
        {
            Debug.Log("we are going to added to queue");
            MatchmakingManager.Instance.JoinQueue(NetworkManager.Singleton.LocalClientId);
        }
    }

    public void StartHostClient(bool isHost)
    {
        Debug.Log("gonna check if we are host");
        if (isHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
