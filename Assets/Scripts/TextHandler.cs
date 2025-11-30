using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextHandler : MonoBehaviour
{
    private TextMeshProUGUI txt;
    private Button btn;
    private GameInternal game;

    void Awake()
    {
        txt = GetComponentInChildren<TextMeshProUGUI>();
        btn = GetComponent<Button>();
        game = FindObjectOfType<GameInternal>();

        btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (game == null) return;

        // forward THIS tile to GameInternal
        game.ButtonClicked(this);
    }

    public void SetText(string t)
    {
        if (txt != null)
            txt.text = t;
    }

    public string GetText()
    {
        return txt != null ? txt.text : string.Empty;
    }

    public void SetInteractable(bool state)
    {
        if (btn != null)
            btn.interactable = state;
    }
}
