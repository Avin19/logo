using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using DG.Tweening;

public class BladeootstrapLoadingUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform logo;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI percentageText;

    [Header("Loading Setting")]
    [SerializeField] private float totalLoadingDuration = 5f;

    [Header("Logo Animation")]
    [SerializeField] private float logoStartScale = 0.8f;
    [SerializeField] private float logoTargetScale = 1f;
    [SerializeField] private float logoIntroDuration = 0.7f;

    [Header("Logo Idle Animation")]
    [SerializeField] private float logoBreathScale = 1.025f;
    [SerializeField] private float logoBreathDuration = 1.2f;

    [Header("Text Animation")]
    [SerializeField] private float textAnimationDuration = 0.2f;


    // =========================================================
    // UPDATE SYSTEM
    // =========================================================

    [Header("Update System")]

    [Tooltip("Raw GitHub URL of update.json")]
    [SerializeField]
    private string updateJsonURL;

    [SerializeField]
    private GameObject updatePanel;

    [SerializeField]
    private TextMeshProUGUI updateTitle;

    [SerializeField]
    private TextMeshProUGUI updateMessage;

    [SerializeField]
    private Button updateButton;

    [SerializeField]
    private Button laterButton;

    [SerializeField]
    private RectTransform updatePanelRect;

    [Header("Update Animation")]
    [SerializeField]
    private float updatePanelDuration = 0.4f;


    // =========================================================
    // PRIVATE
    // =========================================================

    private float currentProgress;

    private Coroutine loadingMessageCoroutine;

    private int lastDisplayedPercentage = -1;

    private string updateStoreURL;


    private string[] loadingMessages =
    {
        "Initializing Audio Systems",
        "Preparing Game Services",
        "Connecting Gameplay Modules",
        "Loading Player Profile",
        "Synchronizing Save Data",
        "Preparing User Interface",
        "Configuring Input Controls",
        "Optimizing Performance",
        "Initializing Physics Engine",
        "Preparing AI Systems",
        "Loading Game Assets",
        "Building Environment",
        "Preparing Visual Effects",
        "Loading Character Data",
        "Initializing Enemy Behaviors",
        "Preparing Mission Data",
        "Loading World State",
        "Generating Gameplay Systems",
        "Initializing Network Components",
        "Preparing Adventure",
        "Sharpening Blades",
        "Scanning Environment",
        "Charging Gameplay Systems"
    };


    // =========================================================
    // UPDATE DATA
    // =========================================================

    [Serializable]
    private class UpdateData
    {
        public string latestVersion;
        public string minimumVersion;
        public bool forceUpdate;
        public string updateUrl;
        public string title;
        public string message;
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        SetupUI();

        PlayLogoIntro();

        loadingMessageCoroutine =
            StartCoroutine(ChangeLoadingMessages());

        StartCoroutine(InitializeGame());

        if (updateButton != null)
            updateButton.onClick.AddListener(OpenStore);

        if (laterButton != null)
            laterButton.onClick.AddListener(ContinueWithoutUpdate);
    }


    // =========================================================
    // SETUP
    // =========================================================

    private void SetupUI()
    {
        currentProgress = 0f;

        loadingSlider.value = 0f;

        lastDisplayedPercentage = -1;

        if (logo != null)
        {
            logo.localScale =
                Vector3.one * logoStartScale;
        }

        if (updatePanel != null)
            updatePanel.SetActive(false);

        loadingText.text =
            "Preparing Adventure...";

        percentageText.text =
            "0%";
    }


    // =========================================================
    // LOGO ANIMATION
    // =========================================================

    private void PlayLogoIntro()
    {
        if (logo == null)
            return;

        logo.DOKill();

        Sequence sequence =
            DOTween.Sequence();

        sequence.Append(
            logo.DOScale(
                logoTargetScale,
                logoIntroDuration
            )
            .SetEase(Ease.OutBack)
        );

        sequence.AppendInterval(0.15f);

        sequence.Append(
            logo.DOScale(
                logoBreathScale,
                logoBreathDuration
            )
            .SetEase(Ease.InOutSine)
        );

        sequence.Append(
            logo.DOScale(
                logoTargetScale,
                logoBreathDuration
            )
            .SetEase(Ease.InOutSine)
        );

        sequence.SetLoops(
            -1,
            LoopType.Restart
        );
    }


    // =========================================================
    // LOADING
    // =========================================================

    private IEnumerator InitializeGame()
    {
        float timer = 0f;

        while (timer < totalLoadingDuration)
        {
            timer += Time.deltaTime;

            currentProgress =
                Mathf.Clamp01(
                    timer / totalLoadingDuration
                );

            UpdateUI();

            yield return null;
        }


        currentProgress = 1f;

        UpdateUI();


        if (loadingMessageCoroutine != null)
        {
            StopCoroutine(
                loadingMessageCoroutine
            );

            loadingMessageCoroutine = null;
        }


        AnimateLoadingText(
            "Loading Complete"
        );


        yield return new WaitForSeconds(0.5f);


        PlayCompletionAnimation();


        yield return new WaitForSeconds(0.4f);


        // =====================================================
        // CHECK UPDATE
        // =====================================================

        yield return StartCoroutine(
            CheckForUpdate()
        );
    }


    // =========================================================
    // UPDATE CHECK
    // =========================================================

    private IEnumerator CheckForUpdate()
    {
        if (string.IsNullOrEmpty(updateJsonURL))
        {
            Debug.LogWarning(
                "Update JSON URL is empty."
            );

            LoadGame();

            yield break;
        }


        AnimateLoadingText(
            "Checking for Updates..."
        );


        using (UnityWebRequest request =
            UnityWebRequest.Get(updateJsonURL))
        {
            request.timeout = 5;

            yield return request.SendWebRequest();


            if (request.result !=
                UnityWebRequest.Result.Success)
            {
                Debug.LogWarning(
                    "Update check failed: " +
                    request.error
                );

                // Don't block the player
                LoadGame();

                yield break;
            }


            string json =
                request.downloadHandler.text;


            UpdateData updateData = null;


            try
            {
                updateData =
                    JsonUtility.FromJson<UpdateData>(
                        json
                    );
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "Invalid update JSON: " +
                    exception.Message
                );

                LoadGame();

                yield break;
            }


            if (updateData == null)
            {
                LoadGame();

                yield break;
            }


            Debug.Log(
                "Current Version: " +
                Application.version
            );

            Debug.Log(
                "Latest Version: " +
                updateData.latestVersion
            );


            bool updateAvailable =
                IsNewerVersion(
                    updateData.latestVersion,
                    Application.version
                );


            bool forceUpdate = false;


            if (!string.IsNullOrEmpty(
                updateData.minimumVersion))
            {
                forceUpdate =
                    IsNewerVersion(
                        updateData.minimumVersion,
                        Application.version
                    );
            }


            // JSON forceUpdate can also force it
            if (updateData.forceUpdate)
                forceUpdate = true;


            if (updateAvailable || forceUpdate)
            {
                updateStoreURL =
                    updateData.updateUrl;


                ShowUpdatePanel(
                    updateData,
                    forceUpdate
                );


                yield break;
            }


            // No update
            LoadGame();
        }
    }


    // =========================================================
    // VERSION COMPARISON
    // =========================================================

    private bool IsNewerVersion(
        string serverVersion,
        string currentVersion)
    {
        if (string.IsNullOrEmpty(
            serverVersion))
        {
            return false;
        }


        if (string.IsNullOrEmpty(
            currentVersion))
        {
            return false;
        }


        try
        {
            Version server =
                new Version(serverVersion);

            Version current =
                new Version(currentVersion);


            return server > current;
        }
        catch
        {
            Debug.LogWarning(
                "Invalid version format. " +
                "Server: " +
                serverVersion +
                " Current: " +
                currentVersion
            );

            return false;
        }
    }


    // =========================================================
    // SHOW UPDATE PANEL
    // =========================================================

    private void ShowUpdatePanel(
        UpdateData updateData,
        bool forceUpdate)
    {
        if (updatePanel == null)
        {
            LoadGame();
            return;
        }


        if (updateTitle != null)
        {
            updateTitle.text =
                string.IsNullOrEmpty(
                    updateData.title)
                    ? "NEW UPDATE AVAILABLE!"
                    : updateData.title;
        }


        if (updateMessage != null)
        {
            updateMessage.text =
                string.IsNullOrEmpty(
                    updateData.message)
                    ? "A new version is available."
                    : updateData.message;
        }


        updatePanel.SetActive(true);


        // Mandatory update
        if (laterButton != null)
        {
            laterButton.gameObject.SetActive(
                !forceUpdate
            );
        }


        if (updatePanelRect != null)
        {
            updatePanelRect.localScale =
                Vector3.zero;

            updatePanelRect.DOKill();

            updatePanelRect
                .DOScale(
                    Vector3.one,
                    updatePanelDuration
                )
                .SetEase(Ease.OutBack);
        }
    }


    // =========================================================
    // UPDATE BUTTON
    // =========================================================

    private void OpenStore()
    {
        if (string.IsNullOrEmpty(
            updateStoreURL))
        {
            Debug.LogWarning(
                "Update store URL is empty."
            );

            return;
        }


        Application.OpenURL(
            updateStoreURL
        );
    }


    // =========================================================
    // LATER BUTTON
    // =========================================================

    private void ContinueWithoutUpdate()
    {
        if (updatePanelRect != null)
        {
            updatePanelRect.DOKill();

            updatePanelRect
                .DOScale(
                    Vector3.zero,
                    0.25f
                )
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    updatePanel.SetActive(false);

                    LoadGame();
                });
        }
        else
        {
            updatePanel.SetActive(false);

            LoadGame();
        }
    }


    // =========================================================
    // LOAD GAME
    // =========================================================

    private void LoadGame()
    {
        SceneManager.LoadScene(1);
    }


    // =========================================================
    // PROGRESS UI
    // =========================================================

    private void UpdateUI()
    {
        loadingSlider.value =
            Mathf.Lerp(
                loadingSlider.value,
                currentProgress,
                Time.deltaTime * 8f
            );


        int percent =
            Mathf.RoundToInt(
                currentProgress * 100f
            );


        percentageText.text =
            percent + "%";


        if (percent !=
            lastDisplayedPercentage)
        {
            lastDisplayedPercentage =
                percent;

            AnimatePercentage();
        }
    }


    private void AnimatePercentage()
    {
        percentageText.transform.DOKill();

        percentageText.transform.localScale =
            Vector3.one;


        percentageText.transform
            .DOPunchScale(
                Vector3.one * 0.08f,
                0.15f,
                3,
                0.5f
            );
    }


    // =========================================================
    // LOADING MESSAGES
    // =========================================================

    private IEnumerator ChangeLoadingMessages()
    {
        while (currentProgress < 1f)
        {
            string message =
                loadingMessages[
                    UnityEngine.Random.Range(
                        0,
                        loadingMessages.Length
                    )
                ];


            yield return StartCoroutine(
                AnimateDots(message)
            );


            yield return new WaitForSeconds(
                0.2f
            );
        }
    }


    private IEnumerator AnimateDots(
        string baseMessage)
    {
        float duration = 1.2f;

        float timer = 0f;


        while (timer < duration)
        {
            timer += Time.deltaTime;


            float t =
                timer % 0.9f;


            string dots;


            if (t < 0.3f)
                dots = ".";

            else if (t < 0.6f)
                dots = "..";

            else
                dots = "...";


            loadingText.text =
                baseMessage + dots;


            yield return null;
        }
    }


    // =========================================================
    // TEXT ANIMATION
    // =========================================================

    private void AnimateLoadingText(
        string message)
    {
        loadingText.transform.DOKill();


        Sequence sequence =
            DOTween.Sequence();


        sequence.Append(
            loadingText.transform
                .DOScale(
                    0.9f,
                    textAnimationDuration
                )
                .SetEase(Ease.InQuad)
        );


        sequence.AppendCallback(() =>
        {
            loadingText.text =
                message;
        });


        sequence.Append(
            loadingText.transform
                .DOScale(
                    1f,
                    textAnimationDuration
                )
                .SetEase(Ease.OutBack)
        );
    }


    // =========================================================
    // COMPLETION
    // =========================================================

    private void PlayCompletionAnimation()
    {
        if (logo == null)
            return;


        logo.DOKill();


        Sequence sequence =
            DOTween.Sequence();


        sequence.Append(
            logo.DOScale(
                1.08f,
                0.2f
            )
            .SetEase(Ease.OutQuad)
        );


        sequence.Append(
            logo.DOScale(
                1f,
                0.3f
            )
            .SetEase(Ease.OutBack)
        );
    }


    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        if (loadingMessageCoroutine != null)
        {
            StopCoroutine(
                loadingMessageCoroutine
            );
        }


        if (updateButton != null)
            updateButton.onClick.RemoveListener(
                OpenStore
            );


        if (laterButton != null)
            laterButton.onClick.RemoveListener(
                ContinueWithoutUpdate
            );


        if (logo != null)
            logo.DOKill();


        if (loadingText != null)
            loadingText.transform.DOKill();


        if (percentageText != null)
            percentageText.transform.DOKill();


        if (updatePanelRect != null)
            updatePanelRect.DOKill();


        DOTween.Kill(this);
    }
}