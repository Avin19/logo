using UnityEngine;
using DG.Tweening;

public class ResultPanelAnimation : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private RectTransform winPanel;
    [SerializeField] private RectTransform lossPanel;

    [Header("Animation")]
    [SerializeField] private float slideDistance = 700f;
    [SerializeField] private float duration = 0.45f;

    private Vector2 winOriginalPosition;
    private Vector2 lossOriginalPosition;

    private void Awake()
    {
        if (winPanel != null)
            winOriginalPosition = winPanel.anchoredPosition;

        if (lossPanel != null)
            lossOriginalPosition = lossPanel.anchoredPosition;
    }

    // =========================================================
    // WIN
    // =========================================================

    public void ShowWin()
    {
        HidePanelsImmediately();

        winPanel.gameObject.SetActive(true);

        winPanel.anchoredPosition =
            winOriginalPosition +
            Vector2.down * slideDistance;

        winPanel
            .DOAnchorPos(
                winOriginalPosition,
                duration
            )
            .SetEase(Ease.OutBack);
    }

    // =========================================================
    // LOSS
    // =========================================================

    public void ShowLoss()
    {
        HidePanelsImmediately();

        lossPanel.gameObject.SetActive(true);

        lossPanel.anchoredPosition =
            lossOriginalPosition +
            Vector2.up * slideDistance;

        lossPanel
            .DOAnchorPos(
                lossOriginalPosition,
                duration
            )
            .SetEase(Ease.OutBack);
    }
    public void HideResultPanels(System.Action onComplete = null)
    {
        bool completed = false;

        void Complete()
        {
            if (completed)
                return;

            completed = true;
            onComplete?.Invoke();
        }

        if (winPanel != null && winPanel.gameObject.activeSelf)
        {
            winPanel.DOKill();

            winPanel
                .DOAnchorPos(
                    winOriginalPosition + Vector2.down * slideDistance,
                    0.3f
                )
                .SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    winPanel.gameObject.SetActive(false);
                    winPanel.anchoredPosition = winOriginalPosition;

                    Complete();
                });
        }
        else if (lossPanel != null && lossPanel.gameObject.activeSelf)
        {
            lossPanel.DOKill();

            lossPanel
                .DOAnchorPos(
                    lossOriginalPosition + Vector2.up * slideDistance,
                    0.3f
                )
                .SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    lossPanel.gameObject.SetActive(false);
                    lossPanel.anchoredPosition = lossOriginalPosition;

                    Complete();
                });
        }
        else
        {
            Complete();
        }
    }
    // =========================================================
    // HIDE
    // =========================================================

    public void HideResultPanels()
    {
        if (winPanel != null)
        {
            winPanel
                .DOAnchorPos(
                    winOriginalPosition +
                    Vector2.down * slideDistance,
                    0.3f
                )
                .SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    winPanel.gameObject.SetActive(false);
                    winPanel.anchoredPosition =
                        winOriginalPosition;
                });
        }

        if (lossPanel != null)
        {
            lossPanel
                .DOAnchorPos(
                    lossOriginalPosition +
                    Vector2.up * slideDistance,
                    0.3f
                )
                .SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    lossPanel.gameObject.SetActive(false);
                    lossPanel.anchoredPosition =
                        lossOriginalPosition;
                });
        }
    }

    // =========================================================
    // IMMEDIATE RESET
    // =========================================================

    private void HidePanelsImmediately()
    {
        if (winPanel != null)
        {
            winPanel.DOKill();

            winPanel.gameObject.SetActive(false);

            winPanel.anchoredPosition =
                winOriginalPosition;
        }

        if (lossPanel != null)
        {
            lossPanel.DOKill();

            lossPanel.gameObject.SetActive(false);

            lossPanel.anchoredPosition =
                lossOriginalPosition;
        }
    }

    private void OnDestroy()
    {
        winPanel?.DOKill();
        lossPanel?.DOKill();
    }
}