using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ClaimButton : Button
{
    [HideInInspector] public static UnityEvent OnRewardClaim = new();

    protected override void OnEnable()
    {
        base.OnEnable();
        onClick.AddListener(ClaimReward);
    }

    protected override void OnDisable()
    {
        base.OnEnable();
        onClick.RemoveListener(ClaimReward);
    }

    private void ClaimReward()
    {
        if (!RotateCells.Instance.isDrawCompleted) return;

        //if (SolveTextController.SolveCount > 0)
        //{
            OnRewardClaim.Invoke();
            EventManager.OnButtonClick.Invoke();
        //}
    }
}
