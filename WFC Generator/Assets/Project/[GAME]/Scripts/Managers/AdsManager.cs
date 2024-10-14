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
        SolveButton.OnSolveBtnUse.AddListener(() => solveCount ++);
        RotateCells.OnModulesRotate.AddListener(ShowInterstitialAd);
        ClaimButton.OnRewardClaim.AddListener(ShowRewardedAd);
    }
    private void OnDisable()
    {
        EventManager.OnLevelStart.RemoveListener(() => bannerAds.ShowBannerAd());
        EventManager.OnLevelFinish.RemoveListener(() => bannerAds.HideBannerAd());
        SolveButton.OnSolveBtnUse.RemoveListener(() => solveCount++);
        RotateCells.OnModulesRotate.RemoveListener(ShowInterstitialAd);
        ClaimButton.OnRewardClaim.RemoveListener(ShowRewardedAd);
    }

    int solveCount = 0;
    private void ShowInterstitialAd()
    {
        if(solveCount >= 3)
        {
            bannerAds.HideBannerAd();
            interstitialAds.ShowInterstitialAd();
            solveCount = 0;
        }
    }

    private void ShowRewardedAd()
    {
        bannerAds.HideBannerAd();
        rewardedAds.ShowRewardedAd();
    }
}
