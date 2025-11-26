using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class APIHandler : MonoBehaviour
{
    [Header("Sheet / Local JSON")]

    [SerializeField] private string sheetName;

    private Button onbutton;
    [SerializeField] private Manager manager;
    private GameInternal gameInternal;



    private void Awake()
    {
        onbutton = GetComponent<Button>();

    }

    private void Start()
    {
        // If sheetName is empty, attempt to read from child TMP text
        if (string.IsNullOrEmpty(sheetName))
        {
            TextMeshProUGUI tmp = GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) sheetName = tmp.text;
        }
        if (manager == null)
        {
            manager = FindObjectOfType<Manager>();
        }


    }

    private void OnEnable()
    {
        if (onbutton == null)
        {
            Debug.LogError("[APIHandler] Button component not found on GameObject.");
            return;
        }

        onbutton.onClick.AddListener(OnButtonClick);

    }

    private void OnDisable()
    {

        onbutton.onClick.RemoveListener(OnButtonClick);

    }

    private void OnButtonClick()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();

        manager.SetCatorgoryToLoad(sheetName);
        manager.LoadingScreen(true);
        manager.StartGame();

    }
}