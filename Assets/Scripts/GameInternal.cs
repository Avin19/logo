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
        StartCoroutine(LoadImage(items[itemCount].LogoURL.ToString()));
        logoText.text = items[itemCount].Manufacturer.ToString();


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
        }
    }

}


