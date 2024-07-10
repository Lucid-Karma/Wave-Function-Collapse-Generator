using System;
using UnityEngine;
using UnityEngine.UI;

public class StartMatchmakingButton : Button
{
    [HideInInspector] public static Action OnMatchmakingRequest;

    protected override void OnEnable()
    {
        base.OnEnable();
        onClick.AddListener(() => OnMatchmakingRequest.Invoke() );
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        onClick.RemoveListener(() => OnMatchmakingRequest.Invoke() );
    }
}
