using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SolveButton : Button
{
    [HideInInspector] public static UnityEvent OnSolveBtnUse = new();
    private bool isButtonUsed = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        onClick.AddListener(SolveLevel);
        EventManager.OnLevelStart.AddListener(ResetButtonState);
    }

    protected override void OnDisable()
    {
        base.OnEnable();
        onClick.RemoveListener(SolveLevel);
        EventManager.OnLevelStart.RemoveListener(ResetButtonState);
    }

    private void SolveLevel()
    {
        if (!CharacterBase.Instance.isDrawCompleted) return;
        if (isButtonUsed) return;

        if (SolveTextController.SolveCount > 0)
        {
            WfcGenerator.OnMapSolve.Invoke();
            OnSolveBtnUse.Invoke();
            EventManager.OnButtonClick.Invoke();
            isButtonUsed = true;
        }
    }

    private void ResetButtonState()
    {
        isButtonUsed = false;
    }
}
