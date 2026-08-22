using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButtonAnimation : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private RectTransform button;

    [Header("Entrance")]
    [SerializeField] private float delay = 0f;
    [SerializeField] private float entranceDuration = 0.4f;
    [SerializeField] private float startScale = 0.75f;

    [Header("Press")]
    [SerializeField] private float pressedScale = 0.92f;
    [SerializeField] private float pressDuration = 0.08f;

    [Header("Idle")]
    [SerializeField] private bool enableIdleAnimation = false;
    [SerializeField] private float idleScale = 1.025f;
    [SerializeField] private float idleDuration = 1.5f;

    private Vector3 originalScale;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<RectTransform>();

        originalScale = button.localScale;
    }

    private void Start()
    {
        PlayEntrance();
    }

    // =========================================================
    // ENTRANCE
    // =========================================================

    public void PlayEntrance()
    {
        button.DOKill();

        button.localScale =
            originalScale * startScale;

        button
            .DOScale(
                originalScale,
                entranceDuration
            )
            .SetDelay(delay)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                if (enableIdleAnimation)
                    StartIdleAnimation();
            });
    }

    // =========================================================
    // IDLE
    // =========================================================

    private void StartIdleAnimation()
    {
        button.DOKill();

        button
            .DOScale(
                originalScale * idleScale,
                idleDuration
            )
            .SetEase(Ease.InOutSine)
            .SetLoops(
                -1,
                LoopType.Yoyo
            );
    }

    // =========================================================
    // BUTTON PRESS
    // =========================================================

    public void OnPointerDown(PointerEventData eventData)
    {
        button.DOKill();

        button
            .DOScale(
                originalScale * pressedScale,
                pressDuration
            )
            .SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        button.DOKill();

        button
            .DOScale(
                originalScale,
                0.15f
            )
            .SetEase(Ease.OutBack);
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        button?.DOKill();
    }
}