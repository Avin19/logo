using UnityEngine;
using TMPro;



public class TextHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textData;


    public void SetText(string text)
    {
        textData.text = text;
    }

}


