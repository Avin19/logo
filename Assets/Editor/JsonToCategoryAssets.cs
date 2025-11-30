
#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq; // ensure com.unity.nuget.newtonsoft-json package is installed

public static class JsonToCategoryAssets
{
    private const string OutputFolder = "Assets/CategoryAssets";

    [MenuItem("Tools/LogoQuiz/Import Categories From Selected JSON")]
    public static void ImportFromSelectedJson()
    {
        var selected = Selection.activeObject as TextAsset;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Import Categories",
                "Please select a TextAsset JSON file in the Project window first.", "OK");
            return;
        }

        try
        {
            var categories = ParseCategoriesFromJson(selected.text);

            if (!AssetDatabase.IsValidFolder(OutputFolder))
                AssetDatabase.CreateFolder("Assets", "CategoryAssets");

            int createdCount = 0;

            foreach (var cat in categories)
            {
                string assetPath = $"{OutputFolder}/{cat.id}.asset";

                CategorySO so = AssetDatabase.LoadAssetAtPath<CategorySO>(assetPath);
                if (so == null)
                {
                    so = ScriptableObject.CreateInstance<CategorySO>();
                    AssetDatabase.CreateAsset(so, assetPath);
                }

                so.id = cat.id;
                so.displayName = cat.displayName;
                so.logos = cat.logos
                    .Select(l => new CategorySO.LogoEntry
                    {
                        name = l.name,
                        imageUrl = l.imageUrl,
                    })
                    .ToArray();

                EditorUtility.SetDirty(so);
                createdCount++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Import Complete",
                $"Imported/Updated {createdCount} categories into '{OutputFolder}'.", "OK");
        }
        catch (Exception ex)
        {
            Debug.LogError($"JsonToCategoryAssets: Failed to import JSON: {ex}");
            EditorUtility.DisplayDialog("Import Error",
                "Failed to parse/import JSON. See console.", "OK");
        }
    }

    // -----------------
    // PUBLIC parser data classes
    // -----------------
    public class ParsedCategory
    {
        public string id;
        public string displayName;
        public List<ParsedLogo> logos = new List<ParsedLogo>();
    }

    public class ParsedLogo
    {
        public string id;
        public string name;
        public string imageUrl;
        public string[] hints = new string[0];
    }

    public static List<ParsedCategory> ParseCategoriesFromJson(string json)
    {
        var outList = new List<ParsedCategory>();
        var root = JObject.Parse(json);

        foreach (var prop in root.Properties())
        {
            string catKey = prop.Name;
            var catObj = prop.Value as JObject;
            if (catObj == null) continue;

            var names = catObj["names"] as JArray;
            var urls = catObj["urls"] as JArray;
            if (names == null || urls == null) continue;

            var parsedCat = new ParsedCategory
            {
                id = SanitizeId(catKey),
                displayName = UppercaseFirst(catKey)
            };

            int count = Math.Min(names.Count, urls.Count);
            for (int i = 0; i < count; i++)
            {
                parsedCat.logos.Add(new ParsedLogo
                {
                    id = $"{parsedCat.id}_{i}",
                    name = names[i]?.ToString(),
                    imageUrl = urls[i]?.ToString()
                });
            }

            outList.Add(parsedCat);
        }

        return outList;
    }

    private static string UppercaseFirst(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return char.ToUpper(s[0]) + s.Substring(1);
    }

    private static string SanitizeId(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        var arr = s.ToLowerInvariant().ToCharArray();
        for (int i = 0; i < arr.Length; i++)
        {
            char c = arr[i];
            if (!(char.IsLetterOrDigit(c) || c == '_'))
                arr[i] = '_';
        }
        return new string(arr);
    }
}
#endif
