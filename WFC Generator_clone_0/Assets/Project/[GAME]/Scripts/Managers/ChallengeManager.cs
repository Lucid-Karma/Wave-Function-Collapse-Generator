using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEditor.PackageManager;
using System;

public class ChallengeManager : Button
{
    //public PuzzleManager puzzleManager; // Reference to your puzzle manager script
    [HideInInspector] public static Action OnPreChallenge;
    [HideInInspector] public static Action OnChallengeRequest;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        onClick.AddListener(() => OnChallengeRequest.Invoke());
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        
        onClick.RemoveListener(() => OnChallengeRequest.Invoke());
    }
}
