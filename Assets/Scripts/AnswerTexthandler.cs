using UnityEngine;
using TMPro;

public class AnswerTexthandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textData;

    private TextHandler source;

    public void SetText(string text, TextHandler sourceHandler = null)
    {
        textData.text = text;
        source = sourceHandler;
    }

    public string GetText()
    {
        return textData.text;
    }

    public TextHandler GetSource()
    {
        return source;
    }

    public void Clear()
    {
        textData.text = "";
        source = null;
    }
}