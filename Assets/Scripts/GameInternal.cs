using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;


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

    private void Start()
    {
        items = levelManager.GetItemDetails();

        StartCoroutine(LoadImage(items[0].LogoURL.ToString()));
        logoText.text = items[0].Manufacturer.ToString();

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


