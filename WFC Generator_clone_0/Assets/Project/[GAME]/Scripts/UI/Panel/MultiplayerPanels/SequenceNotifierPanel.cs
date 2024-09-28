
using UnityEngine;

public class SequenceNotifierPanel : Panel
{
    Animator animator;

    private void OnEnable()
    {
        MultiplayerTurnManager.OnTurnSwitch += UpdateNotifierPanel;
        StartMatchmakingButton.OnMatchmakingRequest += () => this.HidePanel();
        LobbyManager.OnClientDisconnect.AddListener(() => this.HidePanel());
    }
    private void OnDisable()
    {
        MultiplayerTurnManager.OnTurnSwitch -= UpdateNotifierPanel;
        StartMatchmakingButton.OnMatchmakingRequest -= () => this.HidePanel();
        LobbyManager.OnClientDisconnect.RemoveListener(() => this.HidePanel());
    }

    private void UpdateNotifierPanel(bool canShown)
    {
        if (canShown)
        {
            this.ShowPanel();
            animator.SetTrigger("yourTurn");
        }
        else
            this.HidePanel();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
}
