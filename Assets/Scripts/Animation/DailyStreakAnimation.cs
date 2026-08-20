using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DailyStreakAnimation : MonoBehaviour
{
    [Header("Streak Days")]
    [SerializeField] private Image[] dayImages;

    [Header("Sprites")]
    [SerializeField] private Sprite completedSprite;
    [SerializeField] private Sprite incompleteSprite;

    [Header("Animation")]
    [SerializeField] private float startDelay = 0.25f;
    [SerializeField] private float delayBetweenDays = 0.08f;
    [SerializeField] private float popDuration = 0.3f;

    [Header("Current Day")]
    [SerializeField] private bool animateCurrentDay = true;

    private void OnEnable()
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.OnDailyStreakChanged += HandleStreakChanged;
        }
    }

    private void Start()
    {
        if (PlayerDataManager.Instance == null)
            return;

        int streak = PlayerDataManager.Instance.DailyStreak;

        PlayAnimation(streak);
    }

    private void OnDisable()
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.OnDailyStreakChanged -= HandleStreakChanged;
        }
    }

    private void HandleStreakChanged(int streak)
    {
        Debug.Log("Daily streak changed: " + streak);

        PlayAnimation(streak);
    }

    public void PlayAnimation(int streak)
    {
        KillAnimations();

        streak = Mathf.Clamp(streak, 0, dayImages.Length);

        for (int i = 0; i < dayImages.Length; i++)
        {
            if (dayImages[i] == null)
                continue;

            int index = i;

            // --------------------------------
            // Set correct sprite
            // --------------------------------

            if (index < streak)
            {
                dayImages[index].sprite = completedSprite;
            }
            else
            {
                dayImages[index].sprite = incompleteSprite;
            }

            // --------------------------------
            // Initial scale
            // --------------------------------

            dayImages[index].transform.localScale = Vector3.zero;

            // --------------------------------
            // Pop animation
            // --------------------------------

            dayImages[index]
                .transform
                .DOScale(
                    Vector3.one,
                    popDuration
                )
                .SetDelay(
                    startDelay +
                    index * delayBetweenDays
                )
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    // Completed day animation
                    if (index < streak)
                    {
                        PlayCompletedAnimation(
                            dayImages[index].transform
                        );
                    }

                    // Current day pulse
                    if (
                        animateCurrentDay &&
                        index == streak - 1
                    )
                    {
                        StartCurrentDayPulse(
                            dayImages[index].transform
                        );
                    }
                });
        }
    }

    private void PlayCompletedAnimation(
        Transform day)
    {
        day.DOPunchScale(
            Vector3.one * 0.12f,
            0.25f,
            4,
            0.5f
        );
    }

    private void StartCurrentDayPulse(
        Transform day)
    {
        day
            .DOScale(
                1.06f,
                0.8f
            )
            .SetEase(Ease.InOutSine)
            .SetLoops(
                -1,
                LoopType.Yoyo
            );
    }

    private void KillAnimations()
    {
        if (dayImages == null)
            return;

        foreach (Image day in dayImages)
        {
            if (day != null)
            {
                day.transform.DOKill();
            }
        }
    }

    private void OnDestroy()
    {
        KillAnimations();
    }
}