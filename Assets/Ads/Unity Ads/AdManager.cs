using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using UnityEngine;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("Platform Ids")]
    [SerializeField] private string androidGameId;
    [SerializeField] private string iOSGameId;

    [Header("Test Mode (Deactivate For Production)")]
    [SerializeField] private bool testMode = true;

    [SerializeField] private string androidAdUnitId;
    [SerializeField] private string iOSAdUnitId;

    public static AdManager Instance;

    private string gameId;
    private string adUnitId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitializeAds();
    }

    private void InitializeAds()
    {
#if UNITY_IOS
        gameId = iOSGameId;
        adUnitId = iOSAdUnitId;
#elif UNITY_ANDROID
        gameId = androidGameId;
        adUnitId = androidAdUnitId;
#elif UNITY_EDITOR
        gameId = iOSGameId;
        adUnitId = iOSAdUnitId;
#endif

        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize(gameId, testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        print("Unity Ads initialization successful");
        Advertisement.Load(adUnitId, this);
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        print($"Unity Ads failed to initialize: {error.ToString()} - {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Advertisement.Show(placementId, this);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        print($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        print($"Error showing Ad Unit: {adUnitId} - {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        // Optional: Add any logic to handle the start of an ad show
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        // Optional: Add any logic to handle a click on an ad
    }


    public void ShowAd(){
        Advertisement.Load(adUnitId, this); 
    }

    public delegate void RewardedAdCompletedCallback();
    private RewardedAdCompletedCallback rewardedAdCallback;

   

    public void ShowRewardedAd(RewardedAdCompletedCallback callback)
    {
        rewardedAdCallback = callback;
        Advertisement.Load(adUnitId, this);
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals(adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            rewardedAdCallback?.Invoke();
            rewardedAdCallback = null;
        }
    }
}