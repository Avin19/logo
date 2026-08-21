using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public event Action<AchievementDefinition> OnAchievementUnlocked;
    public event Action<string, int, int> OnAchievementProgressChanged;

    private List<AchievementDefinition> achievements =
        new List<AchievementDefinition>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            CreateAchievements();
            InitializePlayerAchievements();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CreateAchievements()
    {
        achievements.Clear();

        // Logos
        Add("first_guess", "First Guess",
            "Solve your first logo", 1, 10);

        Add("logo_hunter", "Logo Hunter",
            "Solve 10 logos", 10, 25);

        Add("brand_spotter", "Brand Spotter",
            "Solve 25 logos", 25, 50);

        Add("logo_learner", "Logo Learner",
            "Solve 50 logos", 50, 100);

        Add("logo_expert", "Logo Expert",
            "Solve 100 logos", 100, 200);

        // Categories
        Add("category_explorer", "Category Explorer",
            "Complete 1 category", 1, 100);

        Add("category_master", "Category Master",
            "Complete 5 categories", 5, 500);

        Add("category_legend", "Category Legend",
            "Complete every category", 1, 3000);

        // Streak
        Add("sharp_eye", "Sharp Eye",
            "Get 5 correct answers in a row", 5, 50);

        Add("unstoppable", "Unstoppable",
            "Get 25 correct answers in a row", 25, 250);

        // Daily
        Add("getting_started", "Getting Started",
            "Reach a 3-day streak", 3, 50);

        Add("dedicated_player", "Dedicated Player",
            "Reach a 7-day streak", 7, 100);

        Add("logo_addict", "Logo Addict",
            "Reach a 14-day streak", 14, 250);

        // Special
        Add("speed_demon", "Speed Demon",
            "Solve a logo in under 5 seconds", 1, 50);

        Add("no_help_needed", "No Help Needed",
            "Solve 10 logos without hints", 10, 150);
    }

    private void Add(
        string id,
        string title,
        string description,
        int target,
        int reward)
    {
        achievements.Add(
            new AchievementDefinition
            {
                Id = id,
                Title = title,
                Description = description,
                Target = target,
                Reward = reward
            }
        );
    }

    private void InitializePlayerAchievements()
    {
        if (PlayerDataManager.Instance == null)
            return;

        if (PlayerDataManager.Instance.data.Achievements == null)
        {
            PlayerDataManager.Instance.data.Achievements =
                new List<AchievementProgress>();
        }

        foreach (AchievementDefinition definition in achievements)
        {
            AchievementProgress existing =
                PlayerDataManager.Instance.data.Achievements
                    .Find(x => x.AchievementId == definition.Id);

            if (existing == null)
            {
                PlayerDataManager.Instance.data.Achievements.Add(
                    new AchievementProgress
                    {
                        AchievementId = definition.Id,
                        Progress = 0,
                        Unlocked = false,
                        UnlockedDate = ""
                    }
                );
            }
        }

        PlayerDataManager.Instance.Save();
    }

    public AchievementDefinition GetAchievement(string id)
    {
        return achievements.Find(x => x.Id == id);
    }

    public AchievementProgress GetProgress(string id)
    {
        if (PlayerDataManager.Instance == null)
            return null;

        return PlayerDataManager.Instance.data.Achievements
            .Find(x => x.AchievementId == id);
    }

    public void AddProgress(
        string achievementId,
        int amount = 1)
    {
        AchievementDefinition definition =
            GetAchievement(achievementId);

        AchievementProgress progress =
            GetProgress(achievementId);

        if (definition == null ||
            progress == null ||
            progress.Unlocked)
        {
            return;
        }

        progress.Progress += amount;

        if (progress.Progress >= definition.Target)
        {
            progress.Progress = definition.Target;

            UnlockAchievement(
                definition,
                progress
            );
        }

        PlayerDataManager.Instance.Save();

        OnAchievementProgressChanged?.Invoke(
            achievementId,
            progress.Progress,
            definition.Target
        );
    }

    private void UnlockAchievement(
        AchievementDefinition definition,
        AchievementProgress progress)
    {
        if (progress.Unlocked)
            return;

        progress.Unlocked = true;

        progress.UnlockedDate =
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Give reward
        PlayerDataManager.Instance.AddCoins(
            definition.Reward
        );

        Debug.Log(
            $"Achievement Unlocked: {definition.Title}"
        );

        OnAchievementUnlocked?.Invoke(
            definition
        );
    }

    public bool IsUnlocked(string id)
    {
        AchievementProgress progress =
            GetProgress(id);

        return progress != null &&
               progress.Unlocked;
    }
    public void OnLogoSolved()
    {
        int totalSolved =
            PlayerDataManager.Instance.GetTotalSolvedLogos();

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
    }
    private void SetProgress(
    string achievementId,
    int value)
    {
        AchievementDefinition definition =
            GetAchievement(achievementId);

        AchievementProgress progress =
            GetProgress(achievementId);

        if (definition == null ||
            progress == null ||
            progress.Unlocked)
        {
            return;
        }

        progress.Progress =
            Mathf.Min(
                value,
                definition.Target
            );

        if (progress.Progress >=
            definition.Target)
        {
            UnlockAchievement(
                definition,
                progress
            );
        }

        OnAchievementProgressChanged?.Invoke(
            achievementId,
            progress.Progress,
            definition.Target
        );
    }

}