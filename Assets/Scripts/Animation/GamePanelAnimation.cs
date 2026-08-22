using System;
using UnityEngine;
using DG.Tweening;

public class GamePanelAnimation : MonoBehaviour
{
    [Header("Game Panel")]
    [SerializeField] private RectTransform gamePanel;

    [Header("Top Info")]
    [SerializeField] private RectTransform infoPanel;

    [Header("Question")]
    [SerializeField] private RectTransform questionImage;
    [SerializeField] private RectTransform questionText;

    [Header("Answer")]
    [SerializeField] private RectTransform userAnswer;
    [SerializeField] private RectTransform randomAnswer;

    [Header("Bottom Buttons")]
    [SerializeField] private RectTransform revealButton;
    [SerializeField] private RectTransform removeLetterButton;
    [SerializeField] private RectTransform skipButton;
    [SerializeField] private RectTransform clearButton;

    [Header("Animation")]
    [SerializeField] private float infoDuration = 0.35f;
    [SerializeField] private float imageDuration = 0.45f;
    [SerializeField] private float textDuration = 0.3f;
    [SerializeField] private float answerDuration = 0.3f;
    [SerializeField] private float buttonDuration = 0.3f;

    [Header("Slide Distance")]
    [SerializeField] private float infoSlideDistance = 150f;
    [SerializeField] private float buttonSlideDistance = 180f;

    [Header("Button Delay")]
    [SerializeField] private float buttonStartDelay = 0.5f;
    [SerializeField] private float buttonDelay = 0.06f;


    private Vector2 infoOriginalPosition;

    private Vector2 revealOriginalPosition;
    private Vector2 removeOriginalPosition;
    private Vector2 skipOriginalPosition;
    private Vector2 clearOriginalPosition;

    private void Awake()
    {
        if (gamePanel == null)
            gamePanel = GetComponent<RectTransform>();

        StoreOriginalPositions();
    }

    private void StoreOriginalPositions()
    {
        if (infoPanel != null)
            infoOriginalPosition =
                infoPanel.anchoredPosition;

        if (revealButton != null)
            revealOriginalPosition =
                revealButton.anchoredPosition;

        if (removeLetterButton != null)
            removeOriginalPosition =
                removeLetterButton.anchoredPosition;

        if (skipButton != null)
            skipOriginalPosition =
                skipButton.anchoredPosition;

        if (clearButton != null)
            clearOriginalPosition =
                clearButton.anchoredPosition;
    }

    // =========================================================
    // OPEN GAME PANEL
    // =========================================================

    public void PlayOpenAnimation()
    {
        KillAnimations();

        StoreOriginalPositions();

        SetInitialState();

        // -----------------------------------------------------
        // INFO PANEL
        // -----------------------------------------------------

        if (infoPanel != null)
        {
            infoPanel
                .DOAnchorPos(
                    infoOriginalPosition,
                    infoDuration
                )
                .SetEase(Ease.OutCubic);
        }

        // -----------------------------------------------------
        // QUESTION IMAGE
        // -----------------------------------------------------

        if (questionImage != null)
        {
            questionImage
                .DOScale(
                    Vector3.one,
                    imageDuration
                )
                .SetDelay(0.08f)
                .SetEase(Ease.OutBack);
        }

        // -----------------------------------------------------
        // QUESTION TEXT
        // -----------------------------------------------------

        if (questionText != null)
        {
            questionText
                .DOScale(
                    Vector3.one,
                    textDuration
                )
                .SetDelay(0.20f)
                .SetEase(Ease.OutBack);
        }

        // -----------------------------------------------------
        // USER ANSWER
        // -----------------------------------------------------

        if (userAnswer != null)
        {
            userAnswer
                .DOScale(
                    Vector3.one,
                    answerDuration
                )
                .SetDelay(0.30f)
                .SetEase(Ease.OutBack);
        }

        // -----------------------------------------------------
        // RANDOM ANSWER
        // -----------------------------------------------------

        if (randomAnswer != null)
        {
            randomAnswer
                .DOScale(
                    Vector3.one,
                    answerDuration
                )
                .SetDelay(0.38f)
                .SetEase(Ease.OutBack);
        }

        // -----------------------------------------------------
        // BOTTOM BUTTONS
        // -----------------------------------------------------

        AnimateBottomButton(
            revealButton,
            revealOriginalPosition,
            buttonStartDelay
        );

        AnimateBottomButton(
            removeLetterButton,
            removeOriginalPosition,
            buttonStartDelay + buttonDelay
        );

        AnimateBottomButton(
            skipButton,
            skipOriginalPosition,
            buttonStartDelay + buttonDelay * 2
        );

        AnimateBottomButton(
            clearButton,
            clearOriginalPosition,
            buttonStartDelay + buttonDelay * 3
        );
    }

    // =========================================================
    // BUTTON ENTRANCE
    // =========================================================

    private void AnimateBottomButton(
        RectTransform button,
        Vector2 originalPosition,
        float delay)
    {
        if (button == null)
            return;

        button.anchoredPosition =
            originalPosition +
            Vector2.down * buttonSlideDistance;

        button.localScale =
            Vector3.one * 0.9f;

        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(delay);

        sequence.Append(
            button
                .DOAnchorPos(
                    originalPosition,
                    buttonDuration
                )
                .SetEase(Ease.OutCubic)
        );

        sequence.Join(
            button
                .DOScale(
                    Vector3.one,
                    buttonDuration
                )
                .SetEase(Ease.OutBack)
        );
    }

    // =========================================================
    // INITIAL STATE
    // =========================================================

    private void SetInitialState()
    {
        // Info panel starts above

        if (infoPanel != null)
        {
            infoPanel.anchoredPosition =
                infoOriginalPosition +
                Vector2.up * infoSlideDistance;
        }

        // Question

        if (questionImage != null)
            questionImage.localScale = Vector3.zero;

        if (questionText != null)
            questionText.localScale = Vector3.zero;

        // Answers

        if (userAnswer != null)
            userAnswer.localScale = Vector3.zero;

        if (randomAnswer != null)
            randomAnswer.localScale = Vector3.zero;

        // Buttons

        SetButtonInitialPosition(
            revealButton,
            revealOriginalPosition
        );

        SetButtonInitialPosition(
            removeLetterButton,
            removeOriginalPosition
        );

        SetButtonInitialPosition(
            skipButton,
            skipOriginalPosition
        );

        SetButtonInitialPosition(
            clearButton,
            clearOriginalPosition
        );
    }

    private void SetButtonInitialPosition(
        RectTransform button,
        Vector2 originalPosition)
    {
        if (button == null)
            return;

        button.anchoredPosition =
            originalPosition +
            Vector2.down * buttonSlideDistance;

        button.localScale =
            Vector3.one * 0.9f;
    }

    // =========================================================
    // NEW QUESTION
    // =========================================================

    public void AnimateNewQuestion()
    {
        if (questionImage != null)
        {
            questionImage.DOKill();

            questionImage.localScale =
                Vector3.one * 0.85f;

            questionImage
                .DOScale(
                    Vector3.one,
                    0.35f
                )
                .SetEase(Ease.OutBack);
        }

        if (questionText != null)
        {
            questionText.DOKill();

            questionText.localScale =
                Vector3.zero;

            questionText
                .DOScale(
                    Vector3.one,
                    0.25f
                )
                .SetDelay(0.05f)
                .SetEase(Ease.OutBack);
        }

        if (userAnswer != null)
        {
            userAnswer.DOKill();

            userAnswer.localScale =
                Vector3.zero;

            userAnswer
                .DOScale(
                    Vector3.one,
                    0.25f
                )
                .SetDelay(0.12f)
                .SetEase(Ease.OutBack);
        }

        if (randomAnswer != null)
        {
            randomAnswer.DOKill();

            randomAnswer.localScale =
                Vector3.zero;

            randomAnswer
                .DOScale(
                    Vector3.one,
                    0.25f
                )
                .SetDelay(0.18f)
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

        if (revealButton != null)
            SlideButtonOut(revealButton);

        if (removeLetterButton != null)
            SlideButtonOut(removeLetterButton);

        if (skipButton != null)
            SlideButtonOut(skipButton);

        if (clearButton != null)
            SlideButtonOut(clearButton);

        if (questionImage != null)
        {
            questionImage
                .DOScale(
                    Vector3.zero,
                    0.2f
                )
                .SetEase(Ease.InBack);
        }

        if (questionText != null)
        {
            questionText
                .DOScale(
                    Vector3.zero,
                    0.2f
                )
                .SetEase(Ease.InBack);
        }

        if (userAnswer != null)
        {
            userAnswer
                .DOScale(
                    Vector3.zero,
                    0.2f
                )
                .SetEase(Ease.InBack);
        }

        if (randomAnswer != null)
        {
            randomAnswer
                .DOScale(
                    Vector3.zero,
                    0.2f
                )
                .SetEase(Ease.InBack);
        }

        if (infoPanel != null)
        {
            infoPanel
                .DOAnchorPos(
                    infoOriginalPosition +
                    Vector2.up * infoSlideDistance,
                    0.3f
                )
                .SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                });
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private void SlideButtonOut(
        RectTransform button)
    {
        button
            .DOAnchorPos(
                button.anchoredPosition +
                Vector2.down * buttonSlideDistance,
                0.25f
            )
            .SetEase(Ease.InCubic);
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void KillAnimations()
    {
        infoPanel?.DOKill();
        questionImage?.DOKill();
        questionText?.DOKill();
        userAnswer?.DOKill();
        randomAnswer?.DOKill();

        revealButton?.DOKill();
        removeLetterButton?.DOKill();
        skipButton?.DOKill();
        clearButton?.DOKill();
    }

    private void OnDestroy()
    {
        KillAnimations();
    }
}