using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color onButton;
    [SerializeField] private Color offButton;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI sfxText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI hapticText;
    [SerializeField] private TextMeshProUGUI playerId;

    [Header("Buttons")]
    [SerializeField] private Button sfxBtn;
    [SerializeField] private Button musicBtn;
    [SerializeField] private Button hapticBtn;

    private bool sfx;
    private bool music;
    private bool haptic;
    [SerializeField] private AudioSource musicManager;

    private void OnEnable()
    {
        sfxBtn.onClick.AddListener(SFXChange);
        musicBtn.onClick.AddListener(MusicChange);
        hapticBtn.onClick.AddListener(HapticChange);
    }

    private void OnDisable()
    {
        sfxBtn.onClick.RemoveListener(SFXChange);
        musicBtn.onClick.RemoveListener(MusicChange);
        hapticBtn.onClick.RemoveListener(HapticChange);
    }

    private void Start()
    {
        LoadSettings();

        RefreshUI();
    }

    void LoadSettings()
    {
        sfx = PlayerDataManager.Instance.SFX;
        music = PlayerDataManager.Instance.Music;
        haptic = PlayerDataManager.Instance.Haptic;
        playerId.text = "YourID :" + PlayerDataManager.Instance.PlayerId;
    }

    #region BUTTON EVENTS

    private void SFXChange()
    {
        SoundManager.Instance.ButtonClick();
        sfx = !sfx;

        PlayerDataManager.Instance.data.SoundEnabled = sfx;
        SoundManager.Instance.gameObject.GetComponent<AudioSource>().mute = !sfx;
        ApplySFX();

        Save();
    }

    private void MusicChange()
    {
        SoundManager.Instance.ButtonClick();
        music = !music;

        PlayerDataManager.Instance.data.MusicEnabled = music;
        musicManager.mute = !music;
        ApplyMusic();

        Save();
    }

    private void HapticChange()
    {
        haptic = !haptic;

        PlayerDataManager.Instance.data.Haptic = haptic;

        HapticManager.Instance.SetHaptics(haptic);

        ApplyHaptic();

        Save();
    }

    #endregion

    #region APPLY SETTINGS

    void ApplySFX()
    {
        UpdateButtonUI(sfxBtn, sfxText, sfx);
    }

    void ApplyMusic()
    {
        UpdateButtonUI(musicBtn, musicText, music);

        // Example:

    }

    void ApplyHaptic()
    {
        UpdateButtonUI(hapticBtn, hapticText, haptic);
    }

    #endregion

    #region UI

    void RefreshUI()
    {
        ApplySFX();
        ApplyMusic();
        ApplyHaptic();
    }

    void UpdateButtonUI(Button button,
                        TextMeshProUGUI text,
                        bool isEnabled)
    {
        button.image.color =
            isEnabled ? onButton : offButton;

        text.text =
            isEnabled ? "ON" : "OFF";
    }

    #endregion

    void Save()
    {
        PlayerDataManager.Instance.Save();
    }
}