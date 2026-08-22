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

    private void InitializePlayerAchievements()
    {
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogWarning(
                "AchievementManager: PlayerDataManager not found."
            );

            return;
        }

        if (PlayerDataManager.Instance.data == null)
        {
            Debug.LogWarning(
                "AchievementManager: PlayerData is null."
            );

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
        int amount = 1)
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
        int value)
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
            totalSolved
        );

        SetProgress(
            "logo_hunter",
            totalSolved
        );

        SetProgress(
            "brand_spotter",
            totalSolved
        );

        SetProgress(
            "logo_learner",
            totalSolved
        );

        SetProgress(
            "logo_expert",
            totalSolved
        );


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
            streak
        );

        SetProgress(
            "dedicated_player",
            streak
        );

        SetProgress(
            "logo_addict",
            streak
        );
    }


    // =========================================================
    // CORRECT ANSWER STREAK
    // =========================================================

    public void UpdateAnswerStreak(
        int correctAnswerStreak)
    {
        SetProgress(
            "sharp_eye",
            correctAnswerStreak
        );

        SetProgress(
            "unstoppable",
            correctAnswerStreak
        );
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
            completedCategories
        );

        SetProgress(
            "category_master",
            completedCategories
        );

        // For Category Legend, the target should be
        // the total number of categories.
        SetProgress(
            "category_legend",
            completedCategories
        );
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