using UnityEngine;
using TMPro;
using UnityEngine.UI;



public class TextHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textData;

    private Button button;


    public void SetText(string text)
    {
        textData.text = text;
    }
    public string GetText()
    {
        return textData.text;
    }

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        GameObject.FindAnyObjectByType<GameInternal>().ButtonClicked(this);
    }
}


