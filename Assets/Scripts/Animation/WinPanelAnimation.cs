using UnityEngine;
using DG.Tweening;

public class WinStarAnimation : MonoBehaviour
{
    [Header("Stars")]
    [SerializeField] private RectTransform leftStar;
    [SerializeField] private RectTransform centerStar;
    [SerializeField] private RectTransform rightStar;

    [Header("Spin")]
    [SerializeField] private float spinDuration = 0.7f;
    [SerializeField] private float spinAmount = 360f;

    [Header("Bounce")]
    [SerializeField] private float bounceDuration = 0.35f;
    [SerializeField] private float bounceHeight = 35f;

    [Header("Pop")]
    [SerializeField] private float popDuration = 0.3f;
    [SerializeField] private float popScale = 1.25f;

    [Header("Timing")]
    [SerializeField] private float starDelay = 0.08f;

    private Vector2 leftOriginalPos;
    private Vector2 centerOriginalPos;
    private Vector2 rightOriginalPos;

    private Vector3 leftOriginalScale;
    private Vector3 centerOriginalScale;
    private Vector3 rightOriginalScale;


    private void Awake()
    {
        SaveOriginalValues();
    }


    private void OnEnable()
    {
        PlayAnimation();
    }


    private void SaveOriginalValues()
    {
        if (leftStar != null)
        {
            leftOriginalPos = leftStar.anchoredPosition;
            leftOriginalScale = leftStar.localScale;
        }

        if (centerStar != null)
        {
            centerOriginalPos = centerStar.anchoredPosition;
            centerOriginalScale = centerStar.localScale;
        }

        if (rightStar != null)
        {
            rightOriginalPos = rightStar.anchoredPosition;
            rightOriginalScale = rightStar.localScale;
        }
    }


    // =========================================================
    // MAIN ANIMATION
    // =========================================================

    public void PlayAnimation()
    {
        KillTweens();

        ResetStars();

        Sequence sequence = DOTween.Sequence();


        // =====================================================
        // 1. ALL THREE STARS SPIN
        // =====================================================

        if (leftStar != null)
        {
            sequence.Join(
                leftStar
                    .DORotate(
                        new Vector3(0f, 0f, spinAmount),
                        spinDuration,
                        RotateMode.FastBeyond360
                    )
                    .SetEase(Ease.OutQuad)
            );
        }


        if (centerStar != null)
        {
            sequence.Join(
                centerStar
                    .DORotate(
                        new Vector3(0f, 0f, spinAmount),
                        spinDuration,
                        RotateMode.FastBeyond360
                    )
                    .SetEase(Ease.OutQuad)
            );
        }


        if (rightStar != null)
        {
            sequence.Join(
                rightStar
                    .DORotate(
                        new Vector3(0f, 0f, -spinAmount),
                        spinDuration,
                        RotateMode.FastBeyond360
                    )
                    .SetEase(Ease.OutQuad)
            );
        }


        // =====================================================
        // 2. BOUNCE
        // =====================================================

        sequence.AppendCallback(
            BounceStars
        );


        // =====================================================
        // 3. POP
        // =====================================================

        sequence.AppendInterval(0.05f);

        if (centerStar != null)
        {
            sequence.Append(
                centerStar
                    .DOScale(
                        centerOriginalScale * popScale,
                        popDuration
                    )
                    .SetEase(Ease.OutBack)
            );

            sequence.Append(
                centerStar
                    .DOScale(
                        centerOriginalScale,
                        0.18f
                    )
                    .SetEase(Ease.InOutQuad)
            );
        }


        // Left and right pop slightly after center

        if (leftStar != null)
        {
            sequence.Insert(
                spinDuration + 0.05f,
                leftStar
                    .DOScale(
                        leftOriginalScale * 1.18f,
                        popDuration
                    )
                    .SetEase(Ease.OutBack)
            );

            sequence.Insert(
                spinDuration + 0.35f,
                leftStar
                    .DOScale(
                        leftOriginalScale,
                        0.18f
                    )
                    .SetEase(Ease.InOutQuad)
            );
        }


        if (rightStar != null)
        {
            sequence.Insert(
                spinDuration + 0.1f,
                rightStar
                    .DOScale(
                        rightOriginalScale * 1.18f,
                        popDuration
                    )
                    .SetEase(Ease.OutBack)
            );

            sequence.Insert(
                spinDuration + 0.4f,
                rightStar
                    .DOScale(
                        rightOriginalScale,
                        0.18f
                    )
                    .SetEase(Ease.InOutQuad)
            );
        }
    }


    // =========================================================
    // BOUNCE
    // =========================================================

    private void BounceStars()
    {
        if (leftStar != null)
        {
            leftStar
                .DOAnchorPos(
                    leftOriginalPos +
                    Vector2.up * bounceHeight,
                    bounceDuration
                )
                .SetEase(Ease.OutQuad)
                .SetLoops(
                    2,
                    LoopType.Yoyo
                );
        }


        if (centerStar != null)
        {
            centerStar
                .DOAnchorPos(
                    centerOriginalPos +
                    Vector2.up * (bounceHeight * 1.25f),
                    bounceDuration
                )
                .SetEase(Ease.OutQuad)
                .SetLoops(
                    2,
                    LoopType.Yoyo
                );
        }


        if (rightStar != null)
        {
            rightStar
                .DOAnchorPos(
                    rightOriginalPos +
                    Vector2.up * bounceHeight,
                    bounceDuration
                )
                .SetEase(Ease.OutQuad)
                .SetLoops(
                    2,
                    LoopType.Yoyo
                );
        }
    }


    // =========================================================
    // RESET
    // =========================================================

    private void ResetStars()
    {
        if (leftStar != null)
        {
            leftStar.anchoredPosition =
                leftOriginalPos;

            leftStar.localScale =
                leftOriginalScale;

            leftStar.localRotation =
                Quaternion.identity;
        }


        if (centerStar != null)
        {
            centerStar.anchoredPosition =
                centerOriginalPos;

            centerStar.localScale =
                centerOriginalScale;

            centerStar.localRotation =
                Quaternion.identity;
        }


        if (rightStar != null)
        {
            rightStar.anchoredPosition =
                rightOriginalPos;

            rightStar.localScale =
                rightOriginalScale;

            rightStar.localRotation =
                Quaternion.identity;
        }
    }


    // =========================================================
    // CLEANUP
    // =========================================================

    private void KillTweens()
    {
        if (leftStar != null)
            leftStar.DOKill();

        if (centerStar != null)
            centerStar.DOKill();

        if (rightStar != null)
            rightStar.DOKill();
    }


    private void OnDisable()
    {
        KillTweens();
    }


    private void OnDestroy()
    {
        KillTweens();
    }
}