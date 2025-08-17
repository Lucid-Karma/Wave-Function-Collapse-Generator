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
        //LevelSolve.OnSolveBtnUse.AddListener(() => solveCount ++);
        CharacterBase.OnModulesRotate.AddListener(ShowInterstitialAd);
        ClaimButton.OnRewardClaim.AddListener(ShowRewardedAd);
        Screenshot.OnScreenshotStart.AddListener(() => bannerAds.HideBannerAd());
        Screenshot.OnScreenshotEnd.AddListener(() => bannerAds.ShowBannerAd());
    }
    private void OnDisable()
    {
        EventManager.OnLvlEndPanelFinish.RemoveListener(() => bannerAds.ShowBannerAd());
        EventManager.OnLevelFinish.RemoveListener(() => bannerAds.HideBannerAd());
        //LevelSolve.OnSolveBtnUse.RemoveListener(() => solveCount++);
        CharacterBase.OnModulesRotate.RemoveListener(ShowInterstitialAd);
        ClaimButton.OnRewardClaim.RemoveListener(ShowRewardedAd);
        Screenshot.OnScreenshotStart.RemoveListener(() => bannerAds.HideBannerAd());
        Screenshot.OnScreenshotEnd.RemoveListener(() => bannerAds.ShowBannerAd());
    }

    private void ShowInterstitialAd()
    {
        if(LevelManager.Instance.LevelCount % 3 == 0)
        {
            bannerAds.HideBannerAd();
            interstitialAds.ShowInterstitialAd();
        }
    }

    private void ShowRewardedAd()
    {
        bannerAds.HideBannerAd();
        rewardedAds.ShowRewardedAd();
    }
}
