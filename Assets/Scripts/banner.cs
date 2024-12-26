using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class banner : MonoBehaviour
{
    private BannerView bannerView;
    public void Awake()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });

        this.RequestBanner();
    }

    public void RequestBanner()
    {
#if UNITY_ANDROID
            string adUnitId = "ca-app-pub-7072847384824958/2410189342";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-7072847384824958/6441482435";
#else
        string adUnitId = "unexpected_platform";
#endif
        // Clean up banner before reusing
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }
}