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
        game = FindFirstObjectByType<GameInternal>();

        if (btn != null)
            btn.onClick.AddListener(OnClick);
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