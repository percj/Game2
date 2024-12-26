 using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AdType
{
    Speed,
    Capacity,
    CapacitySpeed
}
public class rewardedUpgrade : MonoBehaviour
{
    [SerializeField] HelperCardManager helperCardManager;
    [SerializeField] PlayerCardUpgrade playerCardUpgrade;
    [SerializeField] Card card;
    [SerializeField] AdType adType;
    bool Pressed;

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initstatus => { });
        RewardedAdForGame.Instance.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        RewardedAdForGame.Instance.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        RewardedAdForGame.Instance.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;

    }

    public void UserChoseToWatchAd(int adType)
    {
        if (this.adType == (AdType)Enum.ToObject(typeof(AdType), adType)) Pressed = true;
        RewardedAdForGame.Instance.AdTypeForGame = AdTypeForGame.Upgrade;
        if (RewardedAdForGame.Instance.rewardedAd.IsLoaded())
            RewardedAdForGame.Instance.rewardedAd.Show();
    }
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        if(RewardedAdForGame.Instance.AdTypeForGame == AdTypeForGame.Upgrade && Pressed)
        {

            card.LevelUpgradeForLoad();
            if (playerCardUpgrade != null)
            {
                if (adType == AdType.CapacitySpeed)
                    playerCardUpgrade.CollectSpeedUpgraded();
                else if (adType == AdType.Capacity)
                    playerCardUpgrade.CapacityUpgraded();
                else
                    playerCardUpgrade.SpeedUpgraded();
            }
            else if (helperCardManager != null)
            {
                if (adType == AdType.CapacitySpeed)
                    helperCardManager.CollectSpeedUpgraded();
                else if (adType == AdType.Capacity)
                    helperCardManager.CapacityUpgraded();
                else
                    helperCardManager.SpeedUpgraded();
            }
            Pressed = false;
        }
        RewardedAdForGame.Instance.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        RewardedAdForGame.Instance.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        RewardedAdForGame.Instance.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;

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
