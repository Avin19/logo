using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ProfileCardAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform profileCard;
    [SerializeField] private RectTransform avatar;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Card Animation")]
    [SerializeField] private float slideDistance = 350f;
    [SerializeField] private float duration = 0.55f;
    [SerializeField] private float delay = 0.05f;

    [Header("Avatar Animation")]
    [SerializeField] private float avatarPunch = 0.12f;

    private Vector2 originalPosition;

    private void Awake()
    {
        originalPosition = profileCard.anchoredPosition;
    }

    private void Start()
    {
        PlayAnimation();
    }

    public void PlayAnimation()
    {
        // Kill previous tweens
        profileCard.DOKill();

        if (avatar != null)
            avatar.DOKill();

        if (canvasGroup != null)
            canvasGroup.DOKill();

        // --------------------------------
        // Initial state
        // --------------------------------

        profileCard.anchoredPosition =
            originalPosition + Vector2.left * slideDistance;

        profileCard.localScale = Vector3.one * 0.92f;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        // --------------------------------
        // Card entrance
        // --------------------------------

        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(delay);

        // Fade
        if (canvasGroup != null)
        {
            sequence.Join(
                canvasGroup
                    .DOFade(1f, 0.3f)
                    .SetEase(Ease.OutQuad)
            );
        }

        // Slide
        sequence.Join(
            profileCard
                .DOAnchorPos(originalPosition, duration)
                .SetEase(Ease.OutCubic)
        );

        // Scale
        sequence.Join(
            profileCard
                .DOScale(1f, duration)
                .SetEase(Ease.OutBack)
        );

        // --------------------------------
        // Avatar punch
        // --------------------------------

        if (avatar != null)
        {
            sequence.AppendInterval(0.05f);

            sequence.Append(
                avatar
                    .DOPunchScale(
                        Vector3.one * avatarPunch,
                        0.3f,
                        5,
                        0.5f
                    )
            );
        }
    }

    private void OnDestroy()
    {
        profileCard?.DOKill();
        avatar?.DOKill();
        canvasGroup?.DOKill();
    }
}