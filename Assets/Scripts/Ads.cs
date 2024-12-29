using UnityEngine;
using GoogleMobileAds.Api;



public class Ads : MonoBehaviour
{
#if UNITY_ANDROID
    private string _appId = "ca-app-pub-6056814275300024~2123166469";
    private string _adUnitId = "ca-app-pub-3940256099942544/6300978111";
#else
  private string _adUnitId = "unused";
#endif
    private BannerView _bannerView;
    private InterstitialAd interstitialAd;
    private void Start()
    {
        // Initialize the Google Mobile Ads SDK

        MobileAds.Initialize(initStatus => { });

        // Load banner Ads 

        RequestBanner();

        // Load Interstitial Ad

        //RequestInterstitial();
    }

    private void RequestBanner()
    {
        _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest();
        _bannerView.LoadAd(request);
        _bannerView.Show();
    }

    private void RequestInterstitial()
    {
        AdRequest request = new AdRequest();

        InterstitialAd.Load(_adUnitId, request, (InterstitialAd ad, LoadAdError error) => { interstitialAd = ad; });
        interstitialAd.Show();
    }

    // listen to event the banner view may raise

    private void ListenToAdEvent()
    {
        // Raised when an ad is laoded into the banner view.

    }

}


