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