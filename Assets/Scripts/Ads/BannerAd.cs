using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour
{
    // For the purpose of this example, these buttons are for functionality testing:


    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    [SerializeField] string _androidAdUnitId = "Banner_Android";
    [SerializeField] string _iOSAdUnitId = "Banner_iOS";
    string _adUnitId = null;
    private bool isLoaded = false;// This will remain null for unsupported platforms.

    void Start()
    {
        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
    _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif

        // Disable the button until an ad is ready to show:


        // Set the banner position:
        Advertisement.Banner.SetPosition(_bannerPosition);


    }

    // Implement a method to call when the Load Banner button is clicked:
    public void LoadBanner()
    {
        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(_adUnitId, options);
    }

    // Implement code to execute when the loadCallback event triggers:
    void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");

        isLoaded = true;

    }

    // Implement code to execute when the load errorCallback event triggers:
    void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
        isLoaded = false;
        LoadBanner();
        // Optionally execute additional code, such as attempting to load another ad.
    }

    // Implement a method to call when the Show Banner button is clicked:
    public void ShowBannerAd()
    {
        if (isLoaded)
        // Set up options to notify the SDK of show events:
        {
            BannerOptions options = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                hideCallback = OnBannerHidden,
                showCallback = OnBannerShown
            };

            Advertisement.Banner.Show(_adUnitId, options);
        }
        else
        {
            LoadBanner();
        }

        // Show the loaded Banner Ad Unit:
    }

    // Implement a method to call when the Hide Banner button is clicked:
    void HideBannerAd()
    {
        // Hide the banner:
        Advertisement.Banner.Hide();
    }

    void OnBannerClicked() { }
    void OnBannerShown() { }
    void OnBannerHidden() { }

    void OnDestroy()
    {

    }
}
