using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections.Generic;

[Serializable]
public class RootObject
{
    public string range { get; set; }
    public string majorDimension { get; set; }
    public string[][] values;
}
[Serializable]
public class ItemDetail
{
    public string Manufacturer { get; set; }
    public string LogoURL { get; set; }


    public ItemDetail(string manufacturer, string logoURL)
    {
        Manufacturer = manufacturer;
        LogoURL = logoURL;

    }
}

public class LevelManager : MonoBehaviour
{
    [SerializeField] private string iD = "1pYU1mu9NBDYt3Ls_IYxMtbnaNrJ_t2jZxy7MYGFLjEA";
    [SerializeField] private string apiKey = "AIzaSyAA23WLN6TWfFj_J1VXvYPUOCIMSXGo254";
    [SerializeField] private string sheetName = "Sheet1";

    [SerializeField] private Transform gamePanel;
    [SerializeField] private TextAsset jsonText;

    [SerializeField] private List<ItemDetail> item = new List<ItemDetail>();


    public List<ItemDetail> GetItemDetails()
    {
        return item;
    }

    private void OnCarButton()
    {

        sheetName = "CAR";

        StartCoroutine(GetData($"https://sheets.googleapis.com/v4/spreadsheets/{iD}/values/{sheetName}?key={apiKey}"));

    }

    IEnumerator GetData(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Offline");

        }
        string data = www.downloadHandler.text;

        RootObject c = JsonConvert.DeserializeObject<RootObject>(data);

        for (int i = 0; i < c.values[0].Length; i++)
        {


            item.Add(new ItemDetail(c.values[0][i], c.values[1][i]));
        }

        gamePanel.gameObject.SetActive(!gamePanel.gameObject.activeSelf);

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

            Sprite image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);



        }
    }
}



