using UnityEngine;

public class CharacterCameraController : MonoBehaviour
{
    [SerializeField] Camera characterCam;

    private void OnEnable()
    {
        StartMatchmakingButton.OnMatchmakingRequest += DeactivateCam;
        LobbyManager.OnPlayersReady.AddListener(ActivateCam);
        EventManager.OnLevelStart.AddListener(ActivateCam);
    }
    private void OnDisable()
    {
        StartMatchmakingButton.OnMatchmakingRequest -= DeactivateCam;
        LobbyManager.OnPlayersReady.RemoveListener(ActivateCam);
        EventManager.OnLevelStart.RemoveListener(ActivateCam);
    }

    private void Start()
    {
        DeactivateCam();
    }

    private void ActivateCam()
    {
        if(!characterCam.enabled)
            characterCam.enabled = true;
    }

    private void DeactivateCam()
    {
        characterCam.enabled = false;
    }
}
