using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessNoticeAnimController : MonoBehaviour
{
    private Animator noticeAnimator;
    private void OnEnable()
    {
        noticeAnimator = GetComponent<Animator>();

        ChallengeManager.OnChallengeRequest += StartNoticePopUp;
        StartMatchmakingButton.OnMatchmakingRequest += () => noticeAnimator.SetTrigger("close");
    }
    private void OnDisable()
    {
        ChallengeManager.OnChallengeRequest -= StartNoticePopUp;
        StartMatchmakingButton.OnMatchmakingRequest -= () => noticeAnimator.SetTrigger("close");
    }

    private void StartNoticePopUp()
    {
        noticeAnimator.SetTrigger("popUp");
    }
}
