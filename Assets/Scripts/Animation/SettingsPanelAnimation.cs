using System;
using UnityEngine;
using DG.Tweening;

public class SettingsPanelAnimation : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private RectTransform panel;

    [Header("UI Elements")]
    [SerializeField] private RectTransform title;
    [SerializeField] private RectTransform closeButton;

    [SerializeField] private RectTransform[] settingButtons;

    [Header("Slide Animation")]
    [SerializeField] private float slideDistance = 800f;
    [SerializeField] private float slideDuration = 0.5f;

    [Header("Scale")]
    [SerializeField] private float startScale = 0.96f;
    [SerializeField] private float scaleDuration = 0.45f;

    [Header("Children Animation")]
    [SerializeField] private float childDuration = 0.25f;
    [SerializeField] private float childStartDelay = 0.05f;
    [SerializeField] private float childDelay = 0.08f;

    private Vector2 targetPosition;
    private Vector3 originalScale;

    private void Awake()
    {
        if (panel == null)
        {
            panel = GetComponent<RectTransform>();
        }

        targetPosition = panel.anchoredPosition;
        originalScale = panel.localScale;
    }

    // =========================================================
    // OPEN
    // =========================================================

    public void PlayOpenAnimation()
    {
        KillAnimations();

        // Store the final position.
        targetPosition = panel.anchoredPosition;

        // ---------------------------------------------
        // Initial state
        // ---------------------------------------------

        panel.anchoredPosition =
            targetPosition +
            Vector2.down * slideDistance;

        panel.localScale =
            originalScale * startScale;

        // Hide children
        SetChildrenInitialState();

        // ---------------------------------------------
        // Slide up
        // ---------------------------------------------

        panel
            .DOAnchorPos(
                targetPosition,
                slideDuration
            )
            .SetEase(Ease.OutCubic);

        // ---------------------------------------------
        // Scale settle
        // ---------------------------------------------

        panel
            .DOScale(
                originalScale,
                scaleDuration
            )
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                AnimateChildren();
            });
    }

    // =========================================================
    // CHILD INITIAL STATE
    // =========================================================

    private void SetChildrenInitialState()
    {
        if (title != null)
        {
            title.localScale = Vector3.zero;
        }

        if (closeButton != null)
        {
            closeButton.localScale = Vector3.zero;
        }

        if (settingButtons == null)
            return;

        foreach (RectTransform item in settingButtons)
        {
            if (item != null)
            {
                item.localScale = Vector3.zero;
            }
        }
    }

    // =========================================================
    // CHILD ANIMATION
    // =========================================================

    private void AnimateChildren()
    {
        // ---------------------------------------------
        // Title
        // ---------------------------------------------

        if (title != null)
        {
            title
                .DOScale(
                    Vector3.one,
                    childDuration
                )
                .SetDelay(childStartDelay)
                .SetEase(Ease.OutBack);
        }

        // ---------------------------------------------
        // Close button
        // ---------------------------------------------

        if (closeButton != null)
        {
            closeButton
                .DOScale(
                    Vector3.one,
                    childDuration
                )
                .SetDelay(childStartDelay + 0.05f)
                .SetEase(Ease.OutBack);
        }

        // ---------------------------------------------
        // Setting buttons
        // ---------------------------------------------

        if (settingButtons == null)
            return;

        for (int i = 0; i < settingButtons.Length; i++)
        {
            RectTransform item = settingButtons[i];

            if (item == null)
                continue;

            item
                .DOScale(
                    Vector3.one,
                    childDuration
                )
                .SetDelay(
                    childStartDelay +
                    0.1f +
                    i * childDelay
                )
                .SetEase(Ease.OutBack);
        }
    }

    // =========================================================
    // CLOSE
    // =========================================================

    public void PlayCloseAnimation(
        Action onComplete = null)
    {
        KillAnimations();

        // Make sure we know the correct final position.
        targetPosition = panel.anchoredPosition;

        // ---------------------------------------------
        // Slide down
        // ---------------------------------------------

        panel
            .DOAnchorPos(
                targetPosition +
                Vector2.down * slideDistance,
                slideDuration * 0.7f
            )
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                // Reset position so it is ready
                // for the next opening.
                panel.anchoredPosition =
                    targetPosition;

                panel.localScale =
                    originalScale;

                onComplete?.Invoke();
            });

        // ---------------------------------------------
        // Slight shrink
        // ---------------------------------------------

        panel
            .DOScale(
                originalScale * 0.96f,
                slideDuration * 0.7f
            )
            .SetEase(Ease.InBack);
    }

    // =========================================================
    // STOP ALL TWEENS
    // =========================================================

    private void KillAnimations()
    {
        if (panel != null)
            panel.DOKill();

        if (title != null)
            title.DOKill();

        if (closeButton != null)
            closeButton.DOKill();

        if (settingButtons == null)
            return;

        foreach (RectTransform item in settingButtons)
        {
            if (item != null)
                item.DOKill();
        }
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        KillAnimations();
    }
}