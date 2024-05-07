using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CollectButton : Button
{
    [HideInInspector] public static UnityEvent OnCoinCollect = new();

    protected override void OnEnable()
    {
        base.OnEnable();
        onClick.AddListener(CollectCoin);
    }

    protected override void OnDisable()
    {
        base.OnEnable();
        onClick.RemoveListener(CollectCoin);
    }

    private void CollectCoin()
    {
        OnCoinCollect.Invoke();
    }
}
