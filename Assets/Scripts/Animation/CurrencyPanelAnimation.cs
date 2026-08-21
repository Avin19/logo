using UnityEngine;
using DG.Tweening;

public class CurrencyPanelAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation")]
    [SerializeField] private float slideDistance = 300f;
    [SerializeField] private float duration = 0.55f;
    [SerializeField] private float delay = 0.15f;

    private Vector2 originalPosition;

    private void Awake()
    {
        if (panel == null)
            panel = GetComponent<RectTransform>();

        originalPosition = panel.anchoredPosition;
    }

    private void Start()
    {
        PlayAnimation();
    }

    public void PlayAnimation()
    {
        panel.DOKill();

        if (canvasGroup != null)
            canvasGroup.DOKill();

        // -----------------------------------------
        // Initial position
        // -----------------------------------------

        panel.anchoredPosition =
            originalPosition +
            Vector2.right * slideDistance;

        panel.localScale =
            Vector3.one * 0.95f;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        // -----------------------------------------
        // Animation
        // -----------------------------------------

        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(delay);

        // Fade
        if (canvasGroup != null)
        {
            sequence.Join(
                canvasGroup
                    .DOFade(1f, 0.25f)
                    .SetEase(Ease.OutQuad)
            );
        }

        // Right → Original position
        sequence.Join(
            panel
                .DOAnchorPos(
                    originalPosition,
                    duration
                )
                .SetEase(Ease.OutCubic)
        );

        // Small scale entrance
        sequence.Join(
            panel
                .DOScale(
                    Vector3.one,
                    duration
                )
                .SetEase(Ease.OutBack)
        );
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