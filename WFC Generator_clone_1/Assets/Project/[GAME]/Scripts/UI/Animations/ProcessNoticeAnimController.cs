using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessNoticeAnimController : MonoBehaviour
{
    private Animator noticeAnimator;
    private void OnEnable()
    {
        noticeAnimator = GetComponent<Animator>();

        RequestChallengeButton.OnChallengeRequest += StartNoticePopUp;
        StartMatchmakingButton.OnMatchmakingRequest += () => noticeAnimator.SetTrigger("close");
    }
    private void OnDisable()
    {
        RequestChallengeButton.OnChallengeRequest -= StartNoticePopUp;
        StartMatchmakingButton.OnMatchmakingRequest -= () => noticeAnimator.SetTrigger("close");
    }

    private void StartNoticePopUp()
    {
        noticeAnimator.SetTrigger("popUp");
    }
}
