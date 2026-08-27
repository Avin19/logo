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
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogWarning("SoundManager: No AudioSource component found on " + gameObject.name);
        }
    }

    private void PlayClip(int index)
    {
        if (audioSource == null)
            return;

        if (audioClip == null || index < 0 || index >= audioClip.Length || audioClip[index] == null)
        {
            Debug.LogWarning("SoundManager: Audio clip at index " + index + " is missing.");
            return;
        }

        audioSource.PlayOneShot(audioClip[index]);
    }

    public void ButtonClick()
    {
        PlayClip(0);
    }
    public void CorrectAnswer()
    {
        PlayClip(1);
    }
    public void WrongAnswer()
    {
        PlayClip(2);
    }


    public void SoundVolume(float volume)
    {
        if (audioSource != null)
            audioSource.volume = volume;
    }
    public float SetSoundVolumen()
    {
        return audioSource != null ? audioSource.volume : 0f;
    }

    public void SetMuted(bool muted)
    {
        if (audioSource != null)
            audioSource.mute = muted;
    }

}
