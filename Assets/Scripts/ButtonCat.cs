using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCat : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("Category")]
    [SerializeField] private CategorySO categorySO;

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

    public void SetCategorySO(CategorySO category)
    {
        categorySO = category;
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

        gameInternal.LoadCategoryById(categorySO);
    }
}