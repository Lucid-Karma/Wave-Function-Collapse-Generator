using UnityEngine;

public class CharacterCameraController : MonoBehaviour
{
    [SerializeField] Camera characterCam;

    private void OnEnable()
    {
        EventManager.OnLevelStart.AddListener(ActivateCam);
    }
    private void OnDisable()
    {
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
