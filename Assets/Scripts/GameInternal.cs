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
    private int hintPoints;

    private List<ItemDetail> items = new();
    private int itemCount = 0;
    private string correctAnswer = "";
    private int score = 0;
    private int fillIndex = 0;

    private readonly List<char> answerChars = new();
    private readonly List<TextHandler> randomLetterList = new();
    private readonly List<AnswerTexthandler> answerLetter = new();

    private readonly Dictionary<string, Sprite> spriteCache = new(StringComparer.OrdinalIgnoreCase);

    #region UNITY

    private void OnEnable()
    {
        if (nextBtn) nextBtn.onClick.AddListener(OnNext);
        if (preBtn) preBtn.onClick.AddListener(OnPre);
        if (eraseBtn) eraseBtn.onClick.AddListener(RemoveLastLetter);
        if (watchAdBtn) watchAdBtn.onClick.AddListener(RequestHintAd);
    }

    private void OnDisable()
    {
        if (nextBtn) nextBtn.onClick.RemoveListener(OnNext);
        if (preBtn) preBtn.onClick.RemoveListener(OnPre);
        if (eraseBtn) eraseBtn.onClick.RemoveListener(RemoveLastLetter);
        if (watchAdBtn) watchAdBtn.onClick.RemoveListener(RequestHintAd);
    }

    private void Start()
    {
        hintPoints = PlayerPrefs.GetInt(HINT_KEY, 0);
        UpdateHintUI();

        score = PlayerPrefs.GetInt("Score", 0);
        UpdateScoreText();
    }

    #endregion

    #region CATEGORY LOAD

    public void LoadCategoryById(string categoryId)
    {
        Restart();

        CategorySO cat = CategoryRepository.GetById(categoryId);
        if (cat == null)
        {
            Debug.LogWarning($"[GameInternal] Category '{categoryId}' not found.");
            return;
        }

        items = CategoryToItemMapper.Map(cat);
        itemCount = 0;
        fillIndex = 0;

        StartGame();
    }

    #endregion
    public void ButtonClicked(TextHandler textHandler)
    {
        if (textHandler == null)
            return;

        string letter = textHandler.GetText();
        if (string.IsNullOrEmpty(letter))
            return;

        // Fill next empty answer slot
        if (fillIndex < answerLetter.Count)
        {
            answerLetter[fillIndex].SetText(letter);
            fillIndex++;
        }

        // If full -> check answer
        if (fillIndex >= answerLetter.Count)
        {
            CheckAnswer();
        }
    }

    #region GAME FLOW

    public void StartGame()
    {
        if (items.Count == 0)
        {
            Debug.LogWarning("[GameInternal] No items loaded.");
            return;
        }

        LoadGamedate();
    }

    private void LoadGamedate()
    {
        ClearPreviousRound();

        int index = Mathf.Clamp(itemCount, 0, items.Count - 1);
        ItemDetail chosen = items[index];
        if (chosen == null) return;

        correctAnswer = chosen.Manufacturer?.Trim() ?? "";

        // build answer slots
        answerChars.Clear();
        foreach (char c in correctAnswer)
            answerChars.Add(c);

        foreach (char c in answerChars)
        {
            var go = Instantiate(pfCorrectAnwser, userAnswer.transform);
            var handler = go.GetComponent<AnswerTexthandler>();
            handler.SetText("");
            answerLetter.Add(handler);
        }

        CreateRandomLetterSlots();

        // load current logo
        string url = chosen.LogoURL;

        if (!string.IsNullOrEmpty(url))
        {
            if (spriteCache.TryGetValue(url, out Sprite cached))
            {
                logoImage.sprite = cached;
                manager.LoadingScreen(false);
            }
            else
            {
                StartCoroutine(LoadImage(url));
            }
        }

        fillIndex = 0;

        // ✅ START PRELOADING NEXT LOGO
        PreloadNextItem();
    }

    #endregion

    #region IMAGE

    private IEnumerator LoadImage(string url)
    {
        if (spriteCache.ContainsKey(url)) yield break;

        using var uwr = UnityWebRequestTexture.GetTexture(url);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("[GameInternal] Failed to load image: " + uwr.error);
            yield break;
        }

        Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
        if (!tex) yield break;

        Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        spriteCache[url] = s;
        logoImage.sprite = s;

        manager.LoadingScreen(false);
    }

    private IEnumerator PreloadImage(string url)
    {
        if (spriteCache.ContainsKey(url)) yield break;

        using var uwr = UnityWebRequestTexture.GetTexture(url);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success) yield break;

        Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
        if (!tex) yield break;

        Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        spriteCache[url] = s;
    }

    private void PreloadNextItem()
    {
        if (items.Count == 0) return;

        int nextIndex = itemCount + 1 >= items.Count ? 0 : itemCount + 1;
        string url = items[nextIndex]?.LogoURL;

        if (!string.IsNullOrEmpty(url) && !spriteCache.ContainsKey(url))
            StartCoroutine(PreloadImage(url));
    }

    #endregion
    private void CheckAnswer()
    {
        bool allMatch = true;

        for (int i = 0; i < answerLetter.Count; i++)
        {
            string user = answerLetter[i].GetText() ?? "";
            char expected = (i < answerChars.Count) ? answerChars[i] : '\0';

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
            if (SoundManager.Instance != null)
                SoundManager.Instance.CorrectAnswer();

            score++;
        }
        else
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.WrongAnswer();

            if (score > 0)
                score--;
        }

        UpdateScoreText();
        PlayerPrefs.SetInt("Score", score);

        Restart();   // clear current letters

        OnNext();   // load next logo (which should already be preloaded)
    }

    #region NAVIGATION

    private void OnNext()
    {
        itemCount = (itemCount + 1) % items.Count;
        LoadGamedate();
    }

    private void OnPre()
    {
        itemCount--;
        if (itemCount < 0) itemCount = items.Count - 1;
        LoadGamedate();
    }

    #endregion

    #region RANDOM LETTERS

    private void CreateRandomLetterSlots()
    {
        foreach (var r in randomLetterList)
            Destroy(r.gameObject);
        randomLetterList.Clear();

        for (int i = 0; i < randomLetterCount; i++)
        {
            GameObject go = Instantiate(pfRandomLetter, randomAnwser.transform);
            var th = go.GetComponent<TextHandler>();
            randomLetterList.Add(th);
        }

        List<char> pool = new();

        foreach (char c in answerChars)
            if (!char.IsWhiteSpace(c))
                pool.Add(char.ToLower(c));

        char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        while (pool.Count < randomLetterCount)
            pool.Add(alphabet[UnityEngine.Random.Range(0, alphabet.Length)]);

        Shuffle(pool);

        for (int i = 0; i < randomLetterCount; i++)
            randomLetterList[i].SetText(pool[i].ToString());
    }

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            int j = UnityEngine.Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    #endregion

    #region HINT SYSTEM

    public void RequestHintAd() => AdManager.Instance.ShowRewarded();

    public void AddHintPoints(int amt)
    {
        hintPoints += amt;
        PlayerPrefs.SetInt(HINT_KEY, hintPoints);
        UpdateHintUI();
    }

    private void UpdateHintUI()
    {
        if (hintText)
            hintText.text = hintPoints.ToString();
    }

    public void RemoveLastLetter()
    {
        if (hintPoints <= 0 || fillIndex <= 0) return;

        hintPoints--;
        PlayerPrefs.SetInt(HINT_KEY, hintPoints);
        UpdateHintUI();

        fillIndex--;
        answerLetter[fillIndex].SetText("");
    }

    #endregion

    #region SCORE

    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    #endregion

    #region CLEANUP

    public void Restart()
    {
        foreach (var a in answerLetter)
            Destroy(a.gameObject);
        answerLetter.Clear();

        foreach (var r in randomLetterList)
            Destroy(r.gameObject);
        randomLetterList.Clear();

        answerChars.Clear();
        fillIndex = 0;
    }

    private void ClearPreviousRound()
    {
        Restart();
    }

    #endregion
}
