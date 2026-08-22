using UnityEngine;
using DG.Tweening;

public class AchievementPanelAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation")]
    [SerializeField] private float slideDistance = 800f;
    [SerializeField] private float duration = 0.5f;

    private Vector2 originalPosition;

    private void Awake()
    {
        if (panel == null)
            panel = GetComponent<RectTransform>();

        originalPosition = panel.anchoredPosition;
    }

    // =====================================================
    // OPEN
    // =====================================================

    public void PlayOpenAnimation()
    {
        panel.DOKill();

        if (canvasGroup != null)
            canvasGroup.DOKill();

        // Start below screen
        panel.anchoredPosition =
            originalPosition +
            Vector2.down * slideDistance;

        panel.localScale =
            Vector3.one * 0.9f;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        Sequence sequence = DOTween.Sequence();

        // Fade
        if (canvasGroup != null)
        {
            sequence.Join(
                canvasGroup
                    .DOFade(1f, 0.25f)
                    .SetEase(Ease.OutQuad)
            );
        }

        // Slide up
        sequence.Join(
            panel
                .DOAnchorPos(
                    originalPosition,
                    duration
                )
                .SetEase(Ease.OutCubic)
        );

        // Scale
        sequence.Join(
            panel
                .DOScale(
                    Vector3.one,
                    duration
                )
                .SetEase(Ease.OutBack)
        );
    }

    // =====================================================
    // CLOSE
    // =====================================================

    public void PlayCloseAnimation(
        System.Action onComplete = null)
    {
        panel.DOKill();

        if (canvasGroup != null)
            canvasGroup.DOKill();

        Sequence sequence = DOTween.Sequence();

        // Slide down
        sequence.Join(
            panel
                .DOAnchorPos(
                    originalPosition +
                    Vector2.down * slideDistance,
                    0.35f
                )
                .SetEase(Ease.InCubic)
        );

        // Fade
        if (canvasGroup != null)
        {
            sequence.Join(
                canvasGroup
                    .DOFade(
                        0f,
                        0.25f
                    )
                    .SetEase(Ease.InQuad)
            );
        }

        sequence.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    private void OnDisable()
    {
        panel?.DOKill();
        canvasGroup?.DOKill();
    }

    private void OnDestroy()
    {
        panel?.DOKill();
        canvasGroup?.DOKill();
    }
}