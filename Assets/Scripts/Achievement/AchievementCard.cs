using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AchievementCard : MonoBehaviour
{
    [Header("UI References")]

    [SerializeField] private Image iconImage;

    [SerializeField] private TextMeshProUGUI titleText;

    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private TextMeshProUGUI progressText;

    [SerializeField] private TextMeshProUGUI rewardText;

    [SerializeField] private Slider progressBar;

    [Header("State Objects")]

    [SerializeField] private GameObject lockedOverlay;

    [SerializeField] private GameObject unlockedObject;

    [Header("Animation")]

    [SerializeField] private float progressAnimationDuration = 0.4f;

    [SerializeField] private float unlockPunchScale = 1.15f;

    public string AchievementId { get; private set; }

    private AchievementSO definition;


    // =========================================================
    // SETUP
    // =========================================================

    public void Setup(
     AchievementSO achievement,
     AchievementProgress progress)
    {
        definition = achievement;

        AchievementId = achievement.Id;

        if (iconImage != null)
            iconImage.sprite = achievement.Icon;

        if (titleText != null)
            titleText.text = achievement.Title;

        if (descriptionText != null)
            descriptionText.text = achievement.Description;

        if (rewardText != null)
            rewardText.text = "+" + achievement.Reward;

        UpdateProgressInstant(
            progress.Progress,
            achievement.Target
        );

        if (progress.Unlocked)
        {
            SetUnlocked(false);
        }
        else
        {
            SetLocked();
        }
    }

    // =========================================================
    // UPDATE PROGRESS
    // =========================================================

    public void UpdateProgress(
        int progress,
        int target)
    {
        progress =
            Mathf.Clamp(
                progress,
                0,
                target
            );

        if (progressText != null)
        {
            progressText.text =
                progress + " / " + target;
        }

        if (progressBar != null)
        {
            float targetValue =
                target > 0
                    ? (float)progress / target
                    : 0f;

            progressBar
                .DOValue(
                    targetValue,
                    progressAnimationDuration
                )
                .SetEase(Ease.OutCubic);
        }
    }


    // =========================================================
    // INSTANT PROGRESS
    // =========================================================

    private void UpdateProgressInstant(
        int progress,
        int target)
    {
        progress =
            Mathf.Clamp(
                progress,
                0,
                target
            );

        if (progressText != null)
        {
            progressText.text =
                progress + " / " + target;
        }

        if (progressBar != null)
        {
            progressBar.value =
                target > 0
                    ? (float)progress / target
                    : 0f;
        }
    }


    // =========================================================
    // LOCKED
    // =========================================================

    private void SetLocked()
    {
        if (lockedOverlay != null)
        {
            lockedOverlay.SetActive(true);
        }

        if (unlockedObject != null)
        {
            unlockedObject.SetActive(false);
        }
    }


    // =========================================================
    // UNLOCKED
    // =========================================================

    public void SetUnlocked(
        bool animate = true)
    {
        if (lockedOverlay != null)
        {
            lockedOverlay.SetActive(false);
        }

        if (unlockedObject != null)
        {
            unlockedObject.SetActive(true);
        }

        if (animate)
        {
            PlayUnlockAnimation();
        }
    }


    // =========================================================
    // UNLOCK ANIMATION
    // =========================================================

    private void PlayUnlockAnimation()
    {
        transform.DOKill();

        transform
            .DOPunchScale(
                Vector3.one *
                (unlockPunchScale - 1f),
                0.4f,
                5,
                0.5f
            );
    }


    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        transform.DOKill();

        if (progressBar != null)
        {
            progressBar.DOKill();
        }
    }
}