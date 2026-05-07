using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;


public class Manager : MonoBehaviour
{
    [Header("Manager")]

    [SerializeField] private Transform levelHolder;

    [Header("UI Panels")]
    [SerializeField] private Transform mainPanel;
    [SerializeField] private Transform welcomePanel;
    [SerializeField] private Transform levelPanel;
    [SerializeField] private Transform loadingPanel;
    [SerializeField] private Transform settingPanel;

    [Header("Buttons / Prefabs")]
    [SerializeField] private Button startBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button settingbtn;
    [SerializeField] private Button backToLevel;
    [SerializeField] private Button backSettingToMenu;
    [SerializeField] private Button backToLevelMenu;
    [SerializeField] private GameObject pfButton;

    [Header("Ads Settings")]
    [Tooltip("Show interstitial every N times Start button is pressed")]
    [SerializeField] private int interstitialFrequency = 3;
    [Tooltip("Optional - wire a button in the Inspector to this for rewarded ad")]
    [SerializeField] private Button rewardedButton;

    [SerializeField] private int webRequestTimeout = 10;

    [SerializeField] private List<CategorySO> categorySOs;

    private const string INTERSTITIAL_COUNT_KEY = "interstitial_count";

    // store listeners so we can remove them



    private void OnEnable()
    {

        startBtn.onClick.AddListener(StartButtonClick);
        settingbtn.onClick.AddListener(SettingButton);
        backSettingToMenu.onClick.AddListener(BackToMenu);
        backToLevelMenu.onClick.AddListener(BackToMenu);
        backToLevel.onClick.AddListener(BackToLevel);
        rewardedButton.onClick.AddListener(ShowRewardedFromUI);
        quitBtn.onClick.AddListener(() => Application.Quit());


    }


    private void StartButtonClick()
    {

        StartButton();
    }

    private void OnDisable()
    {
        rewardedButton.onClick.RemoveListener(ShowRewardedFromUI);


    }

    private void Start()
    {
        SetAllThePanelFalse();
        if (welcomePanel != null) welcomePanel.gameObject.SetActive(true);
        PlayerDataManager.Instance.Load();
    }


    private void BackToMenu()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();
        SetAllThePanelFalse();
        if (welcomePanel != null) welcomePanel.gameObject.SetActive(true);

        AdMobManager.Instance.TryShowInterstitial();

    }

    private void SettingButton()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();
        SetAllThePanelFalse();
        if (settingPanel != null) settingPanel.gameObject.SetActive(true);
    }

    private void BackToLevel()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();
        // noop, just keep
        SetAllThePanelFalse();
        if (levelPanel != null) levelPanel.gameObject.SetActive(true);



    }


    public void LoadingData()
    {
        SetAllThePanelFalse();
        if (loadingPanel != null) loadingPanel.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        SetAllThePanelFalse();
        if (mainPanel != null) mainPanel.gameObject.SetActive(true);


    }


    public void SetAllThePanelFalse()
    {
        if (welcomePanel != null) welcomePanel.gameObject.SetActive(false);
        if (mainPanel != null) mainPanel.gameObject.SetActive(false);
        if (levelPanel != null) levelPanel.gameObject.SetActive(false);
        if (settingPanel != null) settingPanel.gameObject.SetActive(false);
        if (loadingPanel != null) loadingPanel.gameObject.SetActive(false);
    }

    public void LevelLoaded()
    {
        SetAllThePanelFalse();
        if (levelPanel != null) levelPanel.gameObject.SetActive(true);
        LoadCategoeries();

    }

    public void LoadingScreen(bool set)
    {
        if (loadingPanel != null) loadingPanel.gameObject.SetActive(set);
    }

    private void StartButton()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();

        int count = PlayerPrefs.GetInt(INTERSTITIAL_COUNT_KEY, 0) + 1;
        PlayerPrefs.SetInt(INTERSTITIAL_COUNT_KEY, count);
        PlayerPrefs.Save();

        bool shouldShowInterstitial = interstitialFrequency > 0 && (count % interstitialFrequency == 0);
        LoadingData();
        Invoke(nameof(LevelLoaded), 2f);

    }


    public void ShowRewardedFromUI()
    {


    }
    private void LoadCategoeries()
    {
        foreach (CategorySO categorySO in categorySOs)
        {
            GameObject catrgoryBtn = Instantiate(pfButton, levelHolder);
            catrgoryBtn.GetComponent<ButtonCat>().SetTextToButton(categorySO.category);
            catrgoryBtn.GetComponent<ButtonCat>().SetCategorySO(categorySO);
            catrgoryBtn.GetComponent<ButtonCat>().SetGameInternal(mainPanel.GetComponent<GameInternal>());
            catrgoryBtn.GetComponent<ButtonCat>().SetLoadingPanel(loadingPanel.gameObject);
        }

    }


    private List<string> GetTopLevelKeys(string json)
    {
        var keys = new List<string>();

        try
        {
            var root = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            foreach (var kvp in root)
            {
                keys.Add(kvp.Key);  // "cars", "countries"
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[Manager] Failed to read top-level keys: " + ex.Message);
        }

        return keys;
    }



}
