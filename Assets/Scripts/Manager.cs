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
    [SerializeField] private Transform settingPanel;

    [Header("Buttons / Prefabs")]
    [SerializeField] private Button startBtn;
    [SerializeField] private Button cateBtn;
    [SerializeField] private Button settingbtn;
    [SerializeField] private Button backToLevel;
    [SerializeField] private Button backSettingToMenu;
    [SerializeField] private Button backToLevelMenu;
    [SerializeField] private Button categoryBtn;
    [SerializeField] private GameObject pfButton;

    [Header("Ads Settings")]
    [Tooltip("Show interstitial every N times Start button is pressed")]
    [SerializeField] private int interstitialFrequency = 3;
    [Tooltip("Optional - wire a button in the Inspector to this for rewarded ad")]
    [SerializeField] private Button rewardedButton;


    [SerializeField] private List<CategorySO> categorySOs;

    [SerializeField] private GameInternal gameInternal;
    [SerializeField] private List<GameObject> pfBtns = new List<GameObject>();
    // store listeners so we can remove them



    private void OnEnable()
    {
        startBtn.onClick.AddListener(StartButtonClick);
        cateBtn.onClick.AddListener(StartButtonClick);
        settingbtn.onClick.AddListener(SettingButton);
        backSettingToMenu.onClick.AddListener(CloseSettings);
        backToLevelMenu.onClick.AddListener(BackToMenu);
        backToLevel.onClick.AddListener(BackToLevel);

        if (rewardedButton != null)
            rewardedButton.onClick.AddListener(
                ShowRewardedFromUI
            );

        if (categoryBtn != null)
            categoryBtn.onClick.AddListener(
                CategoryPanel
            );
    }

    private void OnDisable()
    {
        startBtn.onClick.RemoveListener(StartButtonClick);
        cateBtn.onClick.RemoveListener(StartButtonClick);
        settingbtn.onClick.RemoveListener(SettingButton);
        backSettingToMenu.onClick.RemoveListener(BackToMenu);
        backToLevelMenu.onClick.RemoveListener(BackToMenu);
        backToLevel.onClick.RemoveListener(BackToLevel);

        if (rewardedButton != null)
            rewardedButton.onClick.RemoveListener(
                ShowRewardedFromUI
            );

        if (categoryBtn != null)
            categoryBtn.onClick.RemoveListener(
                CategoryPanel
            );
    }
    private void CategoryPanel()
    {
        RandomLoadCategories();
    }

    private void StartButtonClick()
    {

        StartButton();
    }



    private void Start()
    {
        SetAllThePanelFalse();
        if (welcomePanel != null) welcomePanel.gameObject.SetActive(true);
        PlayerDataManager.Instance.Load();
        // pfBtns.Clear();
    }


    private void BackToMenu()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();
        SetAllThePanelFalse();
        if (welcomePanel != null) welcomePanel.gameObject.SetActive(true);
        for (int i = 0; i < levelHolder.childCount; i++)
        {
            Destroy(levelHolder.GetChild(i).gameObject);
        }

        AdMobManager.Instance.TryShowInterstitial();

    }
    private void SettingButton()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.ButtonClick();

        SetAllThePanelFalse();

        if (settingPanel != null)
        {
            settingPanel.gameObject.SetActive(true);

            SettingsPanelAnimation animation =
                settingPanel.GetComponent<SettingsPanelAnimation>();

            if (animation != null)
            {
                animation.PlayOpenAnimation();
            }
        }
    }
    public void CloseSettings()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.ButtonClick();

        SettingsPanelAnimation animation =
            settingPanel.GetComponent<SettingsPanelAnimation>();

        if (animation != null)
        {
            animation.PlayCloseAnimation(() =>
            {
                settingPanel.gameObject.SetActive(false);

                if (welcomePanel != null)
                    welcomePanel.gameObject.SetActive(true);
            });
        }
        else
        {
            settingPanel.gameObject.SetActive(false);

            if (welcomePanel != null)
                welcomePanel.gameObject.SetActive(true);
        }
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
    }

    public void LevelLoaded()
    {
        SetAllThePanelFalse();
        if (levelPanel != null) levelPanel.gameObject.SetActive(true);
        LoadCategoeries();

    }


    private void StartButton()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();
        LoadingData();
        LevelLoaded();

    }


    public void ShowRewardedFromUI()
    {


    }
    private void LoadCategoeries()
    {
        // Clear previous category buttons
        for (int i = levelHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(levelHolder.GetChild(i).gameObject);
        }

        for (int i = 0; i < categorySOs.Count; i++)
        {
            CategorySO categorySO = categorySOs[i];

            GameObject categoryButton =
                Instantiate(pfButton, levelHolder);

            // --------------------------------
            // Setup category data
            // --------------------------------

            ButtonCat buttonCat =
                categoryButton.GetComponent<ButtonCat>();

            buttonCat.SetTextToButton(
                categorySO.category
            );

            buttonCat.SetCategorySO(
                categorySO
            );

            buttonCat.SetGameInternal(
                mainPanel.GetComponent<GameInternal>()
            );

            // --------------------------------
            // DOTween animation
            // --------------------------------

            CategoryButtonAnimation animation =
                categoryButton.GetComponent<CategoryButtonAnimation>();

            if (animation != null)
            {
                int row = i / 2;

                float delay =
                    0.1f +
                    row * 0.12f;

                animation.PlayEntrance(delay);
            }
        }
    }
    private void RandomLoadCategories()
    {
        gameInternal.gameObject.SetActive(true);
        gameInternal.LoadCategoryById(categorySOs[UnityEngine.Random.Range(0, categorySOs.Count)]);
    }






}
