using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuyButton : Button
{
    [HideInInspector] public static UnityEvent OnSolutionBuy = new();

    protected override void OnEnable()
    {
        base.OnEnable();
        onClick.AddListener(BuySolution);
    }

    protected override void OnDisable()
    {
        base.OnEnable();
        onClick.RemoveListener(BuySolution);
    }

    private void BuySolution()
    {
        if (PlayerCoinController.RewardAmount >= 50)
        {
            OnSolutionBuy.Invoke();
            EventManager.OnButtonClick.Invoke();
        }
    }
}
