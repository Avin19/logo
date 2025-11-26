using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class APIHandler : MonoBehaviour
{
    [Header("Sheet / Local JSON")]

    [SerializeField] private string sheetName;

    private Button onbutton;

    private UnityEngine.Events.UnityAction clickAction;

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

    }

    private void OnEnable()
    {
        if (onbutton == null)
        {
            Debug.LogError("[APIHandler] Button component not found on GameObject.");
            return;
        }

        onbutton.onClick.AddListener(clickAction);
        clickAction = () => OnButtonClick(sheetName);
    }

    private void OnDisable()
    {
        if (onbutton != null && clickAction != null)
        {
            onbutton.onClick.RemoveListener(clickAction);
        }
    }

    private void OnButtonClick(string _sheetName)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();


        // levelManager.StartCoroutine(LoadLocalOnly(_sheetName));
    }
}