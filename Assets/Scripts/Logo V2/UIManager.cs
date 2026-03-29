using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private Transform mainMenuPanel;
    [SerializeField] private Transform gamePanel;
    [SerializeField] private Transform winPanel;
    [SerializeField] private Transform lossPanel;
    [SerializeField] private Transform dailyPanel;
    [SerializeField] private Transform settingPanel;

    [Header(" Main Menu Button ")]
    [SerializeField] private Button playBtn;
    [SerializeField] private Button dailyBtn;
    [SerializeField] private Button leaderboardBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button exitBtn;

    [Header(" Game Panel Button")]
    [SerializeField] private Button removeLetterBtn;
    [SerializeField] private Button revealLetterBtn;
    [SerializeField] private Button skipBtn;
    [SerializeField] private Button backBtn;

    [Header(" Win Panel Button")]

    [SerializeField] private Button nextLevelBtn;

    [Header(" Loss Panel ")]

    [SerializeField] private Button tryAgainBtn;
    [SerializeField] private Button quitBtn;

    [Header(" Setting panel ")]

    [SerializeField] private Button closeBtn;

    [Header(" Daily Panel Button ")]

    [SerializeField] private Button daliyPlayBtn;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI mainMenuCoinText;
    [SerializeField] private TextMeshProUGUI gameLevelText;
    [SerializeField] private TextMeshProUGUI gameCoinText;

    [SerializeField] private PlayerData playerData;
    void OnEnable()
    {
        playBtn.onClick.AddListener(() => OnPlayButtonClicked());
        dailyBtn.onClick.AddListener(() => OnDailyButtonClick());
        leaderboardBtn.onClick.AddListener(() => OnLeaderboardButtonClick());
        settingBtn.onClick.AddListener(() => OnSettingButtonClick());
        exitBtn.onClick.AddListener(() => OnExitButtonClick());
        removeLetterBtn.onClick.AddListener(() => OnRemoveButtonClick());
        revealLetterBtn.onClick.AddListener(() => OnRevealButtonClick());
        skipBtn.onClick.AddListener(() => OnSkipButtonClick());
        backBtn.onClick.AddListener(() => OnBackButtonClick());
        nextLevelBtn.onClick.AddListener(() => OnNextLevelButtonCLick());
        tryAgainBtn.onClick.AddListener(() => OnTryAgainButtonClick());
        quitBtn.onClick.AddListener(() => OnQuitButtonClick());
        closeBtn.onClick.AddListener(() => OnCloseButtonClick());
        daliyPlayBtn.onClick.AddListener(() => OnPlayButtonClicked());

    }

    private void OnBackButtonClick()
    {
        SetAllPanelFalse();
        MainMenuPanelActive();
    }

    private void OnCloseButtonClick()
    {
        SetAllPanelFalse();
        MainMenuPanelActive();
    }

    private void OnQuitButtonClick()
    {
        SetAllPanelFalse();
        MainMenuPanelActive();
    }

    private void OnTryAgainButtonClick()
    {
        LoadNextItem();
    }

    private void OnNextLevelButtonCLick()
    {
        LoadNextItem();
    }

    private void LoadNextItem()
    {
        throw new NotImplementedException();
    }

    private void OnSkipButtonClick()
    {
        throw new NotImplementedException();
    }

    private void OnRevealButtonClick()
    {
        throw new NotImplementedException();
    }

    private void OnRemoveButtonClick()
    {
        throw new NotImplementedException();
    }

    private void OnExitButtonClick()
    {
        Application.Quit();
    }

    private void OnSettingButtonClick()
    {
        SetAllPanelFalse();
        SettingPanelActive();
    }

    private void OnLeaderboardButtonClick()
    {
        //LeaderPanel.gameObject.SetActive(true);
    }

    private void OnDailyButtonClick()
    {
        SetAllPanelFalse();
        DailyPanelActive();
    }

    void OnDisable()
    {
        playBtn.onClick.RemoveAllListeners();
    }
    // Start is called before the first frame update
    void Start()
    {

        if (playerData.playerID == string.Empty)
        {

            playerData.playerID = Guid.NewGuid().ToString("N");
            Debug.Log(Guid.NewGuid().ToString("N"));

        }
        SetAllPanelFalse();
        MainMenuPanelActive();

    }

    private void OnPlayButtonClicked()
    {
        SetAllPanelFalse();
        GamePanelActive();
    }

    private void MainMenuPanelActive()
    {
        mainMenuPanel.gameObject.SetActive(true);
        mainMenuCoinText.text = playerData.coin.ToString("000");
    }
    private void GamePanelActive()
    {
        gamePanel.gameObject.SetActive(true);
    }
    private void WinPanelActive()
    {
        winPanel.gameObject.SetActive(true);
    }
    private void LossPanelActive()
    {
        lossPanel.gameObject.SetActive(true);
    }
    private void DailyPanelActive()
    {
        dailyPanel.gameObject.SetActive(true);
    }
    private void SettingPanelActive()
    {
        settingPanel.gameObject.SetActive(true);
    }

    private void SetAllPanelFalse()
    {
        mainMenuPanel.gameObject.SetActive(false);
        gamePanel.gameObject.SetActive(false);
        winPanel.gameObject.SetActive(false);
        lossPanel.gameObject.SetActive(false);
        dailyPanel.gameObject.SetActive(false);
        settingPanel.gameObject.SetActive(false);
    }

    // Update is called once per frame

}
