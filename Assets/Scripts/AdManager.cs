using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour,
    IUnityAdsInitializationListener,
    IUnityAdsLoadListener,
    IUnityAdsShowListener
{
    public static AdManager Instance { get; private set; }

    [Header("Game IDs")]
    [SerializeField] private string _androidGameId;
    [SerializeField] private string _iOSGameId;
    [SerializeField] private bool _testMode = true;

    [Header("Interstitial Ad Units")]
    [SerializeField] private string _androidInterstitialId = "Interstitial_Android";
    [SerializeField] private string _iOSInterstitialId = "Interstitial_iOS";

    [Header("Rewarded Ad Units")]
    [SerializeField] private string _androidRewardedId = "Rewarded_Android";
    [SerializeField] private string _iOSRewardedId = "Rewarded_iOS";

    [Header("Banner Ad Units")]
    [SerializeField] private string _androidBannerId = "Banner_Android";
    [SerializeField] private string _iOSBannerId = "Banner_iOS";
    [SerializeField] private BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    private string _gameId;

    private string _interstitialAdUnitId;
    private string _rewardedAdUnitId;
    private string _bannerAdUnitId;

    private bool _interstitialLoaded;
    private bool _rewardedLoaded;

    private Action _onRewardedComplete; // callback for rewarded ads

    #region Unity Lifecycle

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

#if UNITY_IOS
        _gameId = _iOSGameId;
        _interstitialAdUnitId = _iOSInterstitialId;
        _rewardedAdUnitId = _iOSRewardedId;
        _bannerAdUnitId = _iOSBannerId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
        _interstitialAdUnitId = _androidInterstitialId;
        _rewardedAdUnitId = _androidRewardedId;
        _bannerAdUnitId = _androidBannerId;
#else
        // Editor – use Android IDs by default
        _gameId = _androidGameId;
        _interstitialAdUnitId = _androidInterstitialId;
        _rewardedAdUnitId = _androidRewardedId;
        _bannerAdUnitId = _androidBannerId;
#endif

        InitializeAds();
    }

    #endregion

    #region Initialization

    private void InitializeAds()
    {
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Debug.Log("[AdManager] Initializing Unity Ads");
            Advertisement.Initialize(_gameId, _testMode, this);
        }
        else
        {
            Debug.Log("[AdManager] Ads already initialized or not supported");
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("[AdManager] Unity Ads initialization complete.");

        // Preload everything you need
        LoadInterstitial();
        LoadRewarded();

        Advertisement.Banner.SetPosition(_bannerPosition);
        LoadBanner();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"[AdManager] Initialization Failed: {error} - {message}");
    }

    #endregion

    #region Interstitial

    public void LoadInterstitial()
    {
        Debug.Log("[AdManager] Loading Interstitial: " + _interstitialAdUnitId);
        Advertisement.Load(_interstitialAdUnitId, this);
    }

    public void ShowInterstitial()
    {
        if (!_interstitialLoaded)
        {
            Debug.Log("[AdManager] Interstitial not loaded yet.");
            return;
        }

        Debug.Log("[AdManager] Showing Interstitial: " + _interstitialAdUnitId);
        Advertisement.Show(_interstitialAdUnitId, this);
        _interstitialLoaded = false; // will need to be loaded again
    }

    #endregion

    #region Rewarded

    /// <summary>
    /// Load a rewarded ad.
    /// </summary>
    public void LoadRewarded()
    {
        Debug.Log("[AdManager] Loading Rewarded: " + _rewardedAdUnitId);
        Advertisement.Load(_rewardedAdUnitId, this);
    }

    /// <summary>
    /// Show rewarded ad, and call onCompleteReward if user watches fully.
    /// </summary>
    public void ShowRewarded(Action onCompleteReward = null)
    {
        if (!_rewardedLoaded)
        {
            Debug.Log("[AdManager] Rewarded not loaded yet.");
            return;
        }

        _onRewardedComplete = onCompleteReward;
        Debug.Log("[AdManager] Showing Rewarded: " + _rewardedAdUnitId);
        Advertisement.Show(_rewardedAdUnitId, this);
        _rewardedLoaded = false; // will need to be loaded again
    }

    #endregion

    #region Banner

    public void LoadBanner()
    {
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = () =>
            {
                Debug.Log("[AdManager] Banner loaded.");
            },
            errorCallback = (message) =>
            {
                Debug.LogError("[AdManager] Banner load error: " + message);
            }
        };

        Debug.Log("[AdManager] Loading Banner: " + _bannerAdUnitId);
        Advertisement.Banner.Load(_bannerAdUnitId, options);
    }

    public void ShowBanner()
    {
        BannerOptions options = new BannerOptions
        {
            clickCallback = () => Debug.Log("[AdManager] Banner clicked."),
            hideCallback = () => Debug.Log("[AdManager] Banner hidden."),
            showCallback = () => Debug.Log("[AdManager] Banner shown.")
        };

        Debug.Log("[AdManager] Showing Banner: " + _bannerAdUnitId);
        Advertisement.Banner.Show(_bannerAdUnitId, options);
    }

    public void HideBanner()
    {
        Debug.Log("[AdManager] Hiding Banner");
        Advertisement.Banner.Hide();
    }

    #endregion

    #region IUnityAdsLoadListener

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("[AdManager] Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_interstitialAdUnitId))
        {
            _interstitialLoaded = true;
        }
        else if (adUnitId.Equals(_rewardedAdUnitId))
        {
            _rewardedLoaded = true;
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"[AdManager] Failed to load Ad Unit {adUnitId}: {error} - {message}");

        // Optional: retry logic
        // if (adUnitId.Equals(_interstitialAdUnitId)) LoadInterstitial();
        // if (adUnitId.Equals(_rewardedAdUnitId)) LoadRewarded();
    }

    #endregion

    #region IUnityAdsShowListener

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"[AdManager] Ad Show Complete: {adUnitId} - {showCompletionState}");

        if (adUnitId.Equals(_rewardedAdUnitId) &&
            showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("[AdManager] Rewarded Ad completed, granting reward.");
            _onRewardedComplete?.Invoke();
        }

        // Auto-reload after showing
        if (adUnitId.Equals(_interstitialAdUnitId))
        {
            LoadInterstitial();
        }
        else if (adUnitId.Equals(_rewardedAdUnitId))
        {
            LoadRewarded();
        }
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"[AdManager] Error showing Ad Unit {adUnitId}: {error} - {message}");

        // Optional: attempt to reload
        if (adUnitId.Equals(_interstitialAdUnitId))
        {
            LoadInterstitial();
        }
        else if (adUnitId.Equals(_rewardedAdUnitId))
        {
            LoadRewarded();
        }
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        Debug.Log("[AdManager] Ad Show Start: " + adUnitId);
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        Debug.Log("[AdManager] Ad Clicked: " + adUnitId);
    }

    #endregion
}
