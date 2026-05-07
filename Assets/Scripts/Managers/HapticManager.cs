using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance;

    private bool hapticsEnabled = true;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);


    }
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        hapticsEnabled =
            PlayerDataManager.Instance.Haptic;
    }

    public void SetHaptics(bool enabled)
    {
        hapticsEnabled = enabled;
    }

    #region HAPTICS

    public void LightImpact()
    {
        if (!CanVibrate())
            return;

        Handheld.Vibrate();
    }

    public void MediumImpact()
    {
        if (!CanVibrate())
            return;

        Handheld.Vibrate();
    }

    public void HeavyImpact()
    {
        if (!CanVibrate())
            return;

        Handheld.Vibrate();
    }

    public void Success()
    {
        if (!CanVibrate())
            return;

        Handheld.Vibrate();
    }

    public void Failure()
    {
        if (!CanVibrate())
            return;

        Handheld.Vibrate();
    }

    #endregion

    bool CanVibrate()
    {
#if UNITY_EDITOR
        return false;
#else
        return hapticsEnabled;
#endif
    }
}