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
        button.onClick.RemoveListener(OnButtonClick);
        button.GetComponent<Image>().color = Color.gray;
        GameObject.FindAnyObjectByType<GameInternal>().ButtonClicked(this);
    }
}


