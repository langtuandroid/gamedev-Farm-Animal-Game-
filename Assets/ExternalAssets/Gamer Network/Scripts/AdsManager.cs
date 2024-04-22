using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using GoogleMobileAds.Api;
using System;

public class AdsManager : MonoBehaviour
{
    /*private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardedAd rewarded;
    //AdUnit Ids
    [Header("Change Admob Ids Here")]
    public string bannerId = "ca-app-pub-3940256099942544/6300978111";
    public string interstitialId = "ca-app-pub-3940256099942544/1033173712";
    public string rewardedId = "ca-app-pub-3940256099942544/5224354917";
    //App id : ca-app-pub-3940256099942544~3347511713
    //Delegate
    public delegate void RewardedDelegate();
    public RewardedDelegate rewardedDelegate;

    public static AdsManager instance;
    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });

        
        this.RequestInterstitial();
        this.RequestRewardedAd();
        if (!SaveData.Instance.RemoveAds)
        {
            this.RequestBanner();
        }
    }

    public void RequestBanner()
    {
        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Top);
        
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }
    public void DestroyBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
        
    }
    private void RequestInterstitial()
    {
        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(interstitialId);
       
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
      
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }
    public void ShowInterstitialAd()
    {
        if (this.interstitial.IsLoaded())
        {
            interstitial.Show();
        }
        else
        {
            RequestInterstitial();
        }
    }
    public void RequestRewardedAd()
    {
        this.rewarded = new RewardedAd(rewardedId);

        this.rewarded.OnAdLoaded += HandleRewardedAdLoaded;
        this.rewarded.OnUserEarnedReward += HandleUserEarnedReward;
        this.rewarded.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewarded.LoadAd(request);
    }
    public void ShowRewardedAd()
    {   
        if (this.rewarded.IsLoaded())
        {
            rewarded.Show();
        }
        else
        {
            RequestRewardedAd();
        }
    }
    public bool CanShowRewardedAd()
    {
        if (this.rewarded.IsLoaded())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void HandleRewardedAdClosed(object sender, EventArgs e)
    {
        //called when user Skipped/Closed the ad
    }

    private void HandleUserEarnedReward(object sender, Reward e)
    {
        //called when user completed 30 sec rewarded video
        rewardedDelegate();

    }

    private void HandleRewardedAdLoaded(object sender, EventArgs e)
    {

    }*/
}



