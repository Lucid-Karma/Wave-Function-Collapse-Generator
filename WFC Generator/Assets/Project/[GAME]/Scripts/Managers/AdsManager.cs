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
        EventManager.OnLvlEndPanelFinish.AddListener(() => bannerAds.ShowBannerAd());
        EventManager.OnLevelFinish.AddListener(() => bannerAds.HideBannerAd());
        SolveButton.OnSolveBtnUse.AddListener(() => solveCount ++);
        CharacterBase.OnModulesRotate.AddListener(ShowInterstitialAd);
        //VehicleManager.OnVehiclesStopped.AddListener(ShowInterstitialAd);
        ClaimButton.OnRewardClaim.AddListener(ShowRewardedAd);
        Screenshot.OnScreenshotStart.AddListener(() => bannerAds.HideBannerAd());
        Screenshot.OnScreenshotEnd.AddListener(() => bannerAds.ShowBannerAd());
    }
    private void OnDisable()
    {
        EventManager.OnLvlEndPanelFinish.RemoveListener(() => bannerAds.ShowBannerAd());
        EventManager.OnLevelFinish.RemoveListener(() => bannerAds.HideBannerAd());
        SolveButton.OnSolveBtnUse.RemoveListener(() => solveCount++);
        CharacterBase.OnModulesRotate.RemoveListener(ShowInterstitialAd);
        //VehicleManager.OnVehiclesStopped.RemoveListener(ShowInterstitialAd);
        ClaimButton.OnRewardClaim.RemoveListener(ShowRewardedAd);
        Screenshot.OnScreenshotStart.RemoveListener(() => bannerAds.HideBannerAd());
        Screenshot.OnScreenshotEnd.RemoveListener(() => bannerAds.ShowBannerAd());
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
