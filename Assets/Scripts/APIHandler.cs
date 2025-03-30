using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;

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



    public ItemDetail(string manufacturer, string logoURL)
    {
        Manufacturer = manufacturer;
        LogoURL = logoURL;


    }
}
public class APIHandler : MonoBehaviour
{

    [SerializeField] private string sheetName;

    [SerializeField] private List<ItemDetail> item = new List<ItemDetail>();
    private LevelManager levelManager;

    private string iD;
    private string apiKey;

    private void Awake()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "config.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            APIConfig config = JsonUtility.FromJson<APIConfig>(json);
            iD = config.SHEET_ID;
            apiKey = config.GOOGLE_API_KEY;
        }
        else
        {
            Debug.LogError("Config file not found!");
        }
        onbutton = GetComponent<Button>();
        levelManager = GetComponentInParent<LevelManager>();
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
        SoundManager.Instance.ButtonClick();
        levelManager.Name = _sheetName;
        StartCoroutine(LoadData($"https://sheets.googleapis.com/v4/spreadsheets/{iD}/values/{_sheetName}?key={apiKey}"));
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
            if (item.Count == jsondata.values[0].Length) { }
            else
            {

                for (int i = 0; i < jsondata.values[0].Length; i++)
                {

                    item.Add(new ItemDetail(jsondata.values[0][i], jsondata.values[1][i]));
                }
            }

        }


        levelManager.SetItemdetails(item);


    }

}


[System.Serializable]
public class APIConfig
{
    public string SHEET_ID;
    public string GOOGLE_API_KEY;
}





