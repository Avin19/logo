using UnityEngine;
using System.IO;
using System;


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

    public void AddCoins(int amount)
    {
        data.Coins += amount;
        Save();
    }

    public bool SpendCoins(int amount)
    {
        if (data.Coins >= amount)
        {
            data.Coins -= amount;
            Save();
            return true;
        }

        return false;
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

}