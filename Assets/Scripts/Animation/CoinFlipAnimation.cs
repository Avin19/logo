using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CoinFlipAnimation : MonoBehaviour
{
    [SerializeField] private Image coinImage;

    [Header("Sprites")]
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite edgeSprite;

    [Header("Animation")]
    [SerializeField] private float flipDuration = 0.7f;
    [SerializeField] private float delay = 2f;

    private Sequence sequence;

    private void OnEnable()
    {
        StartCoinAnimation();
    }

    private void StartCoinAnimation()
    {
        sequence?.Kill();

        coinImage.sprite = frontSprite;

        sequence = DOTween.Sequence();

        sequence.AppendInterval(delay);

        // Front → Edge
        sequence.Append(
            transform
                .DOScaleX(0.05f, flipDuration * 0.5f)
                .SetEase(Ease.InQuad)
        );

        sequence.AppendCallback(() =>
        {
            coinImage.sprite = edgeSprite;
        });

        // Edge → Front
        sequence.Append(
            transform
                .DOScaleX(1f, flipDuration * 0.5f)
                .SetEase(Ease.OutQuad)
        );

        sequence.AppendCallback(() =>
        {
            coinImage.sprite = frontSprite;
        });

        sequence.SetLoops(-1);
    }

    private void OnDisable()
    {
        sequence?.Kill();
    }
}