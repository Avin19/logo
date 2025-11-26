using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class APIHandler : MonoBehaviour
{
    [Header("Sheet / Local JSON")]

    [SerializeField] private string sheetName;

    [Header("Output")]
    [SerializeField] private List<ItemDetail> item = new List<ItemDetail>();

    private LevelManager levelManager;
    private Button onbutton;


    private UnityEngine.Events.UnityAction clickAction;


    [SerializeField] private int streamingAssetsTimeout = 10;

    private void Awake()
    {
        onbutton = GetComponent<Button>();
        levelManager = GetComponentInParent<LevelManager>();
    }

    private void Start()
    {
        // If sheetName is empty, attempt to read from child TMP text
        if (string.IsNullOrEmpty(sheetName))
        {
            TextMeshProUGUI tmp = GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) sheetName = tmp.text;
        }

    }

    private void OnEnable()
    {
        if (onbutton == null)
        {
            Debug.LogError("[APIHandler] Button component not found on GameObject.");
            return;
        }

        onbutton.onClick.AddListener(clickAction);
        clickAction = () => OnButtonClick(sheetName);
    }

    private void OnDisable()
    {
        if (onbutton != null && clickAction != null)
        {
            onbutton.onClick.RemoveListener(clickAction);
        }
    }

    private void OnButtonClick(string _sheetName)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.ButtonClick();


        // levelManager.StartCoroutine(LoadLocalOnly(_sheetName));
    }
}