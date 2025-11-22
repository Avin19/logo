using UnityEngine;
using UnityEngine.Advertisements;
public class AdManager : MonoBehaviour, IUnityAdsInitializationListener
{

    [SerializeField] private BannerAd bannerAd;
    [SerializeField] private InterstitialAd interstitialAd;
    [SerializeField] private RewardedAds rewardedAds;

    public static AdManager Instance { get; private set; }
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    [SerializeField] private string _gameId;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        InitializeAds();
    }
    public void InitializeAds()
    {
#if UNITY_IOS
    _gameId = _iOSGameId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
#elif UNITY_EDITOR
    _gameId = _androidGameId; //Only for testing the functionality in the Editor
#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        bannerAd.LoadBanner();
        interstitialAd.LoadAd();
        rewardedAds.LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    // Start is called before the first frame update
    void Start()
    {
        bannerAd.ShowBannerAd();
    }

    public void ShowBanner()
    {
        bannerAd.ShowBannerAd();
    }

    // Update is called once per frame
    public void ShowInterstital()
    {
        interstitialAd.ShowAd();
    }
    public void ShowReward()
    {
        rewardedAds.ShowAd();
    }
}
