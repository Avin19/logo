using UnityEngine;
using TMPro;

public class InfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private TextMeshProUGUI hintTxt;

    // Start is called before the first frame update
    private void Start()
    {
        coinTxt.text = PlayerDataManager.Instance.Coins.ToString();
        hintTxt.text = PlayerDataManager.Instance.Hint.ToString();
    }
}
