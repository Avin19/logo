using System.Collections.Generic;
using UnityEngine;

public class AchievementPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform content;
    [SerializeField] private AchievementCard achievementCardPrefab;

    private readonly List<AchievementCard> cards =
        new List<AchievementCard>();


    // =========================================================
    // ENABLE
    // =========================================================

    private void OnEnable()
    {
        if (AchievementManager.Instance == null)
            return;

        AchievementManager.Instance
            .OnAchievementUnlocked
            += OnAchievementUnlocked;

        AchievementManager.Instance
            .OnAchievementProgressChanged
            += OnAchievementProgressChanged;

        RefreshPanel();
    }


    // =========================================================
    // DISABLE
    // =========================================================

    private void OnDisable()
    {
        if (AchievementManager.Instance == null)
            return;

        AchievementManager.Instance
            .OnAchievementUnlocked
            -= OnAchievementUnlocked;

        AchievementManager.Instance
            .OnAchievementProgressChanged
            -= OnAchievementProgressChanged;
    }


    // =========================================================
    // REFRESH
    // =========================================================

    private void RefreshPanel()
    {
        ClearCards();

        List<AchievementSO> achievements =
            AchievementManager.Instance
                .GetAllAchievements();

        foreach (AchievementSO achievement in achievements)
        {
            if (achievement == null)
                continue;

            AchievementProgress progress =
                AchievementManager.Instance
                    .GetProgress(achievement.Id);

            if (progress == null)
                continue;

            AchievementCard card =
                Instantiate(
                    achievementCardPrefab,
                    content
                );

            card.Setup(
                achievement,
                progress
            );

            cards.Add(card);
        }
    }


    // =========================================================
    // PROGRESS UPDATE
    // =========================================================

    private void OnAchievementProgressChanged(
        string achievementId,
        int progress,
        int target)
    {
        foreach (AchievementCard card in cards)
        {
            if (card == null)
                continue;

            if (card.AchievementId == achievementId)
            {
                card.UpdateProgress(
                    progress,
                    target
                );

                break;
            }
        }
    }


    // =========================================================
    // ACHIEVEMENT UNLOCKED
    // =========================================================

    private void OnAchievementUnlocked(
        AchievementSO achievement)
    {
        if (achievement == null)
            return;

        foreach (AchievementCard card in cards)
        {
            if (card == null)
                continue;

            if (card.AchievementId == achievement.Id)
            {
                card.SetUnlocked();

                break;
            }
        }

        // Later:
        // AchievementPopup.Instance.Show(achievement);
    }


    // =========================================================
    // CLEAR
    // =========================================================

    private void ClearCards()
    {
        if (content == null)
            return;

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        cards.Clear();
    }
}