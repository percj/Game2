using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class rewardedTable : MonoBehaviour
{
    [SerializeField] StationOpener setTotalMoney;
    public void Start()
    {
        MobileAds.Initialize(initstatus => { });
        RewardedAdForGame.Instance.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        RewardedAdForGame.Instance.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        RewardedAdForGame.Instance.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
    }

    public void UserChoseToWatchAd()
    {
        RewardedAdForGame.Instance.AdTypeForGame = AdTypeForGame.Table;
        if (RewardedAdForGame.Instance.rewardedAd.IsLoaded())
        {
            RewardedAdForGame.Instance.rewardedAd.Show();
        }
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        RewardedAdForGame.Instance.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        RewardedAdForGame.Instance.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        RewardedAdForGame.Instance.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        if (RewardedAdForGame.Instance.AdTypeForGame == AdTypeForGame.Table)
        {
            setTotalMoney.Payment(1);
        }
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        RewardedAdForGame.Instance.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        RewardedAdForGame.Instance.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        RewardedAdForGame.Instance.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
    }
    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        RewardedAdForGame.Instance.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        RewardedAdForGame.Instance.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        RewardedAdForGame.Instance.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
    }
}