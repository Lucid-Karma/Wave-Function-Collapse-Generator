using UnityEngine;
using UnityEngine.UI;
using System;

public class RequestChallengeButton : Button
{
    [HideInInspector] public static Action OnPreChallenge;
    [HideInInspector] public static Action OnChallengeRequest;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        onClick.AddListener(RequestChallenge);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        
        onClick.RemoveListener(RequestChallenge);
    }

    private void RequestChallenge()
    {
        if (!RotateCells.Instance.isDrawCompleted) return;

        OnChallengeRequest.Invoke();
        EventManager.OnButtonClick.Invoke();
    }
}
