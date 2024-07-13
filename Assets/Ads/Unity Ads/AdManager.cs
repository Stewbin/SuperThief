using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("Platform Ids")]
    [SerializeField] private string androidGameId;
    [SerializeField] private string iOSGameId;

    [Header("Test Mode (Deactivate For Production)")]
    [SerializeField] private bool testMode = true;

    [Header("Ad Unit Ids")]
    [SerializeField] private string androidRewardedAdUnitId;
    [SerializeField] private string iOSRewardedAdUnitId;
    [SerializeField] private string androidInterstitialAdUnitId;
    [SerializeField] private string iOSInterstitialAdUnitId;
    [SerializeField] private string androidBannerAdUnitId;
    [SerializeField] private string iOSBannerAdUnitId;

    public static AdManager Instance;

    private string gameId;
    private string rewardedAdUnitId;
    private string interstitialAdUnitId;
    private string bannerAdUnitId;
    private bool isRewardedAdReady = false;
    private bool isInterstitialAdReady = false;
    [SerializeField] private BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;

    public delegate void RewardedAdCompletedCallback();
    private RewardedAdCompletedCallback rewardedAdCallback;

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
            rewardedAdUnitId = iOSRewardedAdUnitId;
            interstitialAdUnitId = iOSInterstitialAdUnitId;
            bannerAdUnitId = iOSBannerAdUnitId;
        #elif UNITY_ANDROID
            gameId = androidGameId;
            rewardedAdUnitId = androidRewardedAdUnitId;
            interstitialAdUnitId = androidInterstitialAdUnitId;
            bannerAdUnitId = androidBannerAdUnitId;
        #elif UNITY_EDITOR
            gameId = androidGameId;
            rewardedAdUnitId = androidRewardedAdUnitId;
            interstitialAdUnitId = androidInterstitialAdUnitId;
            bannerAdUnitId = androidBannerAdUnitId;
        #endif

        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize(gameId, testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization successful");
        LoadRewardedAd();
        LoadInterstitialAd();
        LoadBannerAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads failed to initialize: {error.ToString()} - {message}");
    }

    private void LoadRewardedAd()
    {
        Debug.Log($"Loading Rewarded Ad: {rewardedAdUnitId}");
        Advertisement.Load(rewardedAdUnitId, this);
    }

    private void LoadInterstitialAd()
    {
        Debug.Log($"Loading Interstitial Ad: {interstitialAdUnitId}");
        Advertisement.Load(interstitialAdUnitId, this);
    }

    private void LoadBannerAd()
    {
        Debug.Log($"Loading Banner Ad: {bannerAdUnitId}");
        Advertisement.Banner.SetPosition(bannerPosition);
        Advertisement.Banner.Load(bannerAdUnitId,
            new BannerLoadOptions
            {
                loadCallback = OnBannerLoaded,
                errorCallback = OnBannerError
            });
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId.Equals(rewardedAdUnitId))
        {
            isRewardedAdReady = true;
            Debug.Log($"Rewarded Ad Unit {rewardedAdUnitId} loaded successfully");
        }
        else if (placementId.Equals(interstitialAdUnitId))
        {
            isInterstitialAdReady = true;
            Debug.Log($"Interstitial Ad Unit {interstitialAdUnitId} loaded successfully");
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Error loading Ad Unit {placementId}: {error.ToString()} - {message}");
        if (placementId.Equals(rewardedAdUnitId))
        {
            isRewardedAdReady = false;
        }
        else if (placementId.Equals(interstitialAdUnitId))
        {
            isInterstitialAdReady = false;
        }
    }

    public void ShowRewardedAd(RewardedAdCompletedCallback callback)
    {
        if (isRewardedAdReady)
        {
            rewardedAdCallback = callback;
            Advertisement.Show(rewardedAdUnitId, this);
            isRewardedAdReady = false;
        }
        else
        {
            Debug.Log("Rewarded ad not ready. Loading ad...");
            LoadRewardedAd();
        }
    }

    public void ShowInterstitialAd()
    {
        if (isInterstitialAdReady)
        {
            Advertisement.Show(interstitialAdUnitId, this);
            isInterstitialAdReady = false;
        }
        else
        {
            Debug.Log("Interstitial ad not ready. Loading ad...");
            LoadInterstitialAd();
        }
    }

    public void ShowBannerAd()
    {
        Advertisement.Banner.Show(bannerAdUnitId);
    }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Error showing Ad Unit {placementId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"Ad Unit {placementId} show started");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"Ad Unit {placementId} clicked");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals(rewardedAdUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            rewardedAdCallback?.Invoke();
            rewardedAdCallback = null;
        }
        
        if (placementId.Equals(rewardedAdUnitId))
        {
            LoadRewardedAd();
        }
        else if (placementId.Equals(interstitialAdUnitId))
        {
            LoadInterstitialAd();
        }
    }

    private void OnBannerLoaded()
    {
        Debug.Log("Banner ad loaded successfully");
    }

    private void OnBannerError(string message)
    {
        Debug.LogError($"Banner ad error: {message}");
    }

    public void SetBannerPosition(BannerPosition position)
    {
        bannerPosition = position;
        if (Advertisement.Banner.isLoaded)
        {
            Advertisement.Banner.SetPosition(bannerPosition);
        }
    }
}