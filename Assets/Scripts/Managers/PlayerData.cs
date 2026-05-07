using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    // Profile
    public string PlayerName = " Player";
    public string PlayerID = "";

    // Currency 
    public int Coins = 0;
    public int Gems = 0;

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
