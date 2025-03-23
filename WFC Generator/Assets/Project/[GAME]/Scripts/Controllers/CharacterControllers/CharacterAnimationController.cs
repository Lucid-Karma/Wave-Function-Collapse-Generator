using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator animator;
    public Animator Animator { get { return (animator == null) ? animator = GetComponent<Animator>() : animator; } }

    private void OnEnable()
    {
        EventManager.OnGameStart.AddListener(() => InvokeTrigger("Greet"));
        CharacterBase.OnModulesRotate.AddListener(() => InvokeTrigger("Request"));
        //VehicleManager.OnVehiclesStopped.AddListener(() => InvokeTrigger("Request"));
        EventManager.OnLevelFinish.AddListener(() => InvokeTrigger("Clap"));
        EventManager.OnMusicOn.AddListener(() => UpdateIdleVersion("ListeningIdle", true));
        EventManager.OnMusicOff.AddListener(() => UpdateIdleVersion("Idle", false));
        EventManager.OnLevelSuccess.AddListener(EndMultiplayerCharacterAnim);
    }
    private void OnDisable()
    {
        EventManager.OnGameStart.RemoveListener(() => InvokeTrigger("Greet"));
        CharacterBase.OnModulesRotate.RemoveListener(() => InvokeTrigger("Request"));
        //VehicleManager.OnVehiclesStopped.RemoveListener(() => InvokeTrigger("Request"));
        EventManager.OnLevelFinish.RemoveListener(() => InvokeTrigger("Clap"));
        EventManager.OnMusicOn.RemoveListener(() => UpdateIdleVersion("ListeningIdle", true));
        EventManager.OnMusicOff.RemoveListener(() => UpdateIdleVersion("Idle", false));
        EventManager.OnLevelSuccess.RemoveListener(EndMultiplayerCharacterAnim);
    }

    private void Start()
    {
        Animator.SetBool("isMusicPlaying", true);
    }

    private void UpdateIdleVersion(string trigger, bool isMusicPlaying)
    {
        InvokeTrigger(trigger);
        Animator.SetBool("isMusicPlaying", isMusicPlaying);
    }

    private void UpdateMultiplayerCharacterAnim(bool canShown)
    {
        if (canShown)
            InvokeTrigger("Yell");
        else
        {
                InvokeTrigger("Idle");  // ???????
        }  
    }
    private void EndMultiplayerCharacterAnim()
    {
            InvokeTrigger("Victory");
    }

    private void InvokeTrigger(string trigger)
    {
        Animator.SetTrigger(trigger);
    }
}
