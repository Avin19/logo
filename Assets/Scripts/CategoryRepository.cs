using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class CategoryRepository
{
    private static List<CategorySO> cache;

    // Toggle this to enable / disable logs
    private const bool EnableLogging = true;
    private const string LogTag = "[CategoryRepository] ";

    /// <summary>
    /// Synchronous get-all. In Editor, prefers scanning Assets/CategoryAssets via AssetDatabase.
    /// In build/Android, prefers Resources (Resources/CategoryAssets).
    /// If you want dynamic JSON packs, call LoadFromStreamingAssetsJson(...) first to populate cache.
    /// </summary>
    public static List<CategorySO> GetAll()
    {
        if (EnableLogging)
            Debug.Log(LogTag + "GetAll() called. cache is " + (cache == null ? "null -> rebuilding" : "NOT null -> rebuilding anyway (forced)."));

        // If you want caching, uncomment this line:
        // if (cache != null) return cache;

        cache = new List<CategorySO>();

#if UNITY_EDITOR
        // Editor: try AssetDatabase folder first (fast for iteration)
        string assetsFolder = "Assets/Resources/CategoryAssets"; // adjust to your editor output folder
        if (AssetDatabase.IsValidFolder(assetsFolder))
        {
            string[] guids = AssetDatabase.FindAssets("t:CategorySO", new[] { assetsFolder });
            if (EnableLogging)
                Debug.Log(LogTag + $"Editor: Found {guids.Length} CategorySO GUIDs under '{assetsFolder}'.");

            foreach (var g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                var so = AssetDatabase.LoadAssetAtPath<CategorySO>(path);
                if (so != null)
                {
                    cache.Add(so);
                    if (EnableLogging)
                        Debug.Log(LogTag + $"Loaded CategorySO from AssetDatabase: id='{so.id}', path='{path}'.");
                }
            }
        }
        else if (EnableLogging)
        {
            Debug.LogWarning(LogTag + $"Editor: Folder '{assetsFolder}' does not exist.");
        }
#endif

        // Runtime builds (Android): use Resources (place assets under Assets/Resources/CategoryAssets/)
        if (cache.Count == 0)
        {
            if (EnableLogging)
                Debug.Log(LogTag + "cache empty after AssetDatabase check. Trying Resources.LoadAll(\"CategoryAssets\").");

            var fromResources = Resources.LoadAll<CategorySO>("CategoryAssets");
            if (fromResources != null && fromResources.Length > 0)
            {
                cache.AddRange(fromResources);
                if (EnableLogging)
                    Debug.Log(LogTag + $"Loaded {fromResources.Length} CategorySO from Resources/CategoryAssets.");
            }
            else if (EnableLogging)
            {
                Debug.LogWarning(LogTag + "Resources.LoadAll(\"CategoryAssets\") returned 0 results.");
            }
        }

        // Final fallback: try loading all resources in root
        if (cache.Count == 0)
        {
            if (EnableLogging)
                Debug.Log(LogTag + "cache still empty. Trying Resources.LoadAll(\"\") (root).");

            var root = Resources.LoadAll<CategorySO>("");
            if (root != null && root.Length > 0)
            {
                cache.AddRange(root.Where(x => x != null));
                if (EnableLogging)
                    Debug.Log(LogTag + $"Loaded {root.Length} CategorySO from Resources root.");
            }
            else if (EnableLogging)
            {
                Debug.LogWarning(LogTag + "Resources.LoadAll(\"\") returned 0 CategorySO assets.");
            }
        }

        if (EnableLogging)
            Debug.Log(LogTag + "GetAll() finished. Total categories in cache = " + cache.Count);

        return cache;
    }

    public static CategorySO GetById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            if (EnableLogging)
                Debug.LogWarning(LogTag + "GetById() called with null or empty id.");
            return null;
        }

        if (EnableLogging)
            Debug.Log(LogTag + $"GetById(\"{id}\") called.");

        var all = GetAll();
        var result = all.Find(c => string.Equals(c.id, id, StringComparison.OrdinalIgnoreCase));

        if (result == null && EnableLogging)
            Debug.LogWarning(LogTag + $"GetById(\"{id}\") -> NOT FOUND in {all.Count} categories.");
        else if (EnableLogging)
            Debug.Log(LogTag + $"GetById(\"{id}\") -> FOUND (displayName='{result.displayName}').");

        return result;
    }

    public static void ClearCache()
    {
        if (EnableLogging)
            Debug.Log(LogTag + "ClearCache() called. cache will be set to null.");

        cache = null;
    }

    /// <summary>
    /// Optional: populate cache by reading a JSON in StreamingAssets or remote and converting it to in-memory CategorySO instances.
    /// Requires Newtonsoft.Json (JObject) for flexible parsing of your dynamic-key JSON.
    /// Usage (from a MonoBehaviour): StartCoroutine(CategoryRepository.LoadFromStreamingAssetsJson(this, "categories.json", () => { /* done */ }));
    /// </summary>
    public static IEnumerator LoadFromStreamingAssetsJson(MonoBehaviour runner, string filename = "categories.json", Action onComplete = null)
    {
        if (EnableLogging)
            Debug.Log(LogTag + $"LoadFromStreamingAssetsJson() called with filename='{filename}'.");

        // Ensure cache exists and is cleared before populating
        cache = new List<CategorySO>();

        string path;
#if UNITY_ANDROID && !UNITY_EDITOR
        path = System.IO.Path.Combine(Application.streamingAssetsPath, filename);
        // On Android streamingAssetsPath is inside jar -> must use UnityWebRequest
#else
        path = "file://" + System.IO.Path.Combine(Application.streamingAssetsPath, filename);
#endif

        if (EnableLogging)
            Debug.Log(LogTag + $"StreamingAssets JSON full path: {path}");

        using (var uwr = UnityEngine.Networking.UnityWebRequest.Get(path))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError ||
                uwr.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogWarning(LogTag + $"Failed to load StreamingAssets JSON at '{path}': {uwr.error}");
                onComplete?.Invoke();
                yield break;
            }

            string json = uwr.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning(LogTag + "StreamingAssets JSON was empty.");
                onComplete?.Invoke();
                yield break;
            }

            if (EnableLogging)
                Debug.Log(LogTag + $"JSON loaded successfully ({json.Length} chars). Parsing...");

            // Parse dynamic-key JSON using Newtonsoft (JObject)
            try
            {
                var root = Newtonsoft.Json.Linq.JObject.Parse(json);
                int categoryCount = 0;

                foreach (var prop in root.Properties())
                {
                    string catKey = prop.Name; // e.g. "cars"
                    var catObj = prop.Value as Newtonsoft.Json.Linq.JObject;
                    if (catObj == null)
                    {
                        if (EnableLogging)
                            Debug.LogWarning(LogTag + $"Property '{catKey}' was not a JObject. Skipping.");
                        continue;
                    }

                    var names = catObj["names"] as Newtonsoft.Json.Linq.JArray;
                    var urls = catObj["urls"] as Newtonsoft.Json.Linq.JArray;
                    if (names == null || urls == null)
                    {
                        if (EnableLogging)
                            Debug.LogWarning(LogTag + $"Category '{catKey}' missing 'names' or 'urls' array. Skipping.");
                        continue;
                    }

                    var so = ScriptableObject.CreateInstance<CategorySO>();
                    so.id = SanitizeId(catKey);
                    so.displayName = UppercaseFirst(catKey);

                    int count = Math.Min(names.Count, urls.Count);
                    so.logos = new CategorySO.LogoEntry[count];
                    for (int i = 0; i < count; i++)
                    {
                        so.logos[i] = new CategorySO.LogoEntry
                        {
                            name = names[i]?.ToString(),
                            imageUrl = urls[i]?.ToString(),
                        };
                    }

                    cache.Add(so);
                    categoryCount++;

                    if (EnableLogging)
                        Debug.Log(LogTag + $"Parsed category '{so.id}' with {count} items from JSON.");
                }

                if (EnableLogging)
                    Debug.Log(LogTag + $"JSON parsing complete. Total categories added from JSON = {categoryCount}. cache size = {cache.Count}");
            }
            catch (Exception ex)
            {
                Debug.LogError(LogTag + $"JSON parse error: {ex}");
            }
        }

        if (EnableLogging)
            Debug.Log(LogTag + "LoadFromStreamingAssetsJson() finished. Invoking onComplete callback.");

        onComplete?.Invoke();
    }

    // small helpers
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
            if (!(char.IsLetterOrDigit(c) || c == '_')) arr[i] = '_';
        }
        return new string(arr);
    }
}
