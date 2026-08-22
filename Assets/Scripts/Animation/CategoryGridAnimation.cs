using UnityEngine;

public class CategoryGridAnimation : MonoBehaviour
{
    [SerializeField] private Transform content;

    [Header("Animation")]
    [SerializeField] private float initialDelay = 0.1f;
    [SerializeField] private float delayBetweenItems = 0.07f;

    private void Start()
    {
        PlayAnimation();
    }

    public void PlayAnimation()
    {
        CategoryButtonAnimation[] buttons =
            content.GetComponentsInChildren<CategoryButtonAnimation>();

        for (int i = 0; i < buttons.Length; i++)
        {
            float delay =
                initialDelay +
                i * delayBetweenItems;

            buttons[i].PlayEntrance(delay);
        }
    }
}