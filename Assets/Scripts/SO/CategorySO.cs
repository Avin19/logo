using UnityEngine;
using System;

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
        // optionally: public Sprite imageSprite; <-- you can fill later at runtime
    }
}
