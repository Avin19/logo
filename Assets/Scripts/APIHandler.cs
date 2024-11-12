using UnityEngine;


// Here i will getting data from sheet 
// list of categories


public class APIHandler : MonoBehaviour
{
    [SerializeField] private string baseURL = "https://sheets.googleapis.com/v4/spreadsheets/";
    [SerializeField] private string iD = "1pYU1mu9NBDYt3Ls_IYxMtbnaNrJ_t2jZxy7MYGFLjEA";
    [SerializeField] private string apiKey = "AIzaSyAA23WLN6TWfFj_J1VXvYPUOCIMSXGo254";

    [SerializeField] private string sheetName;
    //$"https://sheets.googleapis.com/v4/spreadsheets/{iD}/values/{sheetName}?key={apiKey}"



}


