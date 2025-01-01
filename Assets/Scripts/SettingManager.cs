using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sliderText;


    // Update is called once per frame
    private void OnEnable()
    {
        sfxSlider.onValueChanged.AddListener(OnValueChanged);
    }
    private void OnDisable()
    {
        sfxSlider.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void Start()
    {
        sfxSlider.value = SoundManager.Instance.SetSoundVolumen();
        sliderText.text = (SoundManager.Instance.SetSoundVolumen() * 10).ToString("0");

    }
    private void OnValueChanged(float arg0)
    {
        SoundManager.Instance.SoundVolume(arg0);
        sliderText.text = (arg0 * 10).ToString("0");

    }
}
