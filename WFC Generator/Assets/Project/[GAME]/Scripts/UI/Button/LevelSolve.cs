using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LevelSolve : MonoBehaviour
{
    [HideInInspector] public static UnityEvent OnSolveBtnUse = new();
    private bool isButtonUsed = false;

    private void OnEnable()
    {
        EventManager.OnLevelStart.AddListener(ResetButtonState);
        RewardedAds.OnRewardedAdComplete.AddListener(SolveLevel);
    }

    private void OnDisable()
    {
        EventManager.OnLevelStart.RemoveListener(ResetButtonState);
        RewardedAds.OnRewardedAdComplete.RemoveListener(SolveLevel);
    }

    private void SolveLevel()
    {
        if (!CharacterBase.Instance.isDrawCompleted) return;
        if (isButtonUsed) return;

        StartCoroutine(DelaySolveLevel());
    }

    private IEnumerator DelaySolveLevel()
    {
        yield return new WaitForSeconds(0.8f);

        WfcGenerator.OnMapSolve.Invoke();
        OnSolveBtnUse.Invoke();
        EventManager.OnButtonClick.Invoke();
        isButtonUsed = true;
    }

    private void ResetButtonState()
    {
        isButtonUsed = false;
    }
}
