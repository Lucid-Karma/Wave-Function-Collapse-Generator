using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCoinController : MonoBehaviour
{
    [HideInInspector] public static UnityEvent OnBonusAdded = new();

    public static int RewardAmount
    {
        get
        {
            return PlayerPrefs.GetInt("RewardAmount", 0);
        }
        set
        {
            if(value < 0)
                value = 0;

            PlayerPrefs.SetInt("RewardAmount", value);
        }
    }

    private void OnEnable()
    {
        EventManager.OnLevelSuccess.AddListener(IncreaseCoinAmount);
        BuyButton.OnSolutionBuy.AddListener(DecreaseCoinAmount);
        CollectButton.OnCoinCollect.AddListener(AddBonusCoinToAmount);
        RewardedAds.OnRewardedAdComplete.AddListener(AddBonusCoinToAmount);
    }
    private void OnDisable()
    {
        EventManager.OnLevelSuccess.RemoveListener(IncreaseCoinAmount);
        BuyButton.OnSolutionBuy.RemoveListener(DecreaseCoinAmount);
        CollectButton.OnCoinCollect.RemoveListener(AddBonusCoinToAmount);
        RewardedAds.OnRewardedAdComplete.RemoveListener(AddBonusCoinToAmount);
    }

    private void IncreaseCoinAmount()
    {
        RewardAmount += LevelManager.Instance.CurrentLevel.point;
    }
    private void AddBonusCoinToAmount()
    {
        RewardAmount += 50;
        OnBonusAdded.Invoke();
    }

    void DecreaseCoinAmount()
    {
        if (RewardAmount >= 50)
        {
            RewardAmount -= 50;
        }
    }
}
