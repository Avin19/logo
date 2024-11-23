using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System;



// Get all the item from the LevelManger
// display the image url 
// maintain score and hint 
// check user Input


public class GameInternal : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Image logoImage;
    [SerializeField] private TextMeshProUGUI logoText;
    private List<ItemDetail> items = new List<ItemDetail>();

    [Header(" Button ")]
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button preBtn;
    [SerializeField] private int itemCount = 0;
    [SerializeField] private Manager manager;
    [SerializeField] private string correctAnswer;
    [SerializeField] private GameObject pfCorrectAnwser;
    [SerializeField] private GameObject pfRandomLetter;

    [SerializeField] private GameObject userAnswer;
    [SerializeField] private GameObject randomAnwser;



    // Can use queue in place of List . 
    [SerializeField] private List<char> chars = new List<Char>();

    private List<TextHandler> randomLetterList = new List<TextHandler>();
    private List<AnswerTexthandler> answerLetter = new List<AnswerTexthandler>();
    [SerializeField] private char[] randomchar = new char[20];
    private int count = -1;
    private void OnEnable()
    {
        nextBtn.onClick.AddListener(OnNext);
        preBtn.onClick.AddListener(OnPre);
    }

    private void OnPre()
    {
        if (itemCount == 0)
        {
            itemCount = items.Count - 1;
        }
        else
        {
            itemCount -= 1;
        }
        LoadGamedate();
    }
    private void LoadGamedate()
    {
        chars.Clear();
        manager.LoadingScreen(true);
        int randomNumber = UnityEngine.Random.Range(0, items.Count);
        StartCoroutine(LoadImage(items[randomNumber].LogoURL.ToString()));
        correctAnswer = items[randomNumber].Manufacturer.ToString();
        answerLetter.Clear();
        foreach (char c in correctAnswer)
        {
            chars.Add(c);
            answerLetter.Add(Instantiate(pfCorrectAnwser, userAnswer.transform).GetComponent<AnswerTexthandler>());
        }

        LetterGenerator();

    }

    private void LetterGenerator()
    {
        RandomLetter();
        randomLetterList.Clear();
        for (int i = 0; i < 20; i++)
        {
            GameObject letters = Instantiate(pfRandomLetter, randomAnwser.transform);
            randomLetterList.Add(letters.GetComponent<TextHandler>());
            letters.GetComponent<TextHandler>().SetText(randomchar[UnityEngine.Random.Range(0, randomchar.Length)].ToString());
        }
    }
    private void RandomLetter()
    {
        char[] alphabet = { 'a', 'b', 'c', 'd', 'e', 'f', 'g',
                        'h', 'i', 'j', 'k', 'l', 'm', 'n',
                        'o', 'p', 'q', 'r', 's', 't', 'u',
                        'v', 'w', 'x', 'y', 'z' };

        int index = 0;

        for (int i = 0; i < 20; i++)
        {

            if (i <= 3)
            {
                randomchar[i] = chars[i];
            }
            else
            {
                index = UnityEngine.Random.Range(0, alphabet.Length - 1);
                randomchar[i] = alphabet[index];
            }
        }


    }
    private void Start()
    {
        items = levelManager.GetItems();
        LoadGamedate();
    }

    private void OnNext()
    {
        if (itemCount == items.Count - 1)
        {
            itemCount = 0;
        }
        else
        {
            itemCount += 1;
        }
        LoadGamedate();
    }

    private IEnumerator LoadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("failed to load Iamge" + request.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            logoImage.sprite = sprite;
            manager.LoadingScreen(false);
        }
    }

    public void ButtonClicked(TextHandler textHandler)
    {
        count++;
        count = Mathf.Clamp(count, 0, answerLetter.Capacity - 1);
        if (randomLetterList.Contains(textHandler))
        {
            answerLetter[count].GetComponent<AnswerTexthandler>().SetText(textHandler.GetText());
        }
    }



}


