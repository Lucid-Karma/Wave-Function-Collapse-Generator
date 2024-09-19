using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] Camera characterCam;

    private void OnEnable()
    {
        StartMatchmakingButton.OnMatchmakingRequest += DeactivateCam;
        EventManager.OnLevelStart.AddListener(ActivateCam);
    }
    private void OnDisable()
    {
        StartMatchmakingButton.OnMatchmakingRequest -= DeactivateCam;
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
