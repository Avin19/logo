using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// GameInternal: drives the quiz UI using ItemDetail list provided by LevelManager.
/// - Loads logo images (with in-memory cache)
/// - Generates answer slots and random letters
/// - Maintains score using PlayerPrefs
/// - Handles next/previous navigation
/// 
/// Additional UI fields:
/// - itemNameText: shows the currently selected Manufacturer/name
/// - itemUrlText: shows the currently selected Logo URL (clickable behavior can be added)
/// - indexText: shows "currentIndex / total"
/// - allItemsText (optional): developer/debug text area listing all loaded items
/// </summary>
public class GameInternal : MonoBehaviour
{
    [Header("Managers / UI")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Manager manager;
    [SerializeField] private Image logoImage;
    [SerializeField] private TextMeshProUGUI logoText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Show item content (optional)")]
    [SerializeField] private TextMeshProUGUI itemNameText;   // shows current item name
    [SerializeField] private TextMeshProUGUI itemUrlText;    // shows current item logo URL
    [SerializeField] private TextMeshProUGUI indexText;      // shows "3 / 20"
    [SerializeField] private TextMeshProUGUI allItemsText;   // optional debug area listing all items

    [Header("Prefabs & Parents")]
    [SerializeField] private GameObject pfCorrectAnwser; // prefab with AnswerTexthandler
    [SerializeField] private GameObject pfRandomLetter;   // prefab with TextHandler
    [SerializeField] private GameObject userAnswer;       // parent for answer slots
    [SerializeField] private GameObject randomAnwser;     // parent for random letters

    [Header("Navigation")]
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button preBtn;

    [Header("Game Settings")]
    [SerializeField] private int randomLetterCount = 20;

    // Internal state
    private List<ItemDetail> items = new List<ItemDetail>();
    private int itemCount = 0; // index of current item
    private string correctAnswer = string.Empty;
    private int score = 0;
    private int fillIndex = 0;

    // Collections
    private readonly List<char> answerChars = new List<char>();
    private readonly List<char> randomChars = new List<char>();
    private readonly List<TextHandler> randomLetterList = new List<TextHandler>();
    private readonly List<AnswerTexthandler> answerLetter = new List<AnswerTexthandler>();

    // Simple in-memory cache to avoid re-downloading same image during a session
    private readonly Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);

    #region Unity lifecycle & listeners

    private void OnEnable()
    {
        if (nextBtn != null) nextBtn.onClick.AddListener(OnNext);
        if (preBtn != null) preBtn.onClick.AddListener(OnPre);

        if (levelManager != null)
            levelManager.OnItemsUpdated += OnItemsUpdated;
    }

    private void OnDisable()
    {
        if (nextBtn != null) nextBtn.onClick.RemoveListener(OnNext);
        if (preBtn != null) preBtn.onClick.RemoveListener(OnPre);

        if (levelManager != null)
            levelManager.OnItemsUpdated -= OnItemsUpdated;
    }

    private void Start()
    {
        // Load saved score
        if (PlayerPrefs.HasKey("Score"))
            score = PlayerPrefs.GetInt("Score");
        else
            PlayerPrefs.SetInt("Score", 0);

        UpdateScoreText();
    }

    #endregion

    #region Public handlers

    // Called by LevelManager when items are updated (if using the event)
    private void OnItemsUpdated(List<ItemDetail> newItems)
    {
        // Optionally use this to auto-start or refresh UI
        items = levelManager.GetItems();
        // If you want to auto-start when LevelManager updates, uncomment:
        // StartGame();

        // Update developer all-items panel if present
        RefreshAllItemsText();
    }

    public void StartGame()
    {
        // Defensive checks
        if (levelManager == null)
        {
            Debug.LogWarning("[GameInternal] levelManager not assigned.");
            return;
        }

        items = levelManager.GetItems() ?? new List<ItemDetail>();

        if (manager != null) manager.LoadingScreen(true);

        fillIndex = 0;
        itemCount = 0;

        // Update header text
        if (logoText != null)
            logoText.text = (string.IsNullOrEmpty(levelManager.Name) ? "QUIZ" : levelManager.Name + " QUIZ");

        if (items.Count == 0)
        {
            Debug.LogWarning("[GameInternal] No items available to start game.");
            if (manager != null) manager.LoadingScreen(false);
            return;
        }

        // Show all items in dev panel (if available)
        RefreshAllItemsText();

        // Load first game data
        LoadGamedate();
    }

    #endregion

    #region Navigation

    private void OnPre()
    {
        if (items.Count == 0) return;

        if (itemCount == 0)
            itemCount = items.Count - 1;
        else
            itemCount--;

        LoadGamedate();
    }

    private void OnNext()
    {
        if (items.Count == 0) return;

        if (itemCount >= items.Count - 1)
            itemCount = 0;
        else
            itemCount++;

        LoadGamedate();
    }

    #endregion

    #region Game flow

    private void LoadGamedate()
    {
        ClearPreviousRound();

        if (items == null || items.Count == 0)
        {
            Debug.LogWarning("[GameInternal] No items to load.");
            if (manager != null) manager.LoadingScreen(false);
            return;
        }

        // Choose a deterministic item based on itemCount rather than random each time when navigating
        int index = Mathf.Clamp(itemCount, 0, items.Count - 1);

        ItemDetail chosen = items[index];

        if (chosen == null)
        {
            Debug.LogWarning("[GameInternal] chosen item is null.");
            if (manager != null) manager.LoadingScreen(false);
            return;
        }

        // Update header and "content" UI
        correctAnswer = (chosen.Manufacturer ?? string.Empty).Trim();
        if (logoText != null)
            logoText.text = $"{(string.IsNullOrEmpty(levelManager?.Name) ? "QUIZ" : levelManager.Name + " QUIZ")}";

        // Set the content display (name/url/index)
        DisplayItemContent(chosen, index, items.Count);

        // Set answer characters
        answerChars.Clear();
        foreach (char c in correctAnswer)
        {
            // Include spaces as placeholders too (you may prefer to skip or show underscore)
            answerChars.Add(c);
        }

        // Instantiate answer slots
        foreach (char c in answerChars)
        {
            GameObject answerGO = Instantiate(pfCorrectAnwser, userAnswer.transform);
            var handler = answerGO.GetComponent<AnswerTexthandler>();
            if (handler != null)
            {
                handler.SetText(string.Empty); // start empty
                answerLetter.Add(handler);
            }
            else
            {
                Debug.LogWarning("[GameInternal] pfCorrectAnwser prefab missing AnswerTexthandler component.");
            }
        }

        // Prepare random letters and UI
        CreateRandomLetterSlots();

        // Start loading image (use cache if available)
        string url = chosen.LogoURL ?? string.Empty;
        if (!string.IsNullOrEmpty(url))
        {
            if (spriteCache.TryGetValue(url, out Sprite cached))
            {
                if (logoImage != null) logoImage.sprite = cached;
                if (manager != null) manager.LoadingScreen(false);
            }
            else
            {
                StartCoroutine(LoadImage(url));
            }
        }
        else
        {
            // No image URL — clear image and stop loading screen
            if (logoImage != null) logoImage.sprite = null;
            if (manager != null) manager.LoadingScreen(false);
        }

        // Reset fill index
        fillIndex = 0;
    }

    private void CreateRandomLetterSlots()
    {
        // Clear previous random letters
        foreach (var r in randomLetterList)
        {
            if (r != null && r.gameObject != null)
                Destroy(r.gameObject);
        }
        randomLetterList.Clear();
        randomChars.Clear();

        // create slots
        for (int i = 0; i < randomLetterCount; i++)
        {
            GameObject letters = Instantiate(pfRandomLetter, randomAnwser.transform);
            var th = letters.GetComponent<TextHandler>();
            if (th != null)
                randomLetterList.Add(th);
            else
                Debug.LogWarning("[GameInternal] pfRandomLetter prefab missing TextHandler component.");
        }

        // Build the pool: include all answer letters (excluding spaces optionally) and fill rest with random
        List<char> pool = new List<char>();

        // Option: remove spaces so players don't have to click them; here we include letters only
        foreach (char c in answerChars)
        {
            if (!char.IsWhiteSpace(c))
                pool.Add(char.ToLowerInvariant(c));
        }

        char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        while (pool.Count < randomLetterCount)
        {
            pool.Add(alphabet[UnityEngine.Random.Range(0, alphabet.Length)]);
        }

        // Shuffle pool
        Shuffle(pool);

        // Assign shuffled letters to UI
        for (int i = 0; i < randomLetterList.Count; i++)
        {
            string txt = pool[i].ToString();
            randomLetterList[i].SetText(txt);
        }
    }

    /// <summary>
    /// Simple Fisher-Yates shuffle
    /// </summary>
    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        for (int i = 0; i < n - 1; i++)
        {
            int j = UnityEngine.Random.Range(i, n);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }

    #endregion

    #region Image loading & cache

    private IEnumerator LoadImage(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            if (manager != null) manager.LoadingScreen(false);
            yield break;
        }

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            uwr.timeout = 10;
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogWarning("[GameInternal] Failed to load image: " + uwr.error);
                if (manager != null) manager.LoadingScreen(false);
                yield break;
            }

            Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
            if (tex == null)
            {
                if (manager != null) manager.LoadingScreen(false);
                yield break;
            }

            Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            spriteCache[url] = s;
            if (logoImage != null) logoImage.sprite = s;
            if (manager != null) manager.LoadingScreen(false);
        }
    }

    #endregion

    #region User interactions & checking

    // Called by letter button when user clicks a random letter (TextHandler should call this)
    public void ButtonClicked(TextHandler textHandler)
    {
        if (textHandler == null) return;

        string letter = textHandler.GetText();
        if (string.IsNullOrEmpty(letter)) return;

        // Find next empty answer slot
        if (fillIndex < answerLetter.Count)
        {
            answerLetter[fillIndex].SetText(letter);
            fillIndex++;
        }

        // If all filled, check answer
        if (fillIndex >= answerLetter.Count)
        {
            CheckAnswer();
        }
    }

    private void CheckAnswer()
    {
        bool allMatch = true;

        for (int i = 0; i < answerLetter.Count; i++)
        {
            string user = answerLetter[i].GetText() ?? string.Empty;
            char expected = (i < answerChars.Count) ? answerChars[i] : '\0';

            // If expected is whitespace, consider it matched if user is whitespace or empty
            if (char.IsWhiteSpace(expected))
            {
                if (!string.IsNullOrWhiteSpace(user))
                {
                    allMatch = false;
                    break;
                }
            }
            else
            {
                if (!string.Equals(user, expected.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    allMatch = false;
                    break;
                }
            }
        }

        if (allMatch)
        {
            if (SoundManager.Instance != null) SoundManager.Instance.CorrectAnswer();
            score++;
        }
        else
        {
            if (SoundManager.Instance != null) SoundManager.Instance.WrongAnswer();
            if (score > 0) score--;
        }

        UpdateScoreText();
        PlayerPrefs.SetInt("Score", score);

        // Reset for next round
        Restart();
        // move to next item automatically (optional) - here we move to next
        OnNext();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null) scoreText.text = score.ToString();
    }

    #endregion

    #region Content display helpers

    /// <summary>
    /// Updates the optional UI fields to show selected item content.
    /// </summary>
    private void DisplayItemContent(ItemDetail itemDetail, int index, int total)
    {
        if (itemNameText != null)
            itemNameText.text = itemDetail?.Manufacturer ?? string.Empty;

        if (itemUrlText != null)
            itemUrlText.text = itemDetail?.LogoURL ?? string.Empty;

        if (indexText != null)
            indexText.text = $"{(index + 1)} / {Math.Max(total, 1)}";

        // Also log to console for debugging
        Debug.Log($"[GameInternal] Showing item {index + 1}/{total}: Name='{itemDetail?.Manufacturer}', URL='{itemDetail?.LogoURL}'");
    }

    /// <summary>
    /// Fills the optional allItemsText with a compact list of all loaded items (name - url).
    /// Useful for a dev/debug panel.
    /// </summary>
    private void RefreshAllItemsText()
    {
        if (allItemsText == null) return;

        if (items == null || items.Count == 0)
        {
            allItemsText.text = "(no items)";
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < items.Count; i++)
        {
            var it = items[i];
            sb.AppendLine($"{i + 1}. {it.Manufacturer}  —  {it.LogoURL}");
        }

        allItemsText.text = sb.ToString();
    }

    #endregion

    #region Cleanup / reset

    public void Restart()
    {
        // Destroy answer slots
        foreach (AnswerTexthandler a in answerLetter)
        {
            if (a != null && a.gameObject != null)
                Destroy(a.gameObject);
        }
        answerLetter.Clear();

        // Destroy random letters
        foreach (TextHandler t in randomLetterList)
        {
            if (t != null && t.gameObject != null)
                Destroy(t.gameObject);
        }
        randomLetterList.Clear();

        // Reset helper lists and indices
        answerChars.Clear();
        randomChars.Clear();
        fillIndex = 0;

        // Optionally clear content display (do not force if you want to keep last shown)
        // if (itemNameText != null) itemNameText.text = string.Empty;
        // if (itemUrlText != null) itemUrlText.text = string.Empty;
        // if (indexText != null) indexText.text = string.Empty;
    }

    private void ClearPreviousRound()
    {
        // For safety, call Restart
        Restart();
    }

    #endregion
}
