using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class ButtonCat : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    private Button onbutton;

    [SerializeField] private CategorySO categorySO;
    [SerializeField] private GameInternal gameInternal;
    [SerializeField] private GameObject loadingpanel;

    private void Awake()
    {
        onbutton = GetComponent<Button>();
    }
    public void SetCategorySO(CategorySO _categorySO)
    {
        categorySO = _categorySO;
    }
    public void SetTextToButton(string _buttonText)
    {
        buttonText.text = _buttonText;
    }
    void OnEnable()
    {
        onbutton.onClick.AddListener(OnButtonClick);
    }
    public void SetGameInternal(GameInternal _gameInternal)
    {
        gameInternal = _gameInternal;
    }
    public void SetLoadingPanel(GameObject _loadingpanel)
    {
        loadingpanel = _loadingpanel;
    }
    private void OnButtonClick()
    {
        gameInternal.gameObject.SetActive(true);
        loadingpanel.SetActive(true);
        gameInternal.LoadCategoryById(categorySO);
    }
}