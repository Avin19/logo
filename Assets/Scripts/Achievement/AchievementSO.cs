using UnityEngine;

[CreateAssetMenu(
    fileName = "Achievement",
    menuName = "Logo Quiz/Achievement"
)]
public class AchievementSO : ScriptableObject
{
    [Header("Achievement")]
    public string Id;

    public string Title;

    [TextArea(2, 4)]
    public string Description;

    [Header("Progress")]
    public int Target = 1;

    [Header("Reward")]
    public int Reward = 10;

    [Header("Visual")]
    public Sprite Icon;
}