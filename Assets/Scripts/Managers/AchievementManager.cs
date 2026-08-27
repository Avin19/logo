using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    // =========================================================
    // EVENTS
    // =========================================================

    public event Action<AchievementSO> OnAchievementUnlocked;

    public event Action<string, int, int>
        OnAchievementProgressChanged;


    // =========================================================
    // ACHIEVEMENTS
    // =========================================================

    [Header("Achievements")]
    [SerializeField]
    private List<AchievementSO> achievements =
        new List<AchievementSO>();


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            InitializePlayerAchievements();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // =========================================================
    // INITIALIZE PLAYER ACHIEVEMENTS
    // =========================================================

    private const int MaxInitRetryAttempts = 50;
    private int initRetryAttempts = 0;

    private void InitializePlayerAchievements()
    {
        if (PlayerDataManager.Instance == null ||
            PlayerDataManager.Instance.data == null)
        {
            initRetryAttempts++;

            if (initRetryAttempts > MaxInitRetryAttempts)
            {
                Debug.LogWarning(
                    "AchievementManager: Gave up waiting for PlayerDataManager after " +
                    MaxInitRetryAttempts + " attempts."
                );

                return;
            }

            Invoke(nameof(InitializePlayerAchievements), 0.1f);

            return;
        }

        if (PlayerDataManager.Instance.data.Achievements == null)
        {
            PlayerDataManager.Instance.data.Achievements =
                new List<AchievementProgress>();
        }


        foreach (AchievementSO achievement in achievements)
        {
            if (achievement == null)
                continue;

            AchievementProgress existing =
                PlayerDataManager.Instance.data.Achievements
                    .Find(
                        x => x.AchievementId ==
                             achievement.Id
                    );


            if (existing == null)
            {
                PlayerDataManager.Instance.data.Achievements.Add(
                    new AchievementProgress
                    {
                        AchievementId = achievement.Id,
                        Progress = 0,
                        Unlocked = false,
                        UnlockedDate = ""
                    }
                );
            }
        }

        PlayerDataManager.Instance.Save();
    }


    // =========================================================
    // GET ACHIEVEMENT
    // =========================================================

    public AchievementSO GetAchievement(string id)
    {
        return achievements.Find(
            x => x != null &&
                 x.Id == id
        );
    }


    // =========================================================
    // GET PLAYER PROGRESS
    // =========================================================

    public AchievementProgress GetProgress(string id)
    {
        if (PlayerDataManager.Instance == null)
            return null;

        if (PlayerDataManager.Instance.data == null)
            return null;

        if (PlayerDataManager.Instance.data.Achievements == null)
            return null;


        return PlayerDataManager.Instance.data.Achievements
            .Find(
                x => x.AchievementId == id
            );
    }


    // =========================================================
    // GET ALL ACHIEVEMENTS
    // =========================================================

    public List<AchievementSO> GetAllAchievements()
    {
        return achievements;
    }


    // =========================================================
    // ADD PROGRESS
    // =========================================================

    public void AddProgress(
        string achievementId,
        int amount = 1,
        bool saveImmediately = true)
    {
        AchievementSO achievement =
            GetAchievement(achievementId);

        AchievementProgress progress =
            GetProgress(achievementId);


        if (achievement == null)
        {
            Debug.LogWarning(
                "Achievement not found: " +
                achievementId
            );

            return;
        }


        if (progress == null)
            return;


        if (progress.Unlocked)
            return;


        progress.Progress += amount;


        progress.Progress =
            Mathf.Clamp(
                progress.Progress,
                0,
                achievement.Target
            );


        // Achievement completed
        if (progress.Progress >= achievement.Target)
        {
            UnlockAchievement(
                achievement,
                progress
            );
        }


        if (saveImmediately)
            PlayerDataManager.Instance.Save();


        OnAchievementProgressChanged?.Invoke(
            achievementId,
            progress.Progress,
            achievement.Target
        );
    }


    // =========================================================
    // SET PROGRESS
    // =========================================================

    private void SetProgress(
        string achievementId,
        int value,
        bool saveImmediately = true)
    {
        AchievementSO achievement =
            GetAchievement(achievementId);

        AchievementProgress progress =
            GetProgress(achievementId);


        if (achievement == null)
            return;

        if (progress == null)
            return;

        if (progress.Unlocked)
            return;


        progress.Progress =
            Mathf.Clamp(
                value,
                0,
                achievement.Target
            );


        if (progress.Progress >= achievement.Target)
        {
            UnlockAchievement(
                achievement,
                progress
            );
        }


        if (saveImmediately)
            PlayerDataManager.Instance.Save();


        OnAchievementProgressChanged?.Invoke(
            achievementId,
            progress.Progress,
            achievement.Target
        );
    }


    // =========================================================
    // UNLOCK
    // =========================================================

    private void UnlockAchievement(
        AchievementSO achievement,
        AchievementProgress progress)
    {
        if (progress.Unlocked)
            return;


        progress.Unlocked = true;

        progress.Progress =
            achievement.Target;

        progress.UnlockedDate =
            DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss"
            );


        // =====================================================
        // REWARD
        // =====================================================

        if (achievement.Reward > 0)
        {
            PlayerDataManager.Instance.AddCoins(
                achievement.Reward
            );
        }


        Debug.Log(
            $"🏆 Achievement Unlocked: " +
            $"{achievement.Title} " +
            $"+{achievement.Reward} Coins"
        );


        // =====================================================
        // EVENT
        // =====================================================

        OnAchievementUnlocked?.Invoke(
            achievement
        );
    }


    // =========================================================
    // LOGO SOLVED
    // =========================================================

    public void OnLogoSolved()
    {
        if (PlayerDataManager.Instance == null)
            return;


        int totalSolved =
            PlayerDataManager.Instance
                .GetTotalSolvedLogos();


        // -----------------------------------------------------
        // Total logo achievements
        // -----------------------------------------------------

        SetProgress(
            "first_guess",
            totalSolved,
            false
        );

        SetProgress(
            "logo_hunter",
            totalSolved,
            false
        );

        SetProgress(
            "brand_spotter",
            totalSolved,
            false
        );

        SetProgress(
            "logo_learner",
            totalSolved,
            false
        );

        SetProgress(
            "logo_expert",
            totalSolved,
            false
        );

        if (PlayerDataManager.Instance != null)
            PlayerDataManager.Instance.Save();


        Debug.Log(
            "Total Logos Solved: " +
            totalSolved
        );
    }


    // =========================================================
    // DAILY STREAK
    // =========================================================

    public void UpdateDailyStreakAchievements(
        int streak)
    {
        SetProgress(
            "getting_started",
            streak,
            false
        );

        SetProgress(
            "dedicated_player",
            streak,
            false
        );

        SetProgress(
            "logo_addict",
            streak,
            false
        );

        if (PlayerDataManager.Instance != null)
            PlayerDataManager.Instance.Save();
    }


    // =========================================================
    // CORRECT ANSWER STREAK
    // =========================================================

    public void UpdateAnswerStreak(
        int correctAnswerStreak)
    {
        SetProgress(
            "sharp_eye",
            correctAnswerStreak,
            false
        );

        SetProgress(
            "unstoppable",
            correctAnswerStreak,
            false
        );

        if (PlayerDataManager.Instance != null)
            PlayerDataManager.Instance.Save();
    }


    // =========================================================
    // SPEED DEMON
    // =========================================================

    public void CheckSpeedAchievement(
        float answerTime)
    {
        if (answerTime <= 5f)
        {
            SetProgress(
                "speed_demon",
                1
            );
        }
    }


    // =========================================================
    // NO HELP NEEDED
    // =========================================================

    public void AddNoHintSolved()
    {
        AddProgress(
            "no_help_needed"
        );
    }


    // =========================================================
    // CATEGORY COMPLETED
    // =========================================================

    public void UpdateCategoryAchievements(
        int completedCategories)
    {
        SetProgress(
            "category_explorer",
            completedCategories,
            false
        );

        SetProgress(
            "category_master",
            completedCategories,
            false
        );

        // For Category Legend, the target should be
        // the total number of categories.
        SetProgress(
            "category_legend",
            completedCategories,
            false
        );

        if (PlayerDataManager.Instance != null)
            PlayerDataManager.Instance.Save();
    }


    // =========================================================
    // CHECK UNLOCKED
    // =========================================================

    public bool IsUnlocked(string id)
    {
        AchievementProgress progress =
            GetProgress(id);

        return progress != null &&
               progress.Unlocked;
    }
}