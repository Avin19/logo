using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [Header("  UI Panels")]
    [SerializeField] private Transform mainPanel;
    [SerializeField] private Transform welcomePanel;
    [SerializeField] private Transform levelPanel;
    [SerializeField] private Transform settingPanel;

    [Header("Buttons")]
    [SerializeField] private Button startBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button settingbtn;



    private void OnEnable()
    {
        startBtn.onClick.AddListener(StartButton);
        quitBtn.onClick.AddListener(() => { Application.Quit(); });
        settingbtn.onClick.AddListener(SettingButton);
    }
    private void SettingButton()
    {
        SetAllThePanelFalse();
        settingPanel.gameObject.SetActive(true);

    }
    private void StartButton()
    {
        SetAllThePanelFalse();
        levelPanel.gameObject.SetActive(true);
    }
    private void Start()
    {
        welcomePanel.gameObject.SetActive(true);
    }

    private void SetAllThePanelFalse()
    {
        welcomePanel.gameObject.SetActive(false);
        mainPanel.gameObject.SetActive(false);
        levelPanel.gameObject.SetActive(false);
        settingbtn.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        startBtn.onClick.RemoveListener(StartButton);
        settingbtn.onClick.RemoveListener(SettingButton);
    }


}
