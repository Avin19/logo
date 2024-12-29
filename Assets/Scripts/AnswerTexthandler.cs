using UnityEngine;
using TMPro;


public class AnswerTexthandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textData;


    public void SetText(string text)
    {
        textData.text = text;
    }
    public string GetText()
    {
        return textData.text;
    }
}


