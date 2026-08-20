using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class BladeootstrapLoadingUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform logo;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI percentageText;

    [Header("Loading Setting")]
    [SerializeField] private float totalLoadingDuration = 5f;

    [Header("Logo Animation")]
    [SerializeField] private float logoStartScale = 0.8f;
    [SerializeField] private float logoTargetScale = 1f;
    [SerializeField] private float logoIntroDuration = 0.7f;

    [Header("Logo Idle Animation")]
    [SerializeField] private float logoBreathScale = 1.025f;
    [SerializeField] private float logoBreathDuration = 1.2f;

    [Header("Text Animation")]
    [SerializeField] private float textAnimationDuration = 0.2f;

    private float currentProgress;
    private Coroutine loadingMessageCoroutine;

    private int lastDisplayedPercentage = -1;

    private string[] loadingMessages =
    {
        "Initializing Audio Systems",
        "Preparing Game Services",
        "Connecting Gameplay Modules",
        "Loading Player Profile",
        "Synchronizing Save Data",
        "Preparing User Interface",
        "Configuring Input Controls",
        "Optimizing Performance",
        "Initializing Physics Engine",
        "Preparing AI Systems",
        "Loading Game Assets",
        "Building Environment",
        "Preparing Visual Effects",
        "Loading Character Data",
        "Initializing Enemy Behaviors",
        "Preparing Mission Data",
        "Loading World State",
        "Generating Gameplay Systems",
        "Initializing Network Components",
        "Preparing Adventure",
        "Sharpening Blades",
        "Scanning Environment",
        "Charging Gameplay Systems"
    };

    private void Start()
    {
        SetupUI();

        PlayLogoIntro();

        loadingMessageCoroutine = StartCoroutine(ChangeLoadingMessages());

        StartCoroutine(InitializeGame());
    }

    private void SetupUI()
    {
        currentProgress = 0f;

        loadingSlider.value = 0f;

        if (logo != null)
        {
            logo.localScale = Vector3.one * logoStartScale;
        }

        loadingText.text = "Preparing Adventure...";
        percentageText.text = "0%";
    }

    // =========================================================
    // LOGO ANIMATION
    // =========================================================

    private void PlayLogoIntro()
    {
        if (logo == null)
            return;

        logo.DOKill();

        Sequence sequence = DOTween.Sequence();

        // Initial pop-in
        sequence.Append(
            logo.DOScale(
                logoTargetScale,
                logoIntroDuration
            )
            .SetEase(Ease.OutBack)
        );

        // Small pause
        sequence.AppendInterval(0.15f);

        // Start breathing animation
        sequence.Append(
            logo.DOScale(
                logoBreathScale,
                logoBreathDuration
            )
            .SetEase(Ease.InOutSine)
        );

        sequence.Append(
            logo.DOScale(
                logoTargetScale,
                logoBreathDuration
            )
            .SetEase(Ease.InOutSine)
        );

        sequence.SetLoops(-1, LoopType.Restart);
    }

    // =========================================================
    // LOADING
    // =========================================================

    private IEnumerator InitializeGame()
    {
        float timer = 0f;

        while (timer < totalLoadingDuration)
        {
            timer += Time.deltaTime;

            currentProgress = Mathf.Clamp01(
                timer / totalLoadingDuration
            );

            UpdateUI();

            yield return null;
        }

        currentProgress = 1f;

        UpdateUI();

        if (loadingMessageCoroutine != null)
        {
            StopCoroutine(loadingMessageCoroutine);
            loadingMessageCoroutine = null;
        }

        AnimateLoadingText("Loading Complete");

        yield return new WaitForSeconds(0.5f);

        PlayCompletionAnimation();

        yield return new WaitForSeconds(0.4f);

        SceneManager.LoadScene(1);
    }

    // =========================================================
    // PROGRESS UI
    // =========================================================

    private void UpdateUI()
    {
        // Smooth slider movement.
        loadingSlider.value = Mathf.Lerp(
            loadingSlider.value,
            currentProgress,
            Time.deltaTime * 8f
        );

        int percent = Mathf.RoundToInt(
            currentProgress * 100f
        );

        percentageText.text = percent + "%";

        // Only animate percentage when the number changes.
        if (percent != lastDisplayedPercentage)
        {
            lastDisplayedPercentage = percent;

            AnimatePercentage();
        }
    }

    private void AnimatePercentage()
    {
        percentageText.transform.DOKill();

        percentageText.transform.localScale = Vector3.one;

        percentageText.transform
            .DOPunchScale(
                Vector3.one * 0.08f,
                0.15f,
                3,
                0.5f
            );
    }

    // =========================================================
    // LOADING MESSAGES
    // =========================================================

    private IEnumerator ChangeLoadingMessages()
    {
        while (currentProgress < 1f)
        {
            string message =
                loadingMessages[
                    Random.Range(0, loadingMessages.Length)
                ];

            yield return StartCoroutine(
                AnimateDots(message)
            );

            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator AnimateDots(string baseMessage)
    {
        float duration = 1.2f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer % 0.9f;

            string dots;

            if (t < 0.3f)
                dots = ".";
            else if (t < 0.6f)
                dots = "..";
            else
                dots = "...";

            loadingText.text = baseMessage + dots;

            yield return null;
        }
    }

    // =========================================================
    // TEXT ANIMATION
    // =========================================================

    private void AnimateLoadingText(string message)
    {
        loadingText.transform.DOKill();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            loadingText.transform
                .DOScale(0.9f, textAnimationDuration)
                .SetEase(Ease.InQuad)
        );

        sequence.AppendCallback(() =>
        {
            loadingText.text = message;
        });

        sequence.Append(
            loadingText.transform
                .DOScale(1f, textAnimationDuration)
                .SetEase(Ease.OutBack)
        );
    }

    // =========================================================
    // COMPLETION
    // =========================================================

    private void PlayCompletionAnimation()
    {
        if (logo == null)
            return;

        logo.DOKill();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            logo.DOScale(
                1.08f,
                0.2f
            )
            .SetEase(Ease.OutQuad)
        );

        sequence.Append(
            logo.DOScale(
                1f,
                0.3f
            )
            .SetEase(Ease.OutBack)
        );
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        if (loadingMessageCoroutine != null)
        {
            StopCoroutine(loadingMessageCoroutine);
        }

        if (logo != null)
            logo.DOKill();

        if (loadingText != null)
            loadingText.transform.DOKill();

        if (percentageText != null)
            percentageText.transform.DOKill();

        DOTween.Kill(this);
    }
}