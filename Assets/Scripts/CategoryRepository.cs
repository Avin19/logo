using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Loads CategorySO assets. In Editor it will search Assets/CategoryAssets (AssetDatabase).
/// In Builds it will try Resources/CategoryAssets (so put .asset files there if you need runtime loading).
/// For production consider Addressables.
/// </summary>
public static class CategoryRepository
{
    private static List<CategorySO> cache;

    /// <summary>
    /// Returns all loaded CategorySO assets (cached after first load).
    /// </summary>
    public static List<CategorySO> GetAll()
    {
        if (cache != null) return cache;

        cache = new List<CategorySO>();

#if UNITY_EDITOR
        // Prefer editor AssetDatabase scan if available
        // Search in "Assets/CategoryAssets" first, then fallback to Resources
        string assetsFolder = "Assets/Scripts/SO/CategoryAssets";
        if (AssetDatabase.IsValidFolder(assetsFolder))
        {
            // find all .asset files in the folder
            string[] guids = AssetDatabase.FindAssets("t:CategorySO", new[] { assetsFolder });
            foreach (var g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                var so = AssetDatabase.LoadAssetAtPath<CategorySO>(path);
                if (so != null) cache.Add(so);
            }
        }
#endif

        // If nothing found yet, try Resources load (works in Editor & build if placed under Resources)
        if (cache.Count == 0)
        {
            var fromResources = Resources.LoadAll<CategorySO>("CategoryAssets");
            if (fromResources != null && fromResources.Length > 0)
            {
                cache.AddRange(fromResources);
            }
        }

        // Final fallback: try Resources root
        if (cache.Count == 0)
        {
            var rootResources = Resources.LoadAll<CategorySO>("");
            if (rootResources != null && rootResources.Length > 0)
            {
                cache.AddRange(rootResources.Where(x => x != null));
            }
        }

        return cache;
    }

    /// <summary>
    /// Get a category by its id (case-insensitive).
    /// </summary>
    public static CategorySO GetById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        var all = GetAll();
        return all.Find(c => string.Equals(c.id, id, System.StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Clear internal cache (useful while editing in editor to re-scan assets).
    /// </summary>
    public static void ClearCache()
    {
        cache = null;
    }
}
