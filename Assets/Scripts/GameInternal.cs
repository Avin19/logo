using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEditor.Build.Reporting;



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
    [SerializeField] private List<AnswerTexthandler> answerLetter = new List<AnswerTexthandler>();
    [SerializeField] private List<char> randomChars = new List<Char>();
    [SerializeField] private int count = 0;
    #region  Listener
    private void OnEnable()
    {
        nextBtn.onClick.AddListener(OnNext);
        preBtn.onClick.AddListener(OnPre);
    }
    private void OnDisable()
    {
        nextBtn.onClick.RemoveListener(OnNext);
        preBtn.onClick.RemoveListener(OnPre);
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
    #endregion

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
            GameObject answer = Instantiate(pfCorrectAnwser, userAnswer.transform);
            answerLetter.Add(answer.GetComponent<AnswerTexthandler>());
        }
        LetterGenerator();

    }
    private void LetterGenerator()
    {
        randomLetterList.Clear();
        if (correctAnswer.Length > 20)
        {
            LoadGamedate();
            Debug.Log("correctAnswer is greater then capacity");
            return;

        }
        for (int i = 0; i < 20; i++)
        {
            GameObject letters = Instantiate(pfRandomLetter, randomAnwser.transform);
            randomLetterList.Add(letters.GetComponent<TextHandler>());

        }
        RandomLetter();
        for (int i = 0; i < randomLetterList.Count; i++)
        {

            int index = UnityEngine.Random.Range(0, randomChars.Count);
            randomLetterList[i].SetText(randomChars[index].ToString());
            randomChars.RemoveAt(index);

        }

    }
    private void RandomLetter()
    {
        char[] alphabet = { 'a', 'b', 'c', 'd', 'e', 'f', 'g',
                        'h', 'i', 'j', 'k', 'l', 'm', 'n',
                        'o', 'p', 'q', 'r', 's', 't', 'u',
                        'v', 'w', 'x', 'y', 'z' };
        for (int i = 0; i < randomLetterList.Count; i++)
        {
            if (i < chars.Count)
            {
                randomChars.Add(chars[i]);
            }
            else
            {
                randomChars.Add(alphabet[UnityEngine.Random.Range(0, alphabet.Length)]);
            }
        }


    }
    private void Start()
    {
        items = levelManager.GetItems();
        count = 0;
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

        if (count < answerLetter.Count)
        {
            answerLetter[count].GetComponent<AnswerTexthandler>().SetText(textHandler.GetText());
            count++;
        }
        if (count == answerLetter.Count)
        {
            Debug.Log("Checking Answers Now ! ");
            bool check = false;
            for (int i = 0; i < answerLetter.Count; i++)
            {
                if (chars[i].ToString() == answerLetter[i].GetText())
                {
                    check = true;
                }
                else
                {
                    check = false;
                }
            }

            if (check)
            {
                // increase the point 
            }
            else
            {
                // reduce the heath
            }
            Restart();
            Start();
            //reload the game with new 

        }


    }
    public void Restart()
    {
        foreach (AnswerTexthandler o in answerLetter)
        {
            Destroy(o.gameObject);
        }
        foreach (TextHandler o in randomLetterList)
        {
            Destroy(o.gameObject);
        }

    }


}


