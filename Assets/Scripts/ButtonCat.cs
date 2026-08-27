using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCat : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI progressText;


    [Header("Category")]
    [SerializeField] private CategorySO categorySO;
    [SerializeField] private Transform levelPanel;

    private Button onButton;
    private GameInternal gameInternal;

    private void Awake()
    {
        onButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (onButton != null)
            onButton.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        if (onButton != null)
            onButton.onClick.RemoveListener(OnButtonClick);
    }

    public void SetCategorySO(CategorySO _categorySO)
    {
        categorySO = _categorySO;

        UpdateProgress();
    }
    private void UpdateProgress()
    {
        if (categorySO == null)
            return;

        int solved =
            PlayerDataManager.Instance
                .GetCategorySolvedCount(
                    categorySO.category
                );

        int total =
            categorySO.logos.Length;

        if (progressText != null)
        {
            progressText.text =
                $"{solved}/{total}";
        }
    }

    public void SetTextToButton(string text)
    {
        if (buttonText != null)
            buttonText.text = text;
    }

    public void SetGameInternal(GameInternal internalManager)
    {
        gameInternal = internalManager;
    }
    public void SetLevelPanel(Transform _levelPanel)
    {
        levelPanel = _levelPanel;
    }
    private void OnButtonClick()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.ButtonClick();

        if (gameInternal == null)
        {
            Debug.LogError(
                $"GameInternal is not assigned for {gameObject.name}"
            );

            return;
        }

        if (categorySO == null)
        {
            Debug.LogError(
                $"CategorySO is not assigned for {gameObject.name}"
            );

            return;
        }

        gameInternal.gameObject.SetActive(true);

        if (levelPanel != null)
            levelPanel.gameObject.SetActive(false);

        gameInternal.LoadCategoryById(categorySO);
    }
}