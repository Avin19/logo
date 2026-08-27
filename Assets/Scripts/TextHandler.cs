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

        if (btn != null)
            btn.onClick.AddListener(OnClick);
    }

    public void SetGameInternal(GameInternal g)
    {
        game = g;
    }

    void OnClick()
    {
        if (game == null) return;
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

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }
}