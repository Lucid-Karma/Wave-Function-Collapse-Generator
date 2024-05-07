using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoinController : MonoBehaviour
{
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
    }
    private void OnDisable()
    {
        EventManager.OnLevelSuccess.RemoveListener(IncreaseCoinAmount);
        BuyButton.OnSolutionBuy.RemoveListener(DecreaseCoinAmount);
    }

    private void IncreaseCoinAmount()
    {
        RewardAmount += LevelManager.Instance.CurrentLevel.point;
        PlayerPrefs.SetInt("RewardAmount", RewardAmount);
    }

    void DecreaseCoinAmount()
    {
        if (RewardAmount >= 50)
        {
            RewardAmount -= 50;
            PlayerPrefs.SetInt("RewardAmount", RewardAmount);
        }
    }
}
