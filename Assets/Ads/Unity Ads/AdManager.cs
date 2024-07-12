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
    [SerializeField] private string androidAdUnitId;
    [SerializeField] private string iOSAdUnitId;
    [SerializeField] private string androidBannerAdUnitId;
    [SerializeField] private string iOSBannerAdUnitId;

    public static AdManager Instance;

    private string gameId;
    private string adUnitId;
    private string bannerAdUnitId;
    private bool isAdReady = false;
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
            adUnitId = iOSAdUnitId;
            bannerAdUnitId = iOSBannerAdUnitId;
        #elif UNITY_ANDROID
            gameId = androidGameId;
            adUnitId = androidAdUnitId;
            bannerAdUnitId = androidBannerAdUnitId;
        #elif UNITY_EDITOR
            gameId = androidGameId;
            adUnitId = androidAdUnitId;
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
        LoadAd();
        LoadBannerAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads failed to initialize: {error.ToString()} - {message}");
    }

    private void LoadAd()
    {
        Debug.Log($"Loading Ad: {adUnitId}");
        Advertisement.Load(adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId.Equals(adUnitId))
        {
            isAdReady = true;
            Debug.Log($"Ad Unit {adUnitId} loaded successfully");
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        isAdReady = false;
    }

    public void ShowAd()
    {
        if (isAdReady)
        {
            Advertisement.Show(adUnitId, this);
            isAdReady = false;
        }
        else
        {
            Debug.Log("Ad not ready. Loading ad...");
            LoadAd();
        }
    }

    public void ShowRewardedAd(RewardedAdCompletedCallback callback)
    {
        rewardedAdCallback = callback;
        ShowAd();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"Ad Unit {adUnitId} show started");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"Ad Unit {adUnitId} clicked");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals(adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            rewardedAdCallback?.Invoke();
            rewardedAdCallback = null;
        }
        LoadAd();  // Load the next ad
    }

    private void LoadBannerAd()
    {
        // Set banner position
        Advertisement.Banner.SetPosition(bannerPosition);

        // Load banner ad
        Advertisement.Banner.Load(bannerAdUnitId,
            new BannerLoadOptions
            {
                loadCallback = OnBannerLoaded,
                errorCallback = OnBannerError
            });
    }

    private void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");
    }

    private void OnBannerError(string message)
    {
        Debug.LogError($"Banner Error: {message}");
    }

    public void ShowBannerAd()
    {
        Advertisement.Banner.Show(bannerAdUnitId);
    }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
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