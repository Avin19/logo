using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour, IUnityAdsInitializationListener
{

    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;
    [SerializeField] string _androidAdUnityID = "Banner_Android";
    [SerializeField] string gameID = "5824579";

    [SerializeField] bool testMode = true;
    void Awake()
    {

        InitializeAds();
    }
    public void InitializeAds()
    {

        Advertisement.Initialize(gameID, testMode, this);

    }

    public void LoadBanner()
    {
        Advertisement.Banner.SetPosition(_bannerPosition);
        Advertisement.Banner.Load(_androidAdUnityID);
        ShowBannerAd();
    }

    private void ShowBannerAd()
    {
        Advertisement.Banner.Show(_androidAdUnityID);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Initialization completed ");
        LoadBanner();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log("Initialization failed " + message);
    }
}
