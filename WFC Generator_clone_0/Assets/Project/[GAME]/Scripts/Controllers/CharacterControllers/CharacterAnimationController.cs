using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator animator;
    public Animator Animator { get { return (animator == null) ? animator = GetComponent<Animator>() : animator; } }

    private void OnEnable()
    {
        EventManager.OnGameStart.AddListener(() => InvokeTrigger("Greet"));
        RotateCells.OnModulesRotate.AddListener(() => InvokeTrigger("Request"));
        EventManager.OnLevelFinish.AddListener(() => InvokeTrigger("Clap"));
        EventManager.OnMusicOn.AddListener(() => UpdateIdleVersion("ListeningIdle", true));
        EventManager.OnMusicOff.AddListener(() => UpdateIdleVersion("Idle", false));
        MultiplayerTurnManager.OnTurnSwitch += UpdateMultiplayerCharacterAnim;
        EventManager.OnLevelSuccess.AddListener(EndMultiplayerCharacterAnim);
    }
    private void OnDisable()
    {
        EventManager.OnGameStart.RemoveListener(() => InvokeTrigger("Greet"));
        RotateCells.OnModulesRotate.RemoveListener(() => InvokeTrigger("Request"));
        EventManager.OnLevelFinish.RemoveListener(() => InvokeTrigger("Clap"));
        EventManager.OnMusicOn.RemoveListener(() => UpdateIdleVersion("ListeningIdle", true));
        EventManager.OnMusicOff.RemoveListener(() => UpdateIdleVersion("Idle", false));
        MultiplayerTurnManager.OnTurnSwitch -= UpdateMultiplayerCharacterAnim;
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
        if (!GameModeManager.Instance.IsMultiplayer) return;

        if (canShown)
            InvokeTrigger("Yell");
        else
        {
            if(MultiplayerTurnManager.Instance.currentPlayer == MultiplayerTurnManager.Turn.HostTurn)  // ..since the enum changes after CanShow boolean.
                InvokeTrigger("Idle");  // ???????
        }  
    }
    private void EndMultiplayerCharacterAnim()
    {
        if (GameModeManager.Instance.CurrentGameMode == GameModeManager.GameMode.Multiplayer)
            InvokeTrigger("Victory");
    }

    private void InvokeTrigger(string trigger)
    {
        Animator.SetTrigger(trigger);
    }
}
