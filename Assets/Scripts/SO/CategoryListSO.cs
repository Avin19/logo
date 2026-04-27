using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CategorySOList", menuName = "LogoQuiz/CategoryListSO", order = 1)]
public class CategoryListSO : ScriptableObject
{
    public CategorySO[] categories;
}