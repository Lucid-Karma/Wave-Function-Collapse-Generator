using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SolveButton : Button
{
    [HideInInspector] public static UnityEvent OnSolveBtnUse = new();

    protected override void OnEnable()
    {
        base.OnEnable();
        onClick.AddListener(SolveLevel);
    }

    protected override void OnDisable()
    {
        base.OnEnable();
        onClick.RemoveListener(SolveLevel);
    }

    private void SolveLevel()
    {
        if(SolveTextController.SolveCount > 0)
        {
            WfcGenerator.OnMapSolve.Invoke();
            OnSolveBtnUse.Invoke();
        }
    }
}
