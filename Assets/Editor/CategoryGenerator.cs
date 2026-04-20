using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class CategoryGenerator
{
    private static string ROOT_PATH = "Assets/Sprites";
    private static string SAVE_PATH = "Assets/Data";

    [MenuItem("Tools/Generate Logo Quiz Data")]
    public static void Generate()
    {
        Directory.CreateDirectory(SAVE_PATH);

        List<CategorySO> allCategories = new List<CategorySO>();

        var categoryDirs = Directory.GetDirectories(ROOT_PATH);

        foreach (var categoryDir in categoryDirs)
        {
            string categoryName = Path.GetFileName(categoryDir);

            Debug.Log($"📂 Processing: {categoryName}");

            CategorySO categorySO = ScriptableObject.CreateInstance<CategorySO>();
            categorySO.category = categoryName;

            var files = Directory.GetFiles(categoryDir, "*.png");

            categorySO.logos = files.Select(file =>
            {
                string assetPath = file.Replace("\\", "/");
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

                return new CategorySO.LogoEntry
                {
                    name = CleanName(Path.GetFileNameWithoutExtension(file)),
                    image = sprite
                };
            }).ToArray();

            string assetPathSave = $"{SAVE_PATH}/{categoryName}.asset";
            AssetDatabase.CreateAsset(categorySO, assetPathSave);

            allCategories.Add(categorySO);

            Debug.Log($"✅ Created Category: {categoryName}");
        }

        // ✅ Create CategoryListSO
        CategoryListSO listSO = ScriptableObject.CreateInstance<CategoryListSO>();
        listSO.categories = allCategories.ToArray();

        AssetDatabase.CreateAsset(listSO, $"{SAVE_PATH}/CategoryList.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("🎉 All Categories + List Generated!");
    }

    static string CleanName(string raw)
    {
        return raw.ToLower()
                  .Replace("_", " ")
                  .Replace("-", " ")
                  .Trim();
    }
}