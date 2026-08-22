using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CategoryButtonAnimation : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Entrance")]
    [SerializeField] private float startScale = 0.75f;
    [SerializeField] private float entranceDuration = 0.4f;

    [Header("Press")]
    [SerializeField] private float pressedScale = 0.92f;
    [SerializeField] private float pressDuration = 0.08f;

    private RectTransform rectTransform;
    private Vector3 originalScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void PlayEntrance(float delay)
    {
        rectTransform.DOKill();

        rectTransform.localScale =
            originalScale * startScale;

        rectTransform
            .DOScale(
                originalScale,
                entranceDuration
            )
            .SetDelay(delay)
            .SetEase(Ease.OutBack);
    }
    public void PlayExit(float delay = 0f)
    {
        rectTransform.DOKill();

        Vector2 startPosition = rectTransform.anchoredPosition;

        rectTransform
            .DOAnchorPos(
                startPosition + Vector2.down * 250f,
                0.3f
            )
            .SetDelay(delay)
            .SetEase(Ease.InCubic);

        rectTransform
            .DOScale(
                originalScale * 0.85f,
                0.25f
            )
            .SetDelay(delay)
            .SetEase(Ease.InBack);
    }

    public void OnPointerDown(
        PointerEventData eventData)
    {
        rectTransform.DOKill();

        rectTransform
            .DOScale(
                originalScale * pressedScale,
                pressDuration
            )
            .SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(
        PointerEventData eventData)
    {
        rectTransform.DOKill();

        rectTransform
            .DOScale(
                originalScale,
                0.15f
            )
            .SetEase(Ease.OutBack);
    }

    private void OnDestroy()
    {
        rectTransform?.DOKill();
    }

}