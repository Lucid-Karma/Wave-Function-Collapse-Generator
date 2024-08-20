using System;
using UnityEngine;
using UnityEngine.UI;

public class StartMatchmakingButton : Button
{
    [HideInInspector] public static Action OnMatchmakingRequest;

    protected override void OnEnable()
    {
        base.OnEnable();
        onClick.AddListener(StartMatchmaking);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        onClick.RemoveListener(StartMatchmaking);
    }

    private void StartMatchmaking()
    {
        OnMatchmakingRequest.Invoke();
        EventManager.OnButtonClick.Invoke();
    }
}
