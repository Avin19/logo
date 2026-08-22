using TMPro;
using UnityEngine;

public class ProfileUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelTxt;
    [SerializeField] private TextMeshProUGUI playerTxt;


    void Start()
    {
        levelTxt.text = $"Level " + PlayerDataManager.Instance.data.CurrentLevel;
        playerTxt.text = PlayerDataManager.Instance.data.playerType.ToString();
    }
}
