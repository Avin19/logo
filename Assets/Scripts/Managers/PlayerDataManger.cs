using UnityEngine;
using System.IO;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    public PlayerData playerData;
    private string saveData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            saveData = Path.Combine(Application.persistentDataPath, "/player.json");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #region SAVE

    public void Save()
    {
        string json = JsonUtility.ToJson(playerData, true);
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
            playerData = JsonUtility.FromJson<PlayerData>(json);

            Debug.Log("Save Loaded");

        }
        else
        {
            CreateNewSave();
        }

    }

    private void CreateNewSave()
    {
        playerData = new PlayerData();

        if (string.IsNullOrEmpty(playerData.PlayerID))
        {
            playerData.PlayerID = GenerateUniqueID();
            Debug.Log("Generated Player ID: " + playerData.PlayerID);
            Save();
        }
        if (string.IsNullOrEmpty(playerData.PlayerName))
        {
            playerData.PlayerName = GenerateRandomName();
            Debug.Log("Generated Username: " + playerData.PlayerName);
            Save();
        }

        Save();
    }

    string GenerateUniqueID()
    {
        return System.Guid.NewGuid().ToString();
    }
    string GenerateRandomName()
    {
        string[] adjectives = { "Silent", "Dark", "Shadow", "Swift", "Deadly", "Ghost", "Hidden", "Night" };
        string[] nouns = { "Hunter", "Assassin", "Ninja", "Sniper", "Blade", "Reaper", "Stalker", "Phantom" };

        string adj = adjectives[Random.Range(0, adjectives.Length)];
        string noun = nouns[Random.Range(0, nouns.Length)];
        int number = Random.Range(10, 999);

        return adj + noun + number;
    }
    #endregion
    #region CURRENCY

    public void AddCoins(int amount)
    {
        playerData.Coins += amount;
        Save();
    }

    public bool SpendCoins(int amount)
    {
        if (playerData.Coins >= amount)
        {
            playerData.Coins -= amount;
            Save();
            return true;
        }

        return false;
    }

    #endregion
    #region LEVEL

    public void CompleteLevel(int level)
    {
        if (level >= playerData.HighestUnlockedLevel)
        {
            playerData.HighestUnlockedLevel = level + 1;
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



}