using System;


[Serializable]
public class PlayerData
{
    // Profile
    public string PlayerName;
    public string PlayerID;
    public PlayerType playerType;
    // Currency 
    public int Coins = 0;
    public int hint = 0;
    public int DailyStreak = 0;
    public string LastDailyStreakDate = "";
    //Progress 
    public int CurrentLevel = 1;
    public int HighestUnlockedLevel = 1;

    //Setting
    public bool SoundEnabled = true;
    public bool MusicEnabled = true;
    public float MusicVolume = 100f;
    public float SFXVolume = 100f;
    public bool Haptic = true;

    // Rewards 
    public string LastDailyRewardDate = "";
    public int DailyRewardStreak = 0;

}

public enum PlayerType
{
    Beginner,
    Explorer,
    LogoHunter,
    BrandExpert,
    LogoMaster,
    UltimateQuizzer,

}