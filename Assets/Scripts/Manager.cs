using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manager.cs
/// - Loads categories.json from GitHub RAW (optional) -> overwrites persistent copy when downloaded
/// - Falls back to persistentDataPath -> StreamingAssets -> Resources
/// - Parses DataModel (cars/countries) and legacy Google Sheets `values` format
/// - Populates levelHolder with button prefab instances (pfButton)
/// - Downloads thumbnails with in-memory cache
/// - Keeps UI panel flow and ad hooks
/// </summary>
public class Manager : MonoBehaviour
{
    [Header("Manager")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Transform levelHolder;

    [Header("UI Panels")]
    [SerializeField] private Transform mainPanel;
    [SerializeField] private Transform welcomePanel;
    [SerializeField] private Transform levelPanel;
    [SerializeField] private Transform loadingPanel;
    [SerializeField] private Transform settingPanel;

    [Header("Buttons / Prefabs")]
    [SerializeField] private Button startBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button settingbtn;
    [SerializeField] private Button backToLevel;
    [SerializeField] private Button backToMenu;
    [SerializeField] private Button backToLevelMenu;
    [SerializeField] private GameObject pfButton; // prefab must contain Button + TextMeshProUGUI and optionally Image

    [Header("Ads Settings")]
    [Tooltip("Show interstitial every N times Start button is pressed")]
    [SerializeField] private int interstitialFrequency = 3;
    [Tooltip("Optional - wire a button in the Inspector to this for rewarded ad")]
    [SerializeField] private Button rewardedButton;

    [Header("Remote / JSON")]
    [Tooltip("Raw GitHub URL for categories.json (optional). Example: https://raw.githubusercontent.com/Avin19/Dataloader/main/categories.json")]
    [SerializeField] private string githubJsonUrl = "";
    [SerializeField] private int webRequestTimeout = 10;
    [Header("Which dataset to load from categories.json (cars / countries)")]
    [SerializeField] private string datasetToLoad = "cars";

    private const string INTERSTITIAL_COUNT_KEY = "interstitial_count";

    // Image cache
    private Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);
    private List<Coroutine> runningImageCoroutines = new List<Coroutine>();

    // store listeners so we can remove them
    private UnityEngine.Events.UnityAction startAction;
    private UnityEngine.Events.UnityAction quitAction;
    private UnityEngine.Events.UnityAction settingAction;
    private UnityEngine.Events.UnityAction backToLevelAction;
    private UnityEngine.Events.UnityAction backToMenuAction;
    private UnityEngine.Events.UnityAction backToLevelMenuAction;



    private void OnEnable()
    {
        // Wire UI listeners (store actions so remove works)
        if (startBtn != null)
        {
            startAction = StartButton;
            startBtn.onClick.AddListener(startAction);
        }

        if (quitBtn != null)
        {
            quitAction = () =>
            {
                if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();
                Application.Quit();
            };
            quitBtn.onClick.AddListener(quitAction);
        }

        if (settingbtn != null)
        {
            settingAction = SettingButton;
            settingbtn.onClick.AddListener(settingAction);
        }

        if (backToLevel != null)
        {
            backToLevelAction = BackToLevel;
            backToLevel.onClick.AddListener(backToLevelAction);
        }

        if (backToMenu != null)
        {
            backToMenuAction = BackToMenu;
            backToMenu.onClick.AddListener(backToMenuAction);
        }

        if (backToLevelMenu != null)
        {
            backToLevelMenuAction = BackToMenu;
            backToLevelMenu.onClick.AddListener(backToLevelMenuAction);
        }

        if (rewardedButton != null)
            rewardedButton.onClick.AddListener(ShowRewardedFromUI);


    }

    private void OnDisable()
    {
        startBtn.onClick.RemoveListener(startAction);
        quitBtn.onClick.RemoveListener(quitAction);
        settingbtn.onClick.RemoveListener(settingAction);
        backToLevel.onClick.RemoveListener(backToLevelAction);
        backToMenu.onClick.RemoveListener(backToMenuAction);
        backToLevelMenu.onClick.RemoveListener(backToLevelMenuAction);
        rewardedButton.onClick.RemoveListener(ShowRewardedFromUI);



        // stop running image coroutines
        foreach (var c in runningImageCoroutines)
            if (c != null) StopCoroutine(c);
        runningImageCoroutines.Clear();
    }

    private void Start()
    {
        SetAllThePanelFalse();
        if (welcomePanel != null) welcomePanel.gameObject.SetActive(true);

    }

    #region UI panel actions
    private void BackToMenu()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();
        SetAllThePanelFalse();
        if (welcomePanel != null) welcomePanel.gameObject.SetActive(true);
        AdManager.Instance.ShowRewarded();
    }

    private void SettingButton()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();
        SetAllThePanelFalse();
        if (settingPanel != null) settingPanel.gameObject.SetActive(true);
    }

    private void BackToLevel()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();
        if (levelManager != null)
            levelManager.Name = levelManager.Name; // noop, just keep
        SetAllThePanelFalse();
        if (levelPanel != null) levelPanel.gameObject.SetActive(true);

        AdManager.Instance.ShowInterstitial();

    }


    public void LoadingData()
    {
        SetAllThePanelFalse();
        if (loadingPanel != null) loadingPanel.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        SetAllThePanelFalse();
        if (mainPanel != null) mainPanel.gameObject.SetActive(true);
    }

    public void SetAllThePanelFalse()
    {
        if (welcomePanel != null) welcomePanel.gameObject.SetActive(false);
        if (mainPanel != null) mainPanel.gameObject.SetActive(false);
        if (levelPanel != null) levelPanel.gameObject.SetActive(false);
        if (settingPanel != null) settingPanel.gameObject.SetActive(false);
        if (loadingPanel != null) loadingPanel.gameObject.SetActive(false);
    }

    public void LevelLoaded()
    {
        SetAllThePanelFalse();
        if (levelPanel != null) levelPanel.gameObject.SetActive(true);



    }

    public void LoadingScreen(bool set)
    {
        if (loadingPanel != null) loadingPanel.gameObject.SetActive(set);
    }
    #endregion

    #region Start button & ads
    private void StartButton()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();

        int count = PlayerPrefs.GetInt(INTERSTITIAL_COUNT_KEY, 0) + 1;
        PlayerPrefs.SetInt(INTERSTITIAL_COUNT_KEY, count);
        PlayerPrefs.Save();

        bool shouldShowInterstitial = interstitialFrequency > 0 && (count % interstitialFrequency == 0);



        LoadingData();

        StartCoroutine(LoadCatgories());
    }

    public void ShowRewardedFromUI()
    {


    }

    private void HandleReward()
    {
        Debug.Log($"Grant reward: ");
    }
    #endregion
    private List<string> GetTopLevelKeys(string json)
    {
        var keys = new List<string>();

        try
        {
            var root = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            foreach (var kvp in root)
            {
                keys.Add(kvp.Key);  // "cars", "countries"
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[Manager] Failed to read top-level keys: " + ex.Message);
        }

        return keys;
    }


    #region Loading & Parsing categories.json (remote -> persistent -> streaming -> resources)
    private IEnumerator LoadCatgories()
    {
        string json = null;

        // 1) Try to download remote JSON and overwrite persistent file if URL provided and network available
        if (!string.IsNullOrEmpty(githubJsonUrl) && Application.internetReachability != NetworkReachability.NotReachable)
        {
            using (UnityWebRequest req = UnityWebRequest.Get(githubJsonUrl))
            {
                req.timeout = webRequestTimeout;
                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        json = req.downloadHandler.text;
                        WritePersistentJson(json);
                        Debug.Log("[Manager] Downloaded categories.json from GitHub and saved to persistentDataPath.");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("[Manager] Download succeeded but failed to write persistent file: " + ex.Message);
                        json = null;
                    }
                }
                else
                {
                    Debug.LogWarning("[Manager] Failed to download categories.json from GitHub: " + req.error);
                }
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(githubJsonUrl))
                Debug.Log("[Manager] No internet or githubJsonUrl empty; skipping remote fetch.");
        }

        // 2) If we didn't get JSON from remote, try persistentDataPath
        if (string.IsNullOrEmpty(json))
        {
            json = LoadJsonFromPersistent();
            if (!string.IsNullOrEmpty(json))
                Debug.Log("[Manager] Loaded categories.json from persistentDataPath.");
        }

        // 3) If still not found, try StreamingAssets (use UnityWebRequest on Android)
        if (string.IsNullOrEmpty(json))
        {
            json = LoadJsonFromStreamingAssets();
            if (!string.IsNullOrEmpty(json))
                Debug.Log("[Manager] Loaded categories.json from StreamingAssets.");
        }

        // 4) Fallback to Resources/categories.json
        if (string.IsNullOrEmpty(json))
        {
            TextAsset ta = Resources.Load<TextAsset>("categories");
            if (ta != null)
            {
                json = ta.text;
                Debug.Log("[Manager] Loaded categories.json from Resources.");
            }
        }

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("[Manager] categories.json not found in remote/persistent/StreamingAssets/Resources. Make sure file exists.");
            yield break;
        }
        List<string> categoryKeys = GetTopLevelKeys(json);

        if (categoryKeys.Count == 0)
        {
            Debug.LogError("[Manager] No keys found in categories.json");
            yield break;
        }

        // No URLs at this stage, only names
        PopulateButtons(categoryKeys, null);

        LevelLoaded();
        yield break;

    }

    private string LoadJsonFromPersistent()
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, "categories.json");
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Manager] Error reading persistent categories.json: " + ex.Message);
        }
        return null;
    }

    private string LoadJsonFromStreamingAssets()
    {
        string saPath = Path.Combine(Application.streamingAssetsPath, "categories.json");

        // On Android streaming assets are inside the APK so use UnityWebRequest
        if (Application.platform == RuntimePlatform.Android)
        {
            // This function cannot perform UnityWebRequest synchronously — return null and let coroutine try it.
            // But we can attempt a synchronous fallback by reading persistent/Resources first; here return null.
            return null;
        }

        // Editor/Desktop can read directly
        try
        {
            if (File.Exists(saPath))
                return File.ReadAllText(saPath);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Manager] Error reading StreamingAssets categories.json: " + ex.Message);
        }

        return null;
    }

    private void WritePersistentJson(string json)
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, "categories.json");
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(path, json);

        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Manager] Failed to write categories.json to persistent: " + ex.Message);
        }
    }
    #endregion

    #region Populate buttons / images
    private void PopulateButtons(List<string> names, List<string> urls)
    {
        if (pfButton == null)
        {
            Debug.LogError("[Manager] pfButton prefab not assigned. Please assign the button prefab in the Inspector.");
            return;
        }

        if (levelHolder == null)
        {
            Debug.LogError("[Manager] levelHolder not assigned. Please assign a RectTransform (level container) in the Inspector or name a GameObject 'LevelHolder'.");
            return;
        }

        // Clear existing children
        for (int i = levelHolder.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(levelHolder.GetChild(i).gameObject);
        }

        for (int i = 0; i < names.Count; i++)
        {
            string name = names[i];
            string url = (urls != null && i < urls.Count) ? urls[i] : string.Empty;

            GameObject btnGO = Instantiate(pfButton, levelHolder);
            btnGO.name = "LevelBtn_" + name;

            // Set text (TextMeshProUGUI)
            TextMeshProUGUI txt = btnGO.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = name;
            // else Debug.LogWarning("[Manager] No TextMeshProUGUI found in pfButton prefab.");

            // // Find image
            // Image img = btnGO.GetComponentInChildren<Image>();
            // if (img == null) img = btnGO.GetComponent<Image>();

            // // Attach UrlHolder
            // UrlHolder uh = btnGO.GetComponent<UrlHolder>();
            // if (uh == null) uh = btnGO.AddComponent<UrlHolder>();
            // uh.url = url;

            // // Download thumbnail if available
            // if (!string.IsNullOrEmpty(url) && img != null)
            // {
            //     if (textureCache.TryGetValue(url, out Texture2D cachedTex) && cachedTex != null)
            //     {
            //         AssignTextureToImage(cachedTex, img);
            //     }
            //     else
            //     {
            //         Coroutine c = StartCoroutine(DownloadAndAssignImage(url, img));
            //         runningImageCoroutines.Add(c);
            //     }
            // }

            // Wire button click: build List<ItemDetail> and send to levelManager
            // Button b = btnGO.GetComponent<Button>();
            // if (b != null)
            // {
            //     List<string> namesCopy = names;
            //     List<string> urlsCopy = urls;
            //     string capturedName = name;
            //     int capturedIndex = i;

            //     b.onClick.AddListener(() =>
            //     {
            //         if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();

            //         // Build items from the entire list (names+urls)
            //         List<ItemDetail> items = new List<ItemDetail>();
            //         for (int j = 0; j < (namesCopy?.Count ?? 0); j++)
            //         {
            //             string m = namesCopy[j] ?? string.Empty;
            //             string u = (urlsCopy != null && j < urlsCopy.Count) ? urlsCopy[j] ?? string.Empty : string.Empty;
            //             items.Add(new ItemDetail(m, u));
            //         }

            //         // send to LevelManager
            //         if (levelManager != null)
            //         {
            //             levelManager.SetItemdetails(new List<ItemDetail>(items));
            //         }

            //     });
            // }
            // else
            // {
            //     Debug.LogWarning("[Manager] pfButton prefab has no Button component.");
            // }
        }
    }

    // private IEnumerator DownloadAndAssignImage(string url, Image targetImage)
    // {
    //     if (string.IsNullOrEmpty(url) || targetImage == null)
    //         yield break;

    //     using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
    //     {
    //         uwr.timeout = webRequestTimeout;
    //         yield return uwr.SendWebRequest();

    //         if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
    //         {
    //             Debug.LogWarning($"[Manager] Failed to download image from {url} : {uwr.error}");
    //             yield break;
    //         }

    //         Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
    //         if (tex != null)
    //         {
    //             if (!textureCache.ContainsKey(url)) textureCache[url] = tex;
    //             AssignTextureToImage(tex, targetImage);
    //         }
    //     }
    // }

    // private void AssignTextureToImage(Texture2D tex, Image img)
    // {
    //     if (tex == null || img == null) return;
    //     Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
    //     img.sprite = s;
    //     img.preserveAspect = true;
    // }
    #endregion

    #region Helpers & small classes
    // Holds URL on instantiated button gameObject
    public class UrlHolder : MonoBehaviour { public string url; }

    [Serializable]
    public class DataModel { public NamedList cars; public NamedList countries; }

    [Serializable]
    public class NamedList { public List<string> names; public List<string> urls; }

    [Serializable]
    public class LegacyRoot { public List<List<string>> values; }
    #endregion
}
