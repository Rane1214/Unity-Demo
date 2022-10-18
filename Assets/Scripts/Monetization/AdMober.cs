using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMober
{
    public static bool AdsRemoved;

    private BannerView bannerView;
    private InterstitialAd interstitial;

#if DEBUG

#if UNITY_ANDROID
    private const string adUnitId_banner = "ca-app-pub-1374298856238199/3964980687";
    private const string adUnitId_interstitial = "ca-app-pub-1374298856238199/6065079152";
    private const string adUnitId_rewarded = "ca-app-pub-1374298856238199/7232204054";
#elif UNITY_IPHONE
    private const string adUnitId_banner = "ca-app-pub-3940256099942544/2934735716";
    private const string adUnitId_interstitial = "ca-app-pub-3940256099942544/4411468910";
#else
    private const string adUnitId_banner = "unexpected_platform";
    private const string adUnitId_interstitial = "unexpected_platform";
#endif

#else

#if UNITY_ANDROID
    private const string adUnitId_banner = "ca-app-pub-3940256099942544/6300978111";
    private const string adUnitId_interstitial = "ca-app-pub-3940256099942544/1033173712";
    private const string adUnitId_rewarded = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
    private const string adUnitId_banner = "ca-app-pub-3940256099942544/2934735716";
    private const string adUnitId_interstitial = "ca-app-pub-3940256099942544/4411468910";
#else
    private const string adUnitId_banner = "unexpected_platform";
    private const string adUnitId_interstitial = "unexpected_platform";
#endif


#endif

    public AdMober()
    {
        MobileAds.Initialize(initStatus => { });

        bannerView = new BannerView(adUnitId_banner, AdSize.Banner, AdPosition.Bottom);
        interstitial = new InterstitialAd(adUnitId_interstitial);

        RequestBanner();
        RequestInterstitial();
    }

    private AdRequest GetNewRequest()
    {
        return new AdRequest.Builder().Build();
    }

    private void RequestBanner()
    {
        bannerView.LoadAd(GetNewRequest());
        bannerView.Hide();
    }

    private bool RequestInterstitial()
    {
        interstitial.LoadAd(GetNewRequest());
        return interstitial.IsLoaded();
    }

    public void BannerHide()
    {
        bannerView.Hide();
    }

    public void BannerShow()
    {
        if (AdsRemoved) return;

        bannerView.Show();
    }

    public void InterstitialShow()
    {
        if (AdsRemoved) return;

        if (RequestInterstitial())
            interstitial.Show();
    }

    public class RewardedAds
    {
        public RewardedAd rewardedAd;

        public RewardedAds()
        {
            rewardedAd = new RewardedAd(adUnitId_rewarded);
            rewardedAd.OnAdClosed += PreloadRewarded;
            PreloadRewarded(null, null);
        }

        public void PreloadRewarded(object sender, EventArgs args)
        {
            rewardedAd.LoadAd(new AdRequest.Builder().Build());
        }

        public void Show(out bool proceed)
        {
            if (proceed = AdsRemoved) return;

            if (rewardedAd.IsLoaded())
                rewardedAd.Show();
        }
    }



}
