using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class GameInternal : MonoBehaviour
{
    [Header("Managers / UI")]
    [SerializeField] private Manager manager;
    [SerializeField] private Image logoImage;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Prefabs & Parents")]
    [SerializeField] private GameObject pfCorrectAnwser;
    [SerializeField] private GameObject pfRandomLetter;
    [SerializeField] private GameObject userAnswer;
    [SerializeField] private GameObject randomAnwser;

    [Header("Navigation")]
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button preBtn;
    [SerializeField] private Button watchAdBtn;
    [SerializeField] private Button eraseBtn;
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("Game Settings")]
    [SerializeField] private int randomLetterCount = 20;

    private const string HINT_KEY = "HintPoints";

    private int hintPoints = 0;
    private int score = 0;
    private int fillIndex = 0;
    private int itemCount = 0;

    private List<ItemDetail> items = new();
    private readonly List<char> answerChars = new();
    private readonly List<TextHandler> randomLetterList = new();
    private readonly List<AnswerTexthandler> answerLetter = new();
    private readonly List<TextHandler> sourceLetterAtAnswerIndex = new(); // ★ TRACK USED TILES

    private readonly Dictionary<string, Sprite> spriteCache =
        new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);

    private string correctAnswer = "";

    //────────────────────────────────────────────────────

    #region Unity

    private void OnEnable()
    {
        nextBtn?.onClick.AddListener(OnNext);
        preBtn?.onClick.AddListener(OnPre);
        watchAdBtn?.onClick.AddListener(RequestHintAd);
        eraseBtn?.onClick.AddListener(RemoveLastLetter);
    }

    private void OnDisable()
    {
        nextBtn?.onClick.RemoveListener(OnNext);
        preBtn?.onClick.RemoveListener(OnPre);
        watchAdBtn?.onClick.RemoveListener(RequestHintAd);
        eraseBtn?.onClick.RemoveListener(RemoveLastLetter);
    }

    private void Start()
    {
        hintPoints = PlayerPrefs.GetInt(HINT_KEY, 0);
        UpdateHintUI();

        score = PlayerPrefs.GetInt("Score", 0);
        UpdateScoreUI();
    }

    #endregion

    //────────────────────────────────────────────────────

    #region Public API

    public void RequestHintAd()
    {
        AdManager.Instance?.ShowRewarded();
    }

    public void AddHintPoints(int amount)
    {
        hintPoints += amount;
        PlayerPrefs.SetInt(HINT_KEY, hintPoints);
        UpdateHintUI();
    }

    public void LoadCategoryById(string categoryId)
    {
        Restart();

        CategorySO category = CategoryRepository.GetById(categoryId);

        if (category == null)
        {
            Debug.LogWarning($"[GameInternal] Category '{categoryId}' not found.");
            return;
        }

        items = CategoryToItemMapper.Map(category);

        itemCount = 0;
        StartGame();
    }

    #endregion

    //────────────────────────────────────────────────────

    #region Game Flow

    public void StartGame()
    {
        if (items.Count == 0)
        {
            Debug.LogWarning("[GameInternal] Tried to start game with no items.");
            return;
        }

        fillIndex = 0;
        LoadGamedate();
    }

    private void LoadGamedate()
    {
        ClearPreviousRound();

        ItemDetail current = items[itemCount];

        if (current == null)
            return;

        correctAnswer = current.Manufacturer?.Trim() ?? "";

        BuildAnswerSlots();
        BuildRandomLetters();

        LoadImageIntoLogo(current.LogoURL);
    }

    #endregion

    //────────────────────────────────────────────────────

    #region Letter Input Handling

    public void ButtonClicked(TextHandler tile)
    {
        if (tile == null) return;

        string letter = tile.GetText();
        if (string.IsNullOrEmpty(letter)) return;

        if (fillIndex < answerLetter.Count)
        {
            answerLetter[fillIndex].SetText(letter);

            // TRACK TILE
            sourceLetterAtAnswerIndex[fillIndex] = tile;

            // DISABLE TILE
            tile.SetInteractable(false);

            fillIndex++;
        }

        if (fillIndex >= answerLetter.Count)
            CheckAnswer();
    }

    public void RemoveLastLetter()
    {
        if (hintPoints <= 0 || fillIndex <= 0)
            return;

        hintPoints--;
        PlayerPrefs.SetInt(HINT_KEY, hintPoints);
        UpdateHintUI();

        fillIndex--;

        // Clear answer slot
        answerLetter[fillIndex].SetText("");

        // RE-ENABLE SOURCE TILE
        TextHandler src = sourceLetterAtAnswerIndex[fillIndex];
        if (src != null)
        {
            src.SetInteractable(true);
            sourceLetterAtAnswerIndex[fillIndex] = null;
        }
    }

    #endregion

    //────────────────────────────────────────────────────

    #region UI Builders

    private void BuildAnswerSlots()
    {
        answerChars.Clear();
        answerLetter.Clear();
        sourceLetterAtAnswerIndex.Clear();

        foreach (char c in correctAnswer)
            answerChars.Add(c);

        foreach (char c in answerChars)
        {
            var go = Instantiate(pfCorrectAnwser, userAnswer.transform);
            var handler = go.GetComponent<AnswerTexthandler>();

            handler.SetText("");
            answerLetter.Add(handler);
            sourceLetterAtAnswerIndex.Add(null);
        }
    }

    private void BuildRandomLetters()
    {
        // Clean old tiles
        foreach (var t in randomLetterList)
            Destroy(t.gameObject);

        randomLetterList.Clear();

        // Create tiles
        for (int i = 0; i < randomLetterCount; i++)
        {
            var go = Instantiate(pfRandomLetter, randomAnwser.transform);
            randomLetterList.Add(go.GetComponent<TextHandler>());
        }

        // Build pool
        List<char> pool = new();

        foreach (var c in answerChars)
            if (!char.IsWhiteSpace(c))
                pool.Add(char.ToLowerInvariant(c));

        char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        while (pool.Count < randomLetterCount)
            pool.Add(alphabet[UnityEngine.Random.Range(0, alphabet.Length)]);

        Shuffle(pool);

        // Assign letters (enabled by default)
        for (int i = 0; i < randomLetterList.Count; i++)
        {
            randomLetterList[i].SetText(pool[i].ToString());
            randomLetterList[i].SetInteractable(true);
        }
    }

    #endregion

    //────────────────────────────────────────────────────

    #region Navigation

    private void OnNext()
    {
        if (items.Count == 0) return;

        itemCount = (itemCount + 1) % items.Count;
        LoadGamedate();
    }

    private void OnPre()
    {
        if (items.Count == 0) return;

        itemCount--;
        if (itemCount < 0)
            itemCount = items.Count - 1;

        LoadGamedate();
    }

    #endregion

    //────────────────────────────────────────────────────

    #region Answer Check

    private void CheckAnswer()
    {
        bool correct = true;

        for (int i = 0; i < answerLetter.Count; i++)
        {
            string user = answerLetter[i].GetText();
            if (!string.Equals(user, answerChars[i].ToString(),
                StringComparison.OrdinalIgnoreCase))
            {
                correct = false;
                break;
            }
        }

        if (correct)
        {
            score++;
            SoundManager.Instance?.CorrectAnswer();
        }
        else
        {
            SoundManager.Instance?.WrongAnswer();
            if (score > 0) score--;
        }

        PlayerPrefs.SetInt("Score", score);
        UpdateScoreUI();

        Restart();
        OnNext();
    }

    #endregion

    //────────────────────────────────────────────────────

    #region Utilities

    private IEnumerator LoadImage(string url)
    {
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
            yield break;

        Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
        Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                                 Vector2.one * 0.5f);

        spriteCache[url] = s;
        logoImage.sprite = s;
    }

    private void LoadImageIntoLogo(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;

        if (spriteCache.TryGetValue(url, out Sprite s))
            logoImage.sprite = s;
        else
            StartCoroutine(LoadImage(url));
    }

    private void UpdateScoreUI() =>
        scoreText.text = score.ToString();

    private void UpdateHintUI() =>
        hintText.text = hintPoints.ToString();

    private void Restart()
    {
        foreach (var a in answerLetter)
            Destroy(a.gameObject);

        foreach (var r in randomLetterList)
            Destroy(r.gameObject);

        answerChars.Clear();
        randomLetterList.Clear();
        answerLetter.Clear();
        sourceLetterAtAnswerIndex.Clear();

        fillIndex = 0;
    }

    private void ClearPreviousRound() => Restart();

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            int j = UnityEngine.Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    #endregion
}
