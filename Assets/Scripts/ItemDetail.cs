using UnityEngine;

[System.Serializable]
public class ItemDetail
{
    public string Manufacturer; // mapped from LogoEntry.name
    public Sprite LogoURL;      // mapped from LogoEntry.imageUrl
    public LogoDifficulty Difficulty; // mapped from LogoEntry.difficulty
    // add other fields here as needed (id, hints etc.)
}