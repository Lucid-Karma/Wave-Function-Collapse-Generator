using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator animator;
    public Animator Animator { get { return (animator == null) ? animator = GetComponent<Animator>() : animator; } }

    private void OnEnable()
    {
        EventManager.OnGameStart.AddListener(PlayGreetAnimation);
        RotateCells.OnModulesRotate.AddListener(() => InvokeTrigger("Request"));
        EventManager.OnLevelFinish.AddListener(() => InvokeTrigger("Clap"));
        EventManager.OnMusicOn.AddListener(() => InvokeTrigger("ListeningIdle"));
        EventManager.OnMusicOff.AddListener(() => InvokeTrigger("Idle"));
    }
    private void OnDisable()
    {
        EventManager.OnGameStart.RemoveListener(PlayGreetAnimation);
        RotateCells.OnModulesRotate.RemoveListener(() => InvokeTrigger("Request"));
        EventManager.OnLevelFinish.RemoveListener(() => InvokeTrigger("Clap"));
        EventManager.OnMusicOn.RemoveListener(() => InvokeTrigger("ListeningIdle"));
        EventManager.OnMusicOff.RemoveListener(() => InvokeTrigger("Idle"));
    }

    private void PlayGreetAnimation()
    {
        InvokeTrigger("Greet");
    }

    private void InvokeTrigger(string trigger)
    {
        Animator.SetTrigger(trigger);
    }
}
