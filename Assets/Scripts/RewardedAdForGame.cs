using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AdTypeForGame
{
    Table,
    X2,
    Upgrade
}

public class RewardedAdForGame: MonoBehaviour
{
    [SerializeField] GameObject adButton;
    [SerializeField] GameObject timer;
    [SerializeField] GameObject error;
    public AdTypeForGame AdTypeForGame;
    public RewardedAd rewardedAd;
    private static RewardedAdForGame _instance;
    public static RewardedAdForGame Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }

        MobileAds.Initialize(initstatus => { });
        RequestRewarded();
    }

    public void RequestRewarded()
    {
        string adUnitId;
#if UNITY_ANDROID
            adUnitId = "ca-app-pub-7072847384824958/3424382952";
#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-7072847384824958/5096557271";
#else
        adUnitId = "unexpected_platform";
#endif
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
        this.rewardedAd = new RewardedAd(adUnitId);
        AdRequest request = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(request);
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
    }

    public void UserChoseToWatchAd()
    {
        AdTypeForGame = AdTypeForGame.X2;
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        RequestRewarded();
        if (AdTypeForGame == AdTypeForGame.X2)
        {
            GameSingleton.Instance.multiplier = 2;
            adButton.gameObject.SetActive(false);
            timer.SetActive(true);
        }
    }
    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        error.SetActive(true);
        RequestRewarded();
    }
    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        error.SetActive(true);
        RequestRewarded();
    }
}
