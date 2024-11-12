using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Here i will getting data from sheet 
// list of categories

[Serializable]
public class RootObject
{
    public string range { get; set; }
    public string majorDimension { get; set; }

    public string[][] values { get; set; }
}
[Serializable]
public class ItemDetail
{
    public string Manufacturer { get; set; }
    public string LogoURL { get; set; }
    public Sprite Image { get; set; }


    public ItemDetail(string manufacturer, string logoURL, Sprite image)
    {
        Manufacturer = manufacturer;
        LogoURL = logoURL;
        Image = image;

    }
}
public class APIHandler : MonoBehaviour
{
    [SerializeField] private string baseURL = "https://sheets.googleapis.com/v4/spreadsheets/";
    [SerializeField] private string iD = "1pYU1mu9NBDYt3Ls_IYxMtbnaNrJ_t2jZxy7MYGFLjEA";
    [SerializeField] private string apiKey = "AIzaSyAA23WLN6TWfFj_J1VXvYPUOCIMSXGo254";

    [SerializeField] private string sheetName;
    //$"https://sheets.googleapis.com/v4/spreadsheets/{iD}/values/{sheetName}?key={apiKey}"

    [SerializeField] private List<ItemDetail> item = new List<ItemDetail>();
    private Sprite image;

    private void Awake()
    {
        onbutton = GetComponent<Button>();
    }

    private Button onbutton;
    private void Start()
    {

        sheetName = GetComponentInChildren<TextMeshProUGUI>().text;
    }
    private void OnEnable()
    {
        onbutton.onClick.AddListener(() => OnButtonClick(sheetName));

    }
    private void OnDisable()
    {
        onbutton.onClick.RemoveListener(() => OnButtonClick(sheetName));
    }
    private void OnButtonClick(string _sheetName)
    {
        Debug.Log(_sheetName);
        sheetName = "CAR";
        StartCoroutine(LoadData($"https://sheets.googleapis.com/v4/spreadsheets/{iD}/values/{sheetName}?key={apiKey}"));

    }

    private IEnumerator LoadData(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Offline");
        }
        else
        {
            string data = www.downloadHandler.text;

            RootObject jsondata = JsonConvert.DeserializeObject<RootObject>(data);


            Debug.Log(data);
            for (int i = 0; i < jsondata.values[0].Length; i++)
            {
                StartCoroutine(LoadImage(jsondata.values[1][i].ToString()));
                item.Add(new ItemDetail(jsondata.values[0][i], jsondata.values[1][i], image));
            }

            LevelManager levelManager = GetComponent<LevelManager>();
            levelManager.SwtichToGame();


        }

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

            image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);



        }
    }

    public List<ItemDetail> GetItemDetails()
    {
        return item;
    }



}


