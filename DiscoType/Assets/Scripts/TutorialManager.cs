using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Popups")]
    public List<GameObject> popups = new List<GameObject>();

    [Header("Overlay")]
    public GameObject overlayPanel;

    [Header("Gameplay")]
    public int gameplayTriggerIndex = 2;
    public GameObject gameplayPanel;
    public TypingManager typingManager;

    [Header("Countdown")]
    public CountdownTimer countdownTimer;

    [Header("Slider")]
    public SongSlider songSlider;

    [Header("Popup Animation")]
    public float popupPunchScale = 1.08f;
    public float popupPunchSpeed = 10f;
    public float popupSettleSpeed = 6f;
    public float popupStartScale = 0.75f;

    [Header("Final Panel")]
    public GameObject finalPanel;
    public Button restartButton;
    public Button advanceButton;
    public string nextSceneName = "NextSceneHere";

    private int currentIndex = 0;
    private bool waitingForClick = true;
    private bool tutorialComplete = false;
    private bool phraseCompleted = false;

    void Start()
    {

            if (BackgroundMusicManager.Instance != null)
        BackgroundMusicManager.Instance.SetVolume(0.75f); // Adjust to taste

        HideAll();

        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);

        if (typingManager != null)
            typingManager.onPhraseCompleted = OnPhraseCompleted;

        restartButton.onClick.AddListener(RestartTutorial);
        advanceButton.onClick.AddListener(AdvanceToScene);
        ShowPopup(0);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && waitingForClick && !tutorialComplete)
            Advance();
    }

    void OnPhraseCompleted()
    {
        phraseCompleted = true;
    }

    void Advance()
    {
        if (currentIndex < popups.Count)
            popups[currentIndex].SetActive(false);

        if (currentIndex == gameplayTriggerIndex)
        {
            StartCoroutine(RunGameplaySequence());
            return;
        }

        currentIndex++;

        if (currentIndex >= popups.Count)
        {
            ShowFinalPanel();
            return;
        }

        ShowPopup(currentIndex);
    }

    void ShowPopup(int index)
    {
        HideAll();
        popups[index].SetActive(true);

        if (overlayPanel != null)
            overlayPanel.SetActive(true);

        waitingForClick = true;
        StartCoroutine(PopupAnimation(popups[index].GetComponent<RectTransform>()));
    }

    void ShowFinalPanel()
    {
        HideAll();
        tutorialComplete = true;
        waitingForClick = false;
        finalPanel.SetActive(true);

        if (overlayPanel != null)
            overlayPanel.SetActive(true);

        StartCoroutine(PopupAnimation(finalPanel.GetComponent<RectTransform>()));
    }

    void HideAll()
    {
        foreach (var popup in popups)
            if (popup != null)
                popup.SetActive(false);

        if (finalPanel != null) finalPanel.SetActive(false);

        if (overlayPanel != null)
            overlayPanel.SetActive(false);
    }

    IEnumerator RunGameplaySequence()
    {
        waitingForClick = false;
        phraseCompleted = false;

        if (overlayPanel != null)
            overlayPanel.SetActive(true);

        bool countdownDone = false;

        // Clear first to prevent stacking on repeat runs
        countdownTimer.onGo = null;
        countdownTimer.onComplete = null;

        countdownTimer.onGo += () =>
        {
            if (overlayPanel != null)
                overlayPanel.SetActive(false);

            // Start slider directly — no subscription timing issues
            songSlider?.StartSlider();
        };

        countdownTimer.onComplete += () =>
        {
            if (typingManager != null)
                typingManager.StartTyping();

            countdownDone = true;
        };

        countdownTimer.StartCountdown();

        yield return new WaitUntil(() => countdownDone);
        yield return new WaitUntil(() => phraseCompleted);

        currentIndex++;

        if (currentIndex >= popups.Count)
            ShowFinalPanel();
        else
            ShowPopup(currentIndex);
    }

    IEnumerator PopupAnimation(RectTransform target)
    {
        if (target == null) yield break;

        target.localScale = Vector3.one * popupStartScale;

        float elapsed = 0f;
        float punchInDuration = 1f / popupPunchSpeed;

        while (elapsed < punchInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / punchInDuration);
            float eased = EaseOutBack(t);
            target.localScale = Vector3.one * Mathf.Lerp(popupStartScale, popupPunchScale, eased);
            yield return null;
        }

        elapsed = 0f;
        float settleDuration = 1f / popupSettleSpeed;

        while (elapsed < settleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / settleDuration);
            float eased = EaseOutQuad(t);
            target.localScale = Vector3.one * Mathf.Lerp(popupPunchScale, 1f, eased);
            yield return null;
        }

        target.localScale = Vector3.one;
    }

    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    float EaseOutQuad(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    void RestartTutorial()
    {
        GameProgress.HasCompletedTutorial = false;
        countdownTimer.onGo = null;
        countdownTimer.onComplete = null;

        currentIndex = 0;
        tutorialComplete = false;
        phraseCompleted = false;

        if (typingManager != null)
            typingManager.StopTyping();

        songSlider?.ResetSlider();

        HideAll();

        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);

        ShowPopup(0);
    }

    void AdvanceToScene()
    {
        GameProgress.HasCompletedTutorial = true;
        SceneManager.LoadScene(nextSceneName);
    }
}