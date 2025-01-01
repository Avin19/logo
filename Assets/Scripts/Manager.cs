using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using System;


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
    }

    private void BackToMenu()
    {
        SoundManager.Instance.ButtonClick();
        SetAllThePanelFalse();
        welcomePanel.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        startBtn.onClick.RemoveListener(StartButton);
        settingbtn.onClick.RemoveListener(SettingButton);
        backToLevel.onClick.RemoveListener(BackToLevel);
        backToMenu.onClick.RemoveListener(BackToMenu);
        backToLevel.onClick.RemoveListener(BackToMenu);
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
    private void Start()
    {
        SetAllThePanelFalse();
        welcomePanel.gameObject.SetActive(true);
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
    }
    public void LoadingScreen(bool set)
    {
        loadingPanel.gameObject.SetActive(set);
    }




}
