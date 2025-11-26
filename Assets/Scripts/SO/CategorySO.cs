using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CategorySO_", menuName = "LogoQuiz/CategorySO", order = 0)]
public class CategorySO : ScriptableObject
{
    public string id;
    public string displayName;
    public LogoEntry[] logos;

    [Serializable]
    public struct LogoEntry
    {
        public string name;
        public string imageUrl;
        // optionally: public Sprite imageSprite; <-- you can fill later at runtime
    }
}
