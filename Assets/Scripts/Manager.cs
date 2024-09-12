using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [Header("  UI Panels")]
    [SerializeField] private Transform mainPanel;
    [SerializeField] private Transform welcomePanel;
    [SerializeField] private Transform levelPanel;

    [Header("Buttons")]
    [SerializeField] private Button startBtn;
    [SerializeField] private Button quitBtn;


    private void OnEnable()
    {
        startBtn.onClick.AddListener(() =>
        {
            SetAllThePanelFalse();
            levelPanel.gameObject.SetActive(true);
        });
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
    }




}
