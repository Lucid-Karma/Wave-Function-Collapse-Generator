using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    private string playerID;
    public TextMeshProUGUI idText;

    async void Start()
    {
        await UnityServices.InitializeAsync();
        Debug.Log("US Init");
        SignIn();
    }

    async void SignIn()
    {
        Debug.Log("before signing in");
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        playerID = AuthenticationService.Instance.PlayerId;
        idText.text = playerID;
        Debug.Log($"{playerID}" + " signed in");
    }
}
