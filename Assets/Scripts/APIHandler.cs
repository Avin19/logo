using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// JSON-only APIHandler: reads a local JSON file for the given sheetName and populates ItemDetail list.
/// Tries StreamingAssets/{sheetName}.json first (Android-safe), then Resources/{sheetName}.json (without extension).
/// Supports multiple JSON formats and dynamic top-level key lookup.
/// </summary>
[RequireComponent(typeof(Button))]
public class APIHandler : MonoBehaviour
{
    [Header("Sheet / Local JSON")]

    [SerializeField] private string sheetName;

    [Header("Output")]
    [SerializeField] private List<ItemDetail> item = new List<ItemDetail>();

    private LevelManager levelManager;
    private Button onbutton;

    // stored action for Add/Remove listener
    private UnityEngine.Events.UnityAction clickAction;

    // timeout for UnityWebRequest when reading StreamingAssets/Android
    [SerializeField] private int streamingAssetsTimeout = 10;

    private void Awake()
    {
        onbutton = GetComponent<Button>();
        levelManager = GetComponentInParent<LevelManager>();
    }

    private void Start()
    {
        // If sheetName is empty, attempt to read from child TMP text
        if (string.IsNullOrEmpty(sheetName))
        {
            TextMeshProUGUI tmp = GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) sheetName = tmp.text;
        }
        levelManager.StartCoroutine(LoadLocalOnly(sheetName));
    }

    private void OnEnable()
    {
        if (onbutton == null)
        {
            Debug.LogError("[APIHandler] Button component not found on GameObject.");
            return;
        }

        clickAction = () => OnButtonClick(sheetName);
        onbutton.onClick.AddListener(clickAction);
    }

    private void OnDisable()
    {
        if (onbutton != null && clickAction != null)
        {
            onbutton.onClick.RemoveListener(clickAction);
        }
    }

    /// <summary>
    /// Called when category button is clicked. Sets LevelManager.Name and starts the loader coroutine.
    /// </summary>
    private void OnButtonClick(string _sheetName)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();


    }


    /// <summary>
    /// Loads JSON from StreamingAssets/{sheetName}.json or Resources/{sheetName}.json
    /// Android: uses UnityWebRequest to read file inside APK.
    /// </summary>
    private IEnumerator LoadLocalOnly(string _sheetName)
    {
        if (string.IsNullOrEmpty(_sheetName))
        {
            Debug.LogError("[APIHandler] sheetName is empty.");
            yield break;
        }

        string jsonText = null;
        bool loaded = false;

        // 1) Try StreamingAssets (works for editor/desktop; on Android StreamingAssets are inside APK so use UnityWebRequest)
        string saPath = Path.Combine(Application.streamingAssetsPath + "categories.json");

        if (Application.platform == RuntimePlatform.Android)
        {
            // On Android streamingAssetsPath is inside the APK; use UnityWebRequest to read it
            string uri = saPath; // Unity accepts Path for Android if passed to UnityWebRequest.Get
            using (UnityWebRequest uwr = UnityWebRequest.Get(uri))
            {
                uwr.timeout = streamingAssetsTimeout;
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    jsonText = uwr.downloadHandler.text;
                    loaded = true;
                    Debug.Log($"[APIHandler] Loaded JSON from StreamingAssets (Android): {saPath}");
                }
                else
                {
                    Debug.Log($"[APIHandler] StreamingAssets read failed on Android: {uwr.error} -- trying Resources.");
                }
            }
        }
        else
        {
            // Editor / Desktop / iOS can read directly from file system
            if (File.Exists(saPath))
            {
                try
                {
                    jsonText = File.ReadAllText(saPath);
                    loaded = true;
                    Debug.Log($"[APIHandler] Loaded JSON from StreamingAssets: {saPath}");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[APIHandler] Error reading StreamingAssets file: {ex.Message}");
                }
            }
            else
            {
                Debug.Log($"[APIHandler] No StreamingAssets file at {saPath}");
            }
        }

        // 2) If not found, try Resources (embedded in build) — use Resources.Load without extension
        if (!loaded)
        {
            TextAsset ta = Resources.Load<TextAsset>("categories");
            if (ta != null)
            {
                jsonText = ta.text;
                loaded = true;
                Debug.Log($"[APIHandler] Loaded JSON from Resources: Resources/{_sheetName}.json");
            }
            else
            {
                Debug.Log($"[APIHandler] No Resources file found: Resources/{_sheetName}.json");
            }
        }

        if (!loaded || string.IsNullOrEmpty(jsonText))
        {
            Debug.LogError($"[APIHandler] Could not load local JSON for '{_sheetName}'. Place file in StreamingAssets or Resources.");
            yield break;
        }

        // Parse the JSON and populate items (synchronously)
        ParseAndPopulate(jsonText, _sheetName);

        // Notify level manager (defensive copy)
        if (levelManager != null)
            levelManager.SetItemdetails(new List<ItemDetail>(item));

        yield return null;
    }

    /// <summary>
    /// Parses supported JSON formats and fills the item list.
    /// Supports:
    ///  - legacy RootObjectLegacy values[][] format
    ///  - SimpleModel { names:[], urls:[] }
    ///  - Typed DataModel (cars/countries) for backward compatibility
    ///  - Dynamic top-level key lookup via JObject (case-insensitive)
    /// </summary>
    private void ParseAndPopulate(string jsonText, string contextSheetName)
    {
        if (string.IsNullOrEmpty(jsonText))
        {
            Debug.LogError("[APIHandler] Empty JSON passed to ParseAndPopulate.");
            return;
        }

        // Clear existing items to avoid duplicates
        item.Clear();

        // 1) Try legacy RootObject (values[][]) format
        try
        {
            RootObjectLegacy ro = JsonConvert.DeserializeObject<RootObjectLegacy>(jsonText);
            if (ro != null && ro.values != null && ro.values.Length > 0)
            {
                string[] manufacturers = ro.values.Length > 0 ? ro.values[0] : null;
                string[] logos = ro.values.Length > 1 ? ro.values[1] : null;

                if (manufacturers != null && manufacturers.Length > 0)
                {
                    for (int i = 0; i < manufacturers.Length; i++)
                    {
                        string m = manufacturers[i] ?? string.Empty;
                        string l = (logos != null && i < logos.Length) ? logos[i] ?? string.Empty : string.Empty;
                        item.Add(new ItemDetail(m, l));
                    }

                    Debug.Log($"[APIHandler] Parsed {item.Count} items from legacy values format.");
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[APIHandler] Legacy RootObject parse failed: " + ex.Message);
        }

        // 2) Try simple { "names": [...], "urls": [...] }
        try
        {
            SimpleModel simple = JsonConvert.DeserializeObject<SimpleModel>(jsonText);
            if (simple != null && simple.names != null && simple.names.Count > 0)
            {
                for (int i = 0; i < simple.names.Count; i++)
                {
                    string m = simple.names[i] ?? string.Empty;
                    string l = (simple.urls != null && i < simple.urls.Count) ? simple.urls[i] ?? string.Empty : string.Empty;
                    item.Add(new ItemDetail(m, l));
                }

                Debug.Log($"[APIHandler] Parsed {item.Count} items from SimpleModel format.");
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[APIHandler] SimpleModel parse failed: " + ex.Message);
        }

        // 3) Try structured DataModel { "cars": { names:[], urls:[] }, "countries": {...} }
        try
        {
            DataModel dm = JsonConvert.DeserializeObject<DataModel>(jsonText);
            if (dm != null)
            {
                NamedList pick = null;
                if (!string.IsNullOrEmpty(contextSheetName) && contextSheetName.Equals("cars", StringComparison.OrdinalIgnoreCase) && dm.cars != null)
                    pick = dm.cars;
                else if (!string.IsNullOrEmpty(contextSheetName) && contextSheetName.Equals("countries", StringComparison.OrdinalIgnoreCase) && dm.countries != null)
                    pick = dm.countries;
                else if (dm.cars != null) pick = dm.cars;
                else if (dm.countries != null) pick = dm.countries;

                if (pick != null && pick.names != null && pick.names.Count > 0)
                {
                    for (int i = 0; i < pick.names.Count; i++)
                    {
                        string m = pick.names[i] ?? string.Empty;
                        string l = (pick.urls != null && i < pick.urls.Count) ? pick.urls[i] ?? string.Empty : string.Empty;
                        item.Add(new ItemDetail(m, l));
                    }

                    Debug.Log($"[APIHandler] Parsed {item.Count} items from DataModel format (typed).");
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[APIHandler] DataModel typed parse failed: " + ex.Message);
        }

        // 4) Dynamic lookup: parse as JObject and find the requested top-level key (case-insensitive)
        try
        {
            var root = JObject.Parse(jsonText);
            if (root != null)
            {
                JProperty foundProp = null;
                if (!string.IsNullOrEmpty(contextSheetName))
                {
                    // find property case-insensitively
                    foundProp = root.Properties()
                                     .FirstOrDefault(p => string.Equals(p.Name, contextSheetName, StringComparison.OrdinalIgnoreCase));
                }

                if (foundProp != null)
                {
                    // Try to convert to NamedList first
                    var named = foundProp.Value.ToObject<NamedList>();
                    if (named != null && named.names != null && named.names.Count > 0)
                    {
                        for (int i = 0; i < named.names.Count; i++)
                        {
                            string m = named.names[i] ?? string.Empty;
                            string l = (named.urls != null && i < named.urls.Count) ? named.urls[i] ?? string.Empty : string.Empty;
                            item.Add(new ItemDetail(m, l));
                        }

                        Debug.Log($"[APIHandler] Parsed {item.Count} items from dynamic NamedList for key '{foundProp.Name}'.");
                        return;
                    }

                    // Fallback: simple model under the key
                    var simpleFallback = foundProp.Value.ToObject<SimpleModel>();
                    if (simpleFallback != null && simpleFallback.names != null && simpleFallback.names.Count > 0)
                    {
                        for (int i = 0; i < simpleFallback.names.Count; i++)
                        {
                            string m = simpleFallback.names[i] ?? string.Empty;
                            string l = (simpleFallback.urls != null && i < simpleFallback.urls.Count) ? simpleFallback.urls[i] ?? string.Empty : string.Empty;
                            item.Add(new ItemDetail(m, l));
                        }

                        Debug.Log($"[APIHandler] Parsed {item.Count} items from dynamic SimpleModel for key '{foundProp.Name}'.");
                        return;
                    }

                    // Another fallback: the key itself might be an array of strings (names only)
                    if (foundProp.Value.Type == JTokenType.Array)
                    {
                        var arr = foundProp.Value.ToObject<List<string>>();
                        if (arr != null && arr.Count > 0)
                        {
                            foreach (var s in arr)
                                item.Add(new ItemDetail(s ?? string.Empty, string.Empty));

                            Debug.Log($"[APIHandler] Parsed {item.Count} items from array for key '{foundProp.Name}'.");
                            return;
                        }
                    }
                }
                else
                {
                    // If key not found, log available top-level keys for debugging
                    var keys = root.Properties().Select(p => p.Name).ToArray();
                    Debug.LogWarning($"[APIHandler] Key '{contextSheetName}' not found in JSON. Available keys: {string.Join(", ", keys)}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[APIHandler] Dynamic JObject parse failed: " + ex.Message);
        }

        Debug.LogError("[APIHandler] Unable to parse JSON in any supported format. Check file content.");
    }

    #region Data models

    // Legacy google sheets RootObject (values array)
    [Serializable]
    public class RootObjectLegacy
    {
        public string range { get; set; }
        public string majorDimension { get; set; }
        public string[][] values { get; set; }
    }

    // Simple alternate JSON model: { "names": [...], "urls": [...] }
    [Serializable]
    private class SimpleModel
    {
        public List<string> names;
        public List<string> urls;
    }

    // Structured model: { "cars": { names:[], urls:[] }, "countries": { ... } }
    [Serializable]
    private class DataModel
    {
        public NamedList cars;
        public NamedList countries;
    }

    [Serializable]
    private class NamedList
    {
        public List<string> names;
        public List<string> urls;
    }

    #endregion
}
