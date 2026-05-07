using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameInternal : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private RectTransform _winPanel, _lossPanel;
    [Header("Managers / UI")]
    [SerializeField] private Manager manager;
    [SerializeField] private Image logoImage;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Prefabs & Parents")]
    [SerializeField] private GameObject pfCorrectAnwser;
    [SerializeField] private GameObject pfRandomLetter;
    [SerializeField] private GameObject userAnswer;
    [SerializeField] private GameObject randomAnwser;

    [Header("Buttons")]
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button clearBTn;
    [SerializeField] private Button hintBtn;
    [SerializeField] private Button removeWrongBtn;
    [SerializeField] private Button revealBtn;
    [SerializeField] private Button skipBtn;
    [SerializeField] private Button watchAdBtn;
    [SerializeField] private Button nextWinBtn;
    [SerializeField] private Button tryAgainBtn;
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

    #region UNITY

    private void OnEnable()
    {
        nextBtn?.onClick.AddListener(SkipLevel);
        clearBTn?.onClick.AddListener(RemoveLastLetter);
        hintBtn?.onClick.AddListener(RevealLetter);
        removeWrongBtn?.onClick.AddListener(RemoveWrongLetters);
        revealBtn?.onClick.AddListener(RevealLetter);
        skipBtn?.onClick.AddListener(SkipLevel);
        watchAdBtn?.onClick.AddListener(RequestHintAd);
        nextWinBtn?.onClick.AddListener(WinNextButton);
        tryAgainBtn?.onClick.AddListener(LossTryAgainButton);
    }

    private void WinNextButton()
    {
        Debug.Log("Next button Clicked");
        LoadNextDirect();
        _winPanel.gameObject.SetActive(false);
        _lossPanel.gameObject.SetActive(false);
    }

    private void LossTryAgainButton()
    {
        _winPanel.gameObject.SetActive(false);
        _lossPanel.gameObject.SetActive(false);
        LoadNextDirect();
    }

    private void OnDisable()
    {
        nextBtn?.onClick.RemoveAllListeners();
        hintBtn?.onClick.RemoveAllListeners();
        removeWrongBtn?.onClick.RemoveAllListeners();
        skipBtn?.onClick.RemoveAllListeners();
        watchAdBtn?.onClick.RemoveAllListeners();
        nextBtn?.onClick.RemoveAllListeners();
        tryAgainBtn.onClick.RemoveAllListeners();
        revealBtn.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        hintPoints = PlayerDataManager.Instance.Hint;
        score = PlayerDataManager.Instance.Hint;
        UpdateUI();
        AdMobManager.Instance.ShowBanner();
    }

    #endregion

    #region GAME FLOW

    public void LoadCategoryById(CategorySO category)
    {
        Restart();

        items = CategoryToItemMapper.Map(category);
        itemCount = 0;

        StartGame();
    }

    public void StartGame()
    {
        if (items.Count == 0) return;
        LoadGameData();
    }

    private void LoadGameData()
    {
        Restart();

        var chosen = items[Random.Range(0, items.Count)];
        correctAnswer = chosen.Manufacturer.Trim().ToLower();

        foreach (char c in correctAnswer)
        {
            answerChars.Add(c);

            var go = Instantiate(pfCorrectAnwser, userAnswer.transform);
            var handler = go.GetComponent<AnswerTexthandler>();
            handler.SetText("");
            answerLetter.Add(handler);
        }

        CreateRandomLetters();
        logoImage.sprite = chosen.LogoURL;

        fillIndex = 0;
        manager.LoadingScreen(false);
    }

    #endregion

    #region INPUT

    public void ButtonClicked(TextHandler th)
    {
        if (fillIndex >= answerLetter.Count) return;

        string letter = th.GetText();
        if (string.IsNullOrEmpty(letter)) return;

        answerLetter[fillIndex].SetText(letter);
        th.gameObject.SetActive(false);

        fillIndex++;

        if (fillIndex >= answerLetter.Count)
            CheckAnswer();
    }

    #endregion

    #region ANSWER CHECK

    private void CheckAnswer()
    {
        bool correct = true;

        for (int i = 0; i < answerLetter.Count; i++)
        {
            if (answerLetter[i].GetText().ToLower() != answerChars[i].ToString())
            {
                correct = false;
                break;
            }
        }

        if (correct)
        {

            SoundManager.Instance?.CorrectAnswer();
            _winPanel.gameObject.SetActive(true);
            _lossPanel.gameObject.SetActive(false);
            AdMobManager.Instance.ShowRewarded(() => PlayerDataManager.Instance.data.Coins += 10);
        }
        else
        {
            score = Mathf.Max(0, score - 2);
            SoundManager.Instance?.WrongAnswer();
            _winPanel.gameObject.SetActive(false);
            _lossPanel.gameObject.SetActive(true);
            AdMobManager.Instance.ShowRewarded(() => PlayerDataManager.Instance.data.Coins -= score);
        }

        PlayerDataManager.Instance.data.Coins = score;
        UpdateUI();

        LoadNextDirect();
    }

    #endregion

    #region BUTTON FEATURES

    public void RevealLetter()
    {
        if (hintPoints <= 0)
        {
            RequestHintAd();
            return;
        }

        for (int i = 0; i < answerLetter.Count; i++)
        {
            if (answerLetter[i].GetText().ToLower() != answerChars[i].ToString())
            {
                hintPoints--;
                answerLetter[i].SetText(answerChars[i].ToString());
                fillIndex = i + 1;
                break;
            }
        }

        SaveHints();
        AdMobManager.Instance.TryShowInterstitial();
    }

    public void RemoveLastLetter()
    {
        if (hintPoints <= 0)
        {
            RequestHintAd();
            return;
        }
        for (int i = answerLetter.Count - 1; i >= 0; i--)
        {
            if (!string.IsNullOrEmpty(answerLetter[i].GetText()))
            {
                answerLetter[i].SetText("");
                fillIndex = i;
                break;
            }
        }
        AdMobManager.Instance.TryShowInterstitial();
    }

    public void RemoveWrongLetters()
    {
        if (hintPoints < 2)
        {
            RequestHintAd();
            return;
        }

        hintPoints -= 2;

        foreach (var l in randomLetterList)
        {
            if (!answerChars.Contains(l.GetText().ToLower()[0]))
                l.gameObject.SetActive(false);
        }

        SaveHints();
        AdMobManager.Instance.TryShowInterstitial();
    }

    public void SkipLevel()
    {
        if (hintPoints >= 3)
        {
            hintPoints -= 3;
            SaveHints();
            LoadNextDirect();
        }
        else
        {
            AdMobManager.Instance.ShowRewarded(() =>
            {
                LoadNextDirect();
            });
        }
        AdMobManager.Instance.TryShowInterstitial();
    }

    private void LoadNextDirect()
    {
        itemCount = (itemCount + 1) % items.Count;
        LoadGameData();
    }

    #endregion

    #region ADS / HINTS

    public void RequestHintAd()
    {
        AdMobManager.Instance.ShowRewarded(() =>
        {
            hintPoints += 5;
            SaveHints();
        });
    }

    private void SaveHints()
    {
        PlayerDataManager.Instance.data.hint = hintPoints;
        //PlayerPrefs.SetInt(HINT_KEY, hintPoints);
        UpdateUI();
    }

    #endregion

    #region RANDOM LETTERS

    private void CreateRandomLetters()
    {
        foreach (var r in randomLetterList)
            Destroy(r.gameObject);

        randomLetterList.Clear();

        List<char> pool = new(answerChars);

        char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        while (pool.Count < randomLetterCount)
            pool.Add(alphabet[UnityEngine.Random.Range(0, alphabet.Length)]);

        Shuffle(pool);

        for (int i = 0; i < randomLetterCount; i++)
        {
            var go = Instantiate(pfRandomLetter, randomAnwser.transform);
            var th = go.GetComponent<TextHandler>();
            th.SetText(pool[i].ToString());
            randomLetterList.Add(th);
        }
    }

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    #endregion

    #region UI

    private void UpdateUI()
    {
        scoreText.text = PlayerDataManager.Instance.data.Coins.ToString();
        hintText.text = PlayerDataManager.Instance.data.hint.ToString();
    }

    #endregion

    #region CLEANUP

    public void Restart()
    {
        foreach (var a in answerLetter)
            Destroy(a.gameObject);

        foreach (var r in randomLetterList)
            Destroy(r.gameObject);

        answerLetter.Clear();
        randomLetterList.Clear();
        answerChars.Clear();

        fillIndex = 0;
    }

    #endregion

}
