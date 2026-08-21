using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;


public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    public PlayerData data;
    public int Coins => data.Coins;
    public int Hint => data.hint;
    public bool SFX => data.SoundEnabled;
    public bool Music => data.MusicEnabled;
    public bool Haptic => data.Haptic;
    public string PlayerId => data.PlayerID;
    private string saveData;
    public event Action<int> OnDailyStreakChanged;

    public int DailyStreak => data.DailyStreak;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            saveData = Path.Combine(Application.persistentDataPath, "player.json");
            Load();
            CheckDailyStreak();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #region SAVE

    public void Save()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveData, json);

        Debug.Log(" Game Saved ");
    }

    #endregion
    #region LOAD

    public void Load()
    {
        if (File.Exists(saveData))
        {
            string json = File.ReadAllText(saveData);
            data = JsonUtility.FromJson<PlayerData>(json);

            Debug.Log("Save Loaded");

        }
        else
        {
            CreateNewSave();
        }
        if (string.IsNullOrEmpty(data.PlayerID))
        {
            data.PlayerID = GenerateUniqueID();
            Debug.Log("Generated Player ID: " + data.PlayerID);
            Save();
        }
        if (string.IsNullOrEmpty(data.PlayerName))
        {
            data.PlayerName = GenerateRandomName();
            Debug.Log("Generated Username: " + data.PlayerName);
            Save();
        }

    }

    private void CreateNewSave()
    {
        data = new PlayerData();

        if (string.IsNullOrEmpty(data.PlayerID))
        {
            data.PlayerID = GenerateUniqueID();
            Debug.Log("Generated Player ID: " + data.PlayerID);
            Save();
        }
        if (string.IsNullOrEmpty(data.PlayerName))
        {
            data.PlayerName = GenerateRandomName();
            Debug.Log("Generated Username: " + data.PlayerName);
            Save();
        }

        Save();
        Debug.Log("New Player Data Created");
    }

    string GenerateUniqueID()
    {
        return System.Guid.NewGuid().ToString();
    }
    string GenerateRandomName()
    {
        string[] adjectives = { "Silent", "Dark", "Shadow", "Swift", "Deadly", "Ghost", "Hidden", "Night" };
        string[] nouns = { "Hunter", "Assassin", "Ninja", "Sniper", "Blade", "Reaper", "Stalker", "Phantom" };

        string adj = adjectives[UnityEngine.Random.Range(0, adjectives.Length)];
        string noun = nouns[UnityEngine.Random.Range(0, nouns.Length)];
        int number = UnityEngine.Random.Range(10, 999);

        return adj + noun + number;
    }
    #endregion
    #region CURRENCY
    #region CATEGORY PROGRESS
    public int GetTotalSolvedLogos()
    {
        if (data == null ||
            data.CategoryProgress == null)
        {
            return 0;
        }

        int total = 0;

        foreach (CategoryProgress category in data.CategoryProgress)
        {
            if (category.SolvedQuestionIds != null)
            {
                total += category.SolvedQuestionIds.Count;
            }
        }

        return total;
    }
    public CategoryProgress GetCategoryProgress(
        string categoryId,
        int totalCount)
    {
        if (data.CategoryProgress == null)
        {
            data.CategoryProgress =
                new List<CategoryProgress>();
        }

        CategoryProgress progress =
            data.CategoryProgress.Find(
                x => x.CategoryId == categoryId
            );

        if (progress == null)
        {
            progress = new CategoryProgress
            {
                CategoryId = categoryId,
                TotalCount = totalCount
            };

            data.CategoryProgress.Add(progress);

            Save();
        }
        else
        {
            // Update total if category data changes
            progress.TotalCount = totalCount;
        }

        return progress;
    }


    /// <summary>
    /// Returns number of unique questions solved
    /// in a category.
    /// </summary>
    public int GetCategorySolvedCount(
        string categoryId)
    {
        if (data.CategoryProgress == null)
            return 0;

        CategoryProgress progress =
            data.CategoryProgress.Find(
                x => x.CategoryId == categoryId
            );

        if (progress == null ||
            progress.SolvedQuestionIds == null)
        {
            return 0;
        }

        return progress.SolvedQuestionIds.Count;
    }


    /// <summary>
    /// Marks a specific question as solved.
    /// The same question cannot be counted twice.
    /// </summary>
    public void CompleteCategoryQuestion(
        string categoryId,
        string questionId,
        int totalCount)
    {
        CategoryProgress progress =
            GetCategoryProgress(
                categoryId,
                totalCount
            );

        if (progress.SolvedQuestionIds == null)
        {
            progress.SolvedQuestionIds =
                new List<string>();
        }

        // Already solved
        if (progress.SolvedQuestionIds.Contains(questionId))
        {
            Debug.Log(
                $"Question already solved: {questionId}"
            );

            return;
        }

        // Add unique question
        progress.SolvedQuestionIds.Add(
            questionId
        );

        Save();

        Debug.Log(
            $"Category [{categoryId}] Progress: " +
            $"{progress.SolvedQuestionIds.Count}/" +
            $"{progress.TotalCount}"
        );
    }


    /// <summary>
    /// Returns true when every question in
    /// the category has been solved.
    /// </summary>
    public bool IsCategoryCompleted(
        string categoryId)
    {
        if (data.CategoryProgress == null)
            return false;

        CategoryProgress progress =
            data.CategoryProgress.Find(
                x => x.CategoryId == categoryId
            );

        if (progress == null ||
            progress.SolvedQuestionIds == null)
        {
            return false;
        }

        return progress.SolvedQuestionIds.Count >=
               progress.TotalCount;
    }


    /// <summary>
    /// Returns completion percentage from 0-1.
    /// </summary>
    public float GetCategoryProgressPercent(
        string categoryId)
    {
        if (data.CategoryProgress == null)
            return 0f;

        CategoryProgress progress =
            data.CategoryProgress.Find(
                x => x.CategoryId == categoryId
            );

        if (progress == null ||
            progress.TotalCount <= 0)
        {
            return 0f;
        }

        return (float)progress.SolvedQuestionIds.Count /
               progress.TotalCount;
    }

    #endregion
    #region LEVEL

    public void CompleteLevel(int level)
    {
        if (level >= data.HighestUnlockedLevel)
        {
            data.HighestUnlockedLevel = level + 1;
        }

        Save();
    }

    #endregion

    #region APPLICATION
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }
    #endregion

    #region DAILY STREAK

    private void CheckDailyStreak()
    {
        string today =
            DateTime.Now.ToString("yyyy-MM-dd");

        // ---------------------------------------
        // First time player
        // ---------------------------------------

        if (string.IsNullOrEmpty(data.LastDailyStreakDate))
        {
            data.DailyStreak = 1;
            data.LastDailyStreakDate = today;

            Save();

            Debug.Log(
                $"First daily login. Streak: {data.DailyStreak}"
            );

            OnDailyStreakChanged?.Invoke(
                data.DailyStreak
            );

            return;
        }

        // ---------------------------------------
        // Parse previous date
        // ---------------------------------------

        if (!DateTime.TryParse(
            data.LastDailyStreakDate,
            out DateTime lastDate))
        {
            data.DailyStreak = 1;
            data.LastDailyStreakDate = today;

            Save();

            OnDailyStreakChanged?.Invoke(
                data.DailyStreak
            );

            return;
        }

        DateTime todayDate = DateTime.Now.Date;
        DateTime previousDate = lastDate.Date;

        int difference =
            (todayDate - previousDate).Days;

        // ---------------------------------------
        // Same day
        // ---------------------------------------

        if (difference == 0)
        {
            Debug.Log(
                $"Already logged in today. Streak: {data.DailyStreak}"
            );

            return;
        }

        // ---------------------------------------
        // Next consecutive day
        // ---------------------------------------

        if (difference == 1)
        {
            data.DailyStreak++;

            // Maximum visual streak = 7
            if (data.DailyStreak > 7)
            {
                data.DailyStreak = 1;
            }

            data.LastDailyStreakDate = today;

            Save();

            Debug.Log(
                $"Daily streak increased: {data.DailyStreak}"
            );

            OnDailyStreakChanged?.Invoke(
                data.DailyStreak
            );

            return;
        }

        // ---------------------------------------
        // Player missed a day
        // ---------------------------------------

        data.DailyStreak = 1;
        data.LastDailyStreakDate = today;

        Save();

        Debug.Log(
            "Daily streak reset to 1."
        );

        OnDailyStreakChanged?.Invoke(
            data.DailyStreak
        );
    }

    #endregion
    #endregion

}