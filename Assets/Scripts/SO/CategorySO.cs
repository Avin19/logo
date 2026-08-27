using UnityEngine;
using System;

public enum LogoDifficulty
{
    Easy,
    Medium,
    Hard
}

[CreateAssetMenu(fileName = "CategorySO", menuName = "LogoQuiz/CategorySO", order = 0)]
public class CategorySO : ScriptableObject
{
    public string category;
    public LogoEntry[] logos;

    [Serializable]
    public struct LogoEntry
    {
        public string name;
        public Sprite image;
        public LogoDifficulty difficulty;
        // optionally: public Sprite imageSprite; <-- you can fill later at runtime
    }
}
