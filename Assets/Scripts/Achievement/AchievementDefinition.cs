using System;
using UnityEngine;

[Serializable]
public class AchievementDefinition
{
    public string Id;
    public string Title;
    public string Description;
    public int Target;
    public int Reward;

    public Sprite Icon;
}