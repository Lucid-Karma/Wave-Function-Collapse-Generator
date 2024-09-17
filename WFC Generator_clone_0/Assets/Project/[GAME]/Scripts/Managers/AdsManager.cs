using UnityEngine;

public class AdsManager : Singleton<AdsManager> 
{
    public InitializeAds initializeAds;
    public BannerAds bannerAds;
    public InterstitialAds interstitialAds;
    public RewardedAds rewardedAds;

    private void Awake()
    {
        bannerAds.LoadBannerAd();
        interstitialAds.LoadInterstitialAd();
        rewardedAds.LoadRewardedAd();
    }

    private void OnEnable()
    {
        EventManager.OnLevelStart.AddListener(() => bannerAds.ShowBannerAd());
        EventManager.OnLevelFinish.AddListener(() => bannerAds.HideBannerAd());
        SolveButton.OnSolveBtnUse.AddListener(ShowInterstitialAd);
    }
    private void OnDisable()
    {
        EventManager.OnLevelStart.RemoveListener(() => bannerAds.ShowBannerAd());
        EventManager.OnLevelFinish.RemoveListener(() => bannerAds.HideBannerAd());
        SolveButton.OnSolveBtnUse.RemoveListener(ShowInterstitialAd);
    }

    int solveCount = 0;
    private void ShowInterstitialAd()
    {
        solveCount++;
        if(solveCount >= 3)
        {
            interstitialAds.ShowInterstitialAd();
            solveCount = 0;
        }
    }
}
