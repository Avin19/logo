using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SplashScreenAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform logo;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private TMP_Text percentageText;

    [Header("Progress")]
    [SerializeField] private float loadingDuration = 3f;

    [Header("Logo Animation")]
    [SerializeField] private float logoStartScale = 0.75f;
    [SerializeField] private float logoScale = 1f;

    private Sequence logoSequence;

    private void Start()
    {
        Setup();
        PlayLogoAnimation();
        PlayLoadingAnimation();
    }

    private void Setup()
    {
        progressSlider.value = 0f;

        logo.localScale = Vector3.one * logoStartScale;

        if (loadingText != null)
            loadingText.text = "Loading...";

        if (percentageText != null)
            percentageText.text = "0%";
    }

    private void PlayLogoAnimation()
    {
        logoSequence = DOTween.Sequence();

        // Initial logo entrance
        logoSequence.Append(
            logo.DOScale(logoScale, 0.65f)
                .SetEase(Ease.OutBack)
        );

        // Small pause
        logoSequence.AppendInterval(0.1f);

        // Continuous breathing
        logoSequence.Append(
            logo.DOScale(1.035f, 0.9f)
                .SetEase(Ease.InOutSine)
        );

        logoSequence.Append(
            logo.DOScale(1f, 0.9f)
                .SetEase(Ease.InOutSine)
        );

        logoSequence.SetLoops(-1);
    }

    private void PlayLoadingAnimation()
    {
        progressSlider
            .DOValue(1f, loadingDuration)
            .SetEase(Ease.InOutSine)
            .OnUpdate(UpdateProgressText)
            .OnComplete(OnLoadingComplete);
    }

    private void UpdateProgressText()
    {
        int percentage = Mathf.RoundToInt(progressSlider.value * 100f);

        if (percentageText != null)
            percentageText.text = percentage + "%";

        UpdateLoadingMessage(percentage);
    }

    private void UpdateLoadingMessage(int percentage)
    {
        if (loadingText == null)
            return;

        if (percentage < 25)
            loadingText.text = "Preparing logos...";

        else if (percentage < 50)
            loadingText.text = "Loading challenge...";

        else if (percentage < 75)
            loadingText.text = "Getting your brain ready...";

        else if (percentage < 100)
            loadingText.text = "Almost ready...";

        else
            loadingText.text = "Let's play!";
    }

    private void OnLoadingComplete()
    {
        logoSequence?.Kill();

        // Small final celebration
        logo
            .DOPunchScale(
                Vector3.one * 0.08f,
                0.35f,
                5,
                0.5f
            );
    }

    private void OnDestroy()
    {
        logoSequence?.Kill();
    }
}