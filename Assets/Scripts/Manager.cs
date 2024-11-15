using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;


// Work on setting panel is remaining 
public class Manager : MonoBehaviour
{
    [Header("  UI Panels")]
    [SerializeField] private Transform mainPanel;
    [SerializeField] private Transform welcomePanel;
    [SerializeField] private Transform levelPanel;
    //[SerializeField] private Transform settingPanel;

    [Header("Buttons")]
    [SerializeField] private Button startBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button settingbtn;

    [SerializeField] private GameObject pfButton;






    private void OnEnable()
    {
        startBtn.onClick.AddListener(StartButton);
        quitBtn.onClick.AddListener(() => { Application.Quit(); });
        settingbtn.onClick.AddListener(SettingButton);
    }
    private void SettingButton()
    {
        SetAllThePanelFalse();
        // settingPanel.gameObject.SetActive(true);

    }
    private void StartButton()
    {
        SetAllThePanelFalse();
        StartCoroutine(LoadCatgories());
        levelPanel.gameObject.SetActive(true);
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
            foreach (string cat in c.values[0])
            {
                GameObject Button = Instantiate(pfButton, levelPanel);
                Button.transform.GetComponentInChildren<TextMeshProUGUI>().text = cat;
            }
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
        settingbtn.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        startBtn.onClick.RemoveListener(StartButton);
        settingbtn.onClick.RemoveListener(SettingButton);
    }


}
