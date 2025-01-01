using UnityEngine;
using GoogleMobileAds.Api;



public class Ads : MonoBehaviour
{
    private string _adUnitId = "ca-app-pub-6056814275300024/8497003128";


    private BannerView _bannerView;
    private InterstitialAd interstitialAd;
    private void Start()
    {

        MobileAds.Initialize(initStatus => { });


        RequestBanner();


    }

    private void RequestBanner()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
        }

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        _bannerView = new BannerView(_adUnitId, adaptiveSize, AdPosition.Bottom);
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


}


