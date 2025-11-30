using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    // --- Singleton ----------------------------------------------------------------
    public static AdManager Instance { get; private set; }

    // --- Game IDs -----------------------------------------------------------------
    [Header("Game IDs")]
    [SerializeField] private string androidGameId = "5824579";
    [SerializeField] private string iOSGameId = "5824578";
    [SerializeField] private bool testMode = true;
    private string gameId;

    // --- Banner -------------------------------------------------------------------
    [Header("Banner")]
    [SerializeField] private BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;
    [SerializeField] private string bannerAndroidAdUnitId = "Banner_Android";
    [SerializeField] private string bannerIOSAdUnitId = "Banner_iOS";
    private string bannerAdUnitId;
    private bool bannerLoaded = false;
    private bool isBannerLoading = false;

    // --- Interstitial -------------------------------------------------------------
    [Header("Interstitial")]
    [SerializeField] private string interstitialAndroidAdUnitId = "Interstitial_Android";
    [SerializeField] private string interstitialIOSAdUnitId = "Interstitial_iOS";
    private string interstitialAdUnitId;
    private bool interstitialLoaded = false;
    private bool isInterstitialLoading = false;

    // --- Rewarded -----------------------------------------------------------------
    [Header("Rewarded")]
    [SerializeField] private string rewardedAndroidAdUnitId = "Rewarded_Android";
    [SerializeField] private string rewardedIOSAdUnitId = "Rewarded_iOS";
    private string rewardedAdUnitId;
    private bool rewardedLoaded = false;
    private bool isRewardedLoading = false;

    // Used to remember which adUnitId is showing (so we can route callbacks)
    private string lastShowingAdUnitId = null;

    // ------------------------------------------------------------------------------

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Choose platform-specific IDs
#if UNITY_IOS
        gameId = iOSGameId;
        bannerAdUnitId = bannerIOSAdUnitId;
        interstitialAdUnitId = interstitialIOSAdUnitId;
        rewardedAdUnitId = rewardedIOSAdUnitId;
#elif UNITY_ANDROID
        gameId = androidGameId;
        bannerAdUnitId = bannerAndroidAdUnitId;
        interstitialAdUnitId = interstitialAndroidAdUnitId;
        rewardedAdUnitId = rewardedAndroidAdUnitId;
#else
        // Editor or unsupported platforms
        gameId = androidGameId; // allow testing in editor
        bannerAdUnitId = bannerAndroidAdUnitId;
        interstitialAdUnitId = interstitialAndroidAdUnitId;
        rewardedAdUnitId = rewardedAndroidAdUnitId;
#endif

        InitializeAds();
    }

    #region Initialization
    public void InitializeAds()
    {
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Debug.Log($"AdManager: Initializing Unity Ads (gameId: {gameId}, testMode: {testMode})");
            Advertisement.Initialize(gameId, testMode, this);
        }
        else
        {
            Debug.Log("AdManager: Advertisement already initialized or not supported.");
            if (Advertisement.isInitialized)
            {
                OnInitializationComplete();
            }
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("AdManager: Unity Ads Initialization Complete.");
        Advertisement.Banner.SetPosition(bannerPosition);

        // Start loading everything once
        LoadBanner();
        LoadInterstitial();
        LoadRewarded();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"AdManager: Unity Ads Initialization Failed: {error} - {message}");
    }
    #endregion

    #region Banner
    public void LoadBanner()
    {
        if (string.IsNullOrEmpty(bannerAdUnitId))
        {
            Debug.LogWarning("AdManager: Banner Ad Unit Id is null/empty for this platform.");
            return;
        }

        // Already loaded or in-progress → skip
        if (bannerLoaded || isBannerLoading)
        {
            Debug.Log("AdManager: Banner already loaded or loading, skipping LoadBanner.");
            return;
        }

        Debug.Log("AdManager: Loading banner: " + bannerAdUnitId);
        isBannerLoading = true;

        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(bannerAdUnitId, options);
    }

    public void ShowBanner()
    {
        if (bannerLoaded)
        {
            BannerOptions options = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                hideCallback = OnBannerHidden,
                showCallback = OnBannerShown
            };

            Advertisement.Banner.Show(bannerAdUnitId, options);
        }
        else
        {
            Debug.Log("AdManager: Banner not loaded yet, requesting load.");
            LoadBanner();
        }
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
        bannerLoaded = false;
    }

    private void OnBannerLoaded()
    {
        Debug.Log("AdManager: Banner loaded.");
        bannerLoaded = true;
        isBannerLoading = false;

        // Auto-show when loaded
        ShowBanner();
    }

    private void OnBannerError(string message)
    {
        Debug.LogError($"AdManager: Banner Error: {message}");
        bannerLoaded = false;
        isBannerLoading = false;
    }

    private void OnBannerClicked() { /* optional */ }

    private void OnBannerShown()
    {
        Debug.Log("AdManager: Banner shown.");
    }

    private void OnBannerHidden()
    {
        Debug.Log("AdManager: Banner hidden.");
    }
    #endregion

    #region Interstitial
    public void LoadInterstitial()
    {
        if (string.IsNullOrEmpty(interstitialAdUnitId))
        {
            Debug.LogWarning("AdManager: Interstitial Ad Unit Id is null/empty for this platform.");
            return;
        }

        if (interstitialLoaded || isInterstitialLoading)
        {
            Debug.Log("AdManager: Interstitial already loaded or loading, skipping LoadInterstitial.");
            return;
        }

        Debug.Log("AdManager: Loading interstitial: " + interstitialAdUnitId);
        isInterstitialLoading = true;
        Advertisement.Load(interstitialAdUnitId, this);
    }

    public void ShowInterstitial()
    {
        if (interstitialLoaded)
        {
            lastShowingAdUnitId = interstitialAdUnitId;
            Advertisement.Show(interstitialAdUnitId, this);
        }
        else
        {
            Debug.Log("AdManager: Interstitial not loaded. Requesting load.");
            LoadInterstitial();
        }
    }
    #endregion

    #region Rewarded
    public void LoadRewarded()
    {
        if (string.IsNullOrEmpty(rewardedAdUnitId))
        {
            Debug.LogWarning("AdManager: Rewarded Ad Unit Id is null/empty for this platform.");
            return;
        }

        if (rewardedLoaded || isRewardedLoading)
        {
            Debug.Log("AdManager: Rewarded already loaded or loading, skipping LoadRewarded.");
            return;
        }

        Debug.Log("AdManager: Loading rewarded ad: " + rewardedAdUnitId);
        isRewardedLoading = true;
        Advertisement.Load(rewardedAdUnitId, this);
    }

    public void ShowRewarded()
    {
        if (rewardedLoaded)
        {
            lastShowingAdUnitId = rewardedAdUnitId;
            Advertisement.Show(rewardedAdUnitId, this);
        }
        else
        {
            Debug.Log("AdManager: Rewarded ad not loaded. Requesting load.");
            LoadRewarded();
        }
    }
    #endregion

    #region IUnityAdsLoadListener
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("AdManager: OnUnityAdsAdLoaded: " + adUnitId);

        if (adUnitId.Equals(interstitialAdUnitId))
        {
            interstitialLoaded = true;
            isInterstitialLoading = false;
        }
        else if (adUnitId.Equals(rewardedAdUnitId))
        {
            rewardedLoaded = true;
            isRewardedLoading = false;
        }
        // Banners use BannerLoadOptions callbacks, not this
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"AdManager: Error loading Ad Unit {adUnitId}: {error} - {message}");

        if (adUnitId.Equals(interstitialAdUnitId))
        {
            interstitialLoaded = false;
            isInterstitialLoading = false;
        }
        else if (adUnitId.Equals(rewardedAdUnitId))
        {
            rewardedLoaded = false;
            isRewardedLoading = false;
        }
    }
    #endregion

    #region IUnityAdsShowListener
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"AdManager: Error showing Ad Unit {adUnitId}: {error} - {message}");

        if (adUnitId.Equals(interstitialAdUnitId))
        {
            interstitialLoaded = false;
            isInterstitialLoading = false;
        }
        else if (adUnitId.Equals(rewardedAdUnitId))
        {
            rewardedLoaded = false;
            isRewardedLoading = false;
        }
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        Debug.Log("AdManager: OnUnityAdsShowStart: " + adUnitId);
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        Debug.Log("AdManager: OnUnityAdsShowClick: " + adUnitId);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"AdManager: OnUnityAdsShowComplete: {adUnitId} - {showCompletionState}");

        if (adUnitId.Equals(interstitialAdUnitId))
        {
            interstitialLoaded = false;
            isInterstitialLoading = false;
            // Auto-load next
            LoadInterstitial();
        }
        else if (adUnitId.Equals(rewardedAdUnitId))
        {
            rewardedLoaded = false;
            isRewardedLoading = false;

            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                Debug.Log("AdManager: Rewarded ad completed - grant reward here.");
                GrantReward();
            }

            LoadRewarded();
        }

        lastShowingAdUnitId = null;
    }
    #endregion

    #region Reward handling
    private void GrantReward()
    {
        // Example: add 5 hint points
        GameInternal gi = FindObjectOfType<GameInternal>();
        if (gi != null)
        {
            gi.AddHintPoints(5);
        }
        else
        {
            Debug.LogWarning("AdManager: Could not find GameInternal to grant reward.");
        }
    }
    #endregion

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
