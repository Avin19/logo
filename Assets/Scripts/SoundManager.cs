using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    private AudioSource audioSource;

    [SerializeField] private AudioClip[] audioClip;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void ButtonClick()
    {
        audioSource.PlayOneShot(audioClip[0]);
    }
    public void CorrectAnswer()
    {
        audioSource.PlayOneShot(audioClip[1]);
    }
    public void WrongAnswer()
    {
        audioSource.PlayOneShot(audioClip[2]);
    }


    public void SoundVolume(float volume)
    {
        audioSource.volume = volume;
    }
    public float SetSoundVolumen()
    {
        return audioSource.volume;
    }

}
