using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using System;
using GoogleMobileAds.Api;

// Work on setting panel is remaining 
public class Manager : MonoBehaviour
{
    [Header("Manager")]
    [SerializeField] private GameInternal gameInternal;
    [SerializeField] private Transform levelHolder;

    [Header("  UI Panels")]
    [SerializeField] private Transform mainPanel;
    [SerializeField] private Transform welcomePanel;
    [SerializeField] private Transform levelPanel;
    [SerializeField] private Transform loadingPanel;
    [SerializeField] private Transform settingPanel;

    [Header("Buttons")]
    [SerializeField] private Button startBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button settingbtn;
    [SerializeField] private Button backToLevel;
    [SerializeField] private Button backToMenu;
    [SerializeField] private Button backToLevelMenu;
    [SerializeField] private GameObject pfButton;

    [Header("Ads Settings")]
    [Tooltip("Show interstitial every N times Start button is pressed")]
    [SerializeField] private int interstitialFrequency = 3;
    [Tooltip("Optional - wire a button in the Inspector to this for rewarded ad")]
    [SerializeField] private Button rewardedButton;

    private const string INTERSTITIAL_COUNT_KEY = "interstitial_count";

    private void OnEnable()
    {
        startBtn.onClick.AddListener(StartButton);
        quitBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.ButtonClick();
            Application.Quit();
        });
        settingbtn.onClick.AddListener(SettingButton);
        backToLevel.onClick.AddListener(BackToLevel);
        backToMenu.onClick.AddListener(BackToMenu);
        backToLevelMenu.onClick.AddListener(BackToMenu);

        if (rewardedButton != null)
            rewardedButton.onClick.AddListener(ShowRewardedFromUI);

        // Subscribe to reward callback if AdManager provides it (safe null-checks)
        if (AdManager.Instance != null)
        {
            AdManager.Instance.OnUserEarnedReward += HandleReward;
        }
    }

    private void OnDisable()
    {
        startBtn.onClick.RemoveListener(StartButton);
        settingbtn.onClick.RemoveListener(SettingButton);
        backToLevel.onClick.RemoveListener(BackToLevel);
        backToMenu.onClick.RemoveListener(BackToMenu);
        backToLevel.onClick.RemoveListener(BackToMenu);

        if (rewardedButton != null)
            rewardedButton.onClick.RemoveListener(ShowRewardedFromUI);

        if (AdManager.Instance != null)
        {
            AdManager.Instance.OnUserEarnedReward -= HandleReward;
        }
    }

    private void Start()
    {
        SetAllThePanelFalse();
        welcomePanel.gameObject.SetActive(true);

        // Show a banner on the main menu (if AdManager exists)
        if (AdManager.Instance != null)
        {
            AdManager.Instance.ShowBanner();
        }
    }

    private void BackToMenu()
    {
        SoundManager.Instance.ButtonClick();
        SetAllThePanelFalse();
        welcomePanel.gameObject.SetActive(true);

        // Optionally show banner again
        if (AdManager.Instance != null)
        {
            AdManager.Instance.ShowBanner();
        }
    }

    private void SettingButton()
    {
        SoundManager.Instance.ButtonClick();

        SetAllThePanelFalse();
        settingPanel.gameObject.SetActive(true);
    }

    private void BackToLevel()
    {
        SoundManager.Instance.ButtonClick();
        gameInternal.Restart();
        SetAllThePanelFalse();
        levelPanel.gameObject.SetActive(true);
    }

    public void LoadingData()
    {
        SetAllThePanelFalse();
        loadingPanel.gameObject.SetActive(true);
    }

    private void StartButton()
    {
        SoundManager.Instance.ButtonClick();

        // Increment the counter and decide whether to show an interstitial
        int count = PlayerPrefs.GetInt(INTERSTITIAL_COUNT_KEY, 0) + 1;
        PlayerPrefs.SetInt(INTERSTITIAL_COUNT_KEY, count);
        PlayerPrefs.Save();

        bool shouldShowInterstitial = interstitialFrequency > 0 && (count % interstitialFrequency == 0);

        if (shouldShowInterstitial && AdManager.Instance != null)
        {
            // Show interstitial and continue flow. If not ready, the AdManager will try to load it.
            AdManager.Instance.ShowInterstitial();
        }

        LoadingData();
        StartCoroutine(LoadCatgories());
    }

    private IEnumerator LoadCatgories()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://sheets.googleapis.com/v4/spreadsheets/1pYU1mu9NBDYt3Ls_IYxMtbnaNrJ_t2jZxy7MYGFLjEA/values/Sheet1?key=AIzaSyAA23WLN6TWfFj_J1VXvYPUOCIMSXGo254");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Offline");
        }
        else
        {
            string data = request.downloadHandler.text;

            RootObject c = JsonConvert.DeserializeObject<RootObject>(data);
            // Debug.Log(c.values[0].Length);
            if (c.values[0].Length == levelHolder.childCount)
            {

            }
            else
            {
                foreach (string cat in c.values[0])
                {
                    GameObject Button = Instantiate(pfButton, levelHolder);
                    Button.transform.GetComponentInChildren<TextMeshProUGUI>().text = cat;
                }
            }
            LevelLoaded();
        }
    }

    public void StartGame()
    {
        SetAllThePanelFalse();
        mainPanel.gameObject.SetActive(true);
    }

    public void SetAllThePanelFalse()
    {
        welcomePanel.gameObject.SetActive(false);
        mainPanel.gameObject.SetActive(false);
        levelPanel.gameObject.SetActive(false);
        settingPanel.gameObject.SetActive(false);
        loadingPanel.gameObject.SetActive(false);
    }

    public void LevelLoaded()
    {
        SetAllThePanelFalse();
        levelPanel.gameObject.SetActive(true);

        // optional: hide the menu banner when entering level (if you want)
        // if (AdManager.Instance != null) AdManager.Instance.HideBanner();
    }

    public void LoadingScreen(bool set)
    {
        loadingPanel.gameObject.SetActive(set);
    }

    // Public method you can wire to a UI button to show a rewarded ad
    public void ShowRewardedFromUI()
    {
        if (AdManager.Instance != null)
        {
            AdManager.Instance.ShowRewarded();
        }
        else
        {
            Debug.LogWarning("AdManager.Instance is null – rewarded ad not available.");
        }
    }

    // Called when user earns a reward. You can grant in-game currency, skip a level, etc.
    private void HandleReward(Reward reward)
    {
        Debug.Log($"Grant reward: {reward.Type} x {reward.Amount}");
        // TODO: grant in-game reward here. Example:
        // gameInternal.GiveExtraHint((int)reward.Amount);
    }

}
