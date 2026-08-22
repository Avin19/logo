using UnityEngine;
using DG.Tweening;

public class LossPanelAnimation : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private RectTransform panel;

    [Header("UI Elements")]
    [SerializeField] private RectTransform title;
    [SerializeField] private RectTransform sadBulb;
    [SerializeField] private RectTransform correctLogo;
    [SerializeField] private RectTransform homeButton;
    [SerializeField] private RectTransform tryAgainButton;
    [SerializeField] private RectTransform watchAdButton;

    [Header("Panel Animation")]
    [SerializeField] private float panelDuration = 0.4f;

    [Header("Title")]
    [SerializeField] private float titleDuration = 0.3f;

    [Header("Bulb")]
    [SerializeField] private float bulbDuration = 0.35f;
    [SerializeField] private float bulbPunchScale = 0.12f;

    [Header("Correct Logo")]
    [SerializeField] private float logoDuration = 0.3f;

    [Header("Buttons")]
    [SerializeField] private float buttonDuration = 0.3f;
    [SerializeField] private float buttonDelay = 0.08f;

    private Vector3 panelScale;
    private Vector3 titleScale;
    private Vector3 bulbScale;
    private Vector3 logoScale;
    private Vector3 homeScale;
    private Vector3 tryAgainScale;
    private Vector3 watchAdScale;

    private Vector2 homePosition;
    private Vector2 tryAgainPosition;
    private Vector2 watchAdPosition;


    private void Awake()
    {
        SaveOriginalValues();
    }


    private void OnEnable()
    {
        PlayAnimation();
    }


    // =========================================================
    // SAVE ORIGINAL VALUES
    // =========================================================

    private void SaveOriginalValues()
    {
        if (panel != null)
            panelScale = panel.localScale;

        if (title != null)
            titleScale = title.localScale;

        if (sadBulb != null)
            bulbScale = sadBulb.localScale;

        if (correctLogo != null)
            logoScale = correctLogo.localScale;

        if (homeButton != null)
        {
            homeScale = homeButton.localScale;
            homePosition = homeButton.anchoredPosition;
        }

        if (tryAgainButton != null)
        {
            tryAgainScale = tryAgainButton.localScale;
            tryAgainPosition =
                tryAgainButton.anchoredPosition;
        }

        if (watchAdButton != null)
        {
            watchAdScale = watchAdButton.localScale;
            watchAdPosition =
                watchAdButton.anchoredPosition;
        }
    }


    // =========================================================
    // PLAY
    // =========================================================

    public void PlayAnimation()
    {
        KillTweens();

        ResetElements();

        Sequence sequence = DOTween.Sequence();


        // =====================================================
        // 1. PANEL POP
        // =====================================================

        if (panel != null)
        {
            sequence.Append(
                panel
                    .DOScale(
                        panelScale,
                        panelDuration
                    )
                    .SetEase(Ease.OutBack)
            );
        }


        // =====================================================
        // 2. TITLE POP
        // =====================================================

        if (title != null)
        {
            sequence.Append(
                title
                    .DOScale(
                        titleScale,
                        titleDuration
                    )
                    .SetEase(Ease.OutBack)
            );
        }


        // =====================================================
        // 3. SAD BULB POP
        // =====================================================

        if (sadBulb != null)
        {
            sequence.Append(
                sadBulb
                    .DOScale(
                        bulbScale,
                        bulbDuration
                    )
                    .SetEase(Ease.OutBack)
            );

            sequence.Join(
                sadBulb
                    .DORotate(
                        new Vector3(0f, 0f, -10f),
                        0.12f
                    )
                    .SetLoops(
                        2,
                        LoopType.Yoyo
                    )
            );
        }


        // =====================================================
        // 4. BULB PUNCH
        // =====================================================

        if (sadBulb != null)
        {
            sequence.Append(
                sadBulb
                    .DOPunchScale(
                        Vector3.one * bulbPunchScale,
                        0.4f,
                        5,
                        0.5f
                    )
            );
        }


        // =====================================================
        // 5. CORRECT LOGO POP
        // =====================================================

        if (correctLogo != null)
        {
            sequence.Append(
                correctLogo
                    .DOScale(
                        logoScale,
                        logoDuration
                    )
                    .SetEase(Ease.OutBack)
            );

            sequence.Join(
                correctLogo
                    .DORotate(
                        new Vector3(0f, 0f, 8f),
                        0.12f
                    )
                    .SetLoops(
                        2,
                        LoopType.Yoyo
                    )
            );
        }


        // =====================================================
        // 6. HOME BUTTON
        // =====================================================

        if (homeButton != null)
        {
            sequence.AppendInterval(buttonDelay);

            sequence.Append(
                homeButton
                    .DOScale(
                        homeScale,
                        buttonDuration
                    )
                    .SetEase(Ease.OutBack)
            );
        }


        // =====================================================
        // 7. TRY AGAIN BUTTON
        // =====================================================

        if (tryAgainButton != null)
        {
            sequence.Append(
                tryAgainButton
                    .DOScale(
                        tryAgainScale,
                        buttonDuration
                    )
                    .SetEase(Ease.OutBack)
            );
        }


        // =====================================================
        // 8. WATCH AD BUTTON
        // =====================================================

        if (watchAdButton != null)
        {
            sequence.AppendInterval(buttonDelay);

            sequence.Append(
                watchAdButton
                    .DOScale(
                        watchAdScale,
                        buttonDuration
                    )
                    .SetEase(Ease.OutBack)
            );
        }


        // =====================================================
        // 9. BUTTON IDLE ANIMATION
        // =====================================================

        sequence.AppendCallback(
            StartButtonIdleAnimation
        );
    }


    // =========================================================
    // RESET
    // =========================================================

    private void ResetElements()
    {
        if (panel != null)
            panel.localScale = Vector3.zero;

        if (title != null)
            title.localScale = Vector3.zero;

        if (sadBulb != null)
        {
            sadBulb.localScale = Vector3.zero;
            sadBulb.localRotation =
                Quaternion.identity;
        }

        if (correctLogo != null)
        {
            correctLogo.localScale = Vector3.zero;
            correctLogo.localRotation =
                Quaternion.identity;
        }

        if (homeButton != null)
        {
            homeButton.localScale = Vector3.zero;
            homeButton.anchoredPosition =
                homePosition;
        }

        if (tryAgainButton != null)
        {
            tryAgainButton.localScale = Vector3.zero;
            tryAgainButton.anchoredPosition =
                tryAgainPosition;
        }

        if (watchAdButton != null)
        {
            watchAdButton.localScale = Vector3.zero;
            watchAdButton.anchoredPosition =
                watchAdPosition;
        }
    }


    // =========================================================
    // BUTTON IDLE
    // =========================================================

    private void StartButtonIdleAnimation()
    {
        if (tryAgainButton != null)
        {
            tryAgainButton
                .DOScale(
                    tryAgainScale * 1.04f,
                    0.8f
                )
                .SetEase(Ease.InOutSine)
                .SetLoops(
                    -1,
                    LoopType.Yoyo
                );
        }

        if (watchAdButton != null)
        {
            watchAdButton
                .DOScale(
                    watchAdScale * 1.03f,
                    0.9f
                )
                .SetEase(Ease.InOutSine)
                .SetLoops(
                    -1,
                    LoopType.Yoyo
                );
        }
    }


    // =========================================================
    // CLEANUP
    // =========================================================

    private void KillTweens()
    {
        if (panel != null)
            panel.DOKill();

        if (title != null)
            title.DOKill();

        if (sadBulb != null)
            sadBulb.DOKill();

        if (correctLogo != null)
            correctLogo.DOKill();

        if (homeButton != null)
            homeButton.DOKill();

        if (tryAgainButton != null)
            tryAgainButton.DOKill();

        if (watchAdButton != null)
            watchAdButton.DOKill();
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