using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SongSlider : MonoBehaviour
{
    [Header("References")]
    public Slider slider;
    public AudioSource audioSource;

    [Header("Normal Game Settings")]
    [Tooltip("How many seconds the slider takes to reach the end in normal gameplay")]
    public float sliderDuration = 30f;

    [Header("Tutorial Settings")]
    public TypingManager typingManager;

    public bool IsRunning => isRunning;

[Header("End Sequence")]
public CanvasGroup overlayCanvasGroup;
public TMP_Text finishedText;
public AudioClip whistleClip;
public AudioSource sfxSource;
public float fadeDuration = 10f;
public AudioReactiveRotator audioReactiveRotator;
public Button resultsButton;        // Add this
public float punchScaleAmount = 1.4f; // Add this
public float punchSpeed = 12f;        // Add this

[Header("Stats")]
public StatsTracker statsTracker;

    private bool isTutorialMode = false;
    private float elapsed = 0f;
    private bool isRunning = false;
    private bool endSequenceStarted = false;

    void Start()
    {
        if (slider == null)
        {
            Debug.LogError("SongSlider: Assign the Slider in the Inspector!");
            return;
        }
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value    = 0f;
        isRunning = false;

        if (overlayCanvasGroup != null)
        {
            overlayCanvasGroup.alpha = 0f;
            overlayCanvasGroup.gameObject.SetActive(true);
        }

        if (finishedText != null)
            finishedText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isRunning) return;
        if (isTutorialMode)
            UpdateTutorialSlider();
        else
            UpdateNormalSlider();
    }

    void UpdateNormalSlider()
    {
        elapsed += Time.deltaTime;
        slider.value = Mathf.Clamp01(elapsed / sliderDuration);

        // Start fading overlay in the last fadeDuration seconds
        float timeRemaining = sliderDuration - elapsed;
        if (timeRemaining <= fadeDuration && !endSequenceStarted)
        {
            endSequenceStarted = true;
        }

        if (endSequenceStarted && overlayCanvasGroup != null)
        {
            float fadeProgress = 1f - (timeRemaining / fadeDuration);
            overlayCanvasGroup.alpha = Mathf.Clamp01(fadeProgress);
        }

        if (elapsed >= sliderDuration)
        {
            slider.value = 1f;
            isRunning    = false;
            StartCoroutine(OnSliderFinished());
        }
    }

void UpdateTutorialSlider()
{
    if (typingManager == null)
    {
        Debug.LogError("SongSlider: typingManager is null!");
        return;
    }
    float progress = (float)typingManager.CurrentIndex / typingManager.PhraseLength;
    slider.value   = Mathf.Clamp01(progress);
    if (typingManager.CurrentIndex >= typingManager.PhraseLength)
    {
        slider.value = 1f;
        isRunning    = false;
        // No end sequence for tutorial
    }
}

IEnumerator OnSliderFinished()
{
    // Save stats before anything else
    if (statsTracker != null)
    {
        SessionStats.WPM           = statsTracker.GetWPM();
        SessionStats.Accuracy      = statsTracker.GetAccuracy();
        SessionStats.TotalWords    = statsTracker.GetTotalWords();
        SessionStats.TotalErrors   = statsTracker.GetMistakes();
        SessionStats.HighestStreak = statsTracker.GetHighestStreak();
    }

    // Stop typing immediately
    if (typingManager != null)
        typingManager.StopTyping();

    if (overlayCanvasGroup != null)
        overlayCanvasGroup.alpha = 1f;

    if (audioSource != null)
        audioSource.Stop();

    if (audioReactiveRotator != null)
        audioReactiveRotator.StopReacting();

    if (sfxSource != null && whistleClip != null)
    {
        sfxSource.clip = whistleClip;
        sfxSource.Play();
    }

    if (resultsButton != null)
    {
        CanvasGroup btnGroup = resultsButton.GetComponent<CanvasGroup>();
        if (btnGroup == null)
            btnGroup = resultsButton.gameObject.AddComponent<CanvasGroup>();
        btnGroup.alpha = 0f;
    }

    if (finishedText != null)
    {
        finishedText.gameObject.SetActive(true);
        finishedText.rectTransform.localScale = Vector3.zero;

        float elapsed = 0f;
        float duration = 1f / punchSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = EaseOutBack(t);
            finishedText.rectTransform.localScale = Vector3.one * Mathf.Lerp(0f, punchScaleAmount, eased);
            yield return null;
        }

        elapsed = 0f;
        float settleDuration = 0.15f;
        while (elapsed < settleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / settleDuration);
            finishedText.rectTransform.localScale = Vector3.one * Mathf.Lerp(punchScaleAmount, 1f, t);
            yield return null;
        }

        finishedText.rectTransform.localScale = Vector3.one;
    }

    yield return new WaitForSeconds(1f);

    if (resultsButton != null)
    {
        CanvasGroup btnGroup = resultsButton.GetComponent<CanvasGroup>();
        float elapsed = 0f;
        float fadeDur = 0.5f;

        while (elapsed < fadeDur)
        {
            elapsed += Time.deltaTime;
            btnGroup.alpha = Mathf.Clamp01(elapsed / fadeDur);
            yield return null;
        }

        btnGroup.alpha = 1f;
    }
}

float EaseOutBack(float t)
{
    float c1 = 1.70158f;
    float c3 = c1 + 1f;
    return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
}

    public void StartSlider()
    {
        isTutorialMode     = !GameProgress.HasCompletedTutorial;
        endSequenceStarted = false;
        elapsed            = 0f;
        slider.value       = 0f;
        isRunning          = true;

        if (overlayCanvasGroup != null)
            overlayCanvasGroup.alpha = 0f;

        if (finishedText != null)
            finishedText.gameObject.SetActive(false);

        Debug.Log($"SongSlider: StartSlider called — isTutorialMode: {isTutorialMode}");
    }

    public void StopSlider()
    {
        isRunning = false;
    }

    public void ResetSlider()
    {
        elapsed            = 0f;
        slider.value       = 0f;
        isRunning          = false;
        endSequenceStarted = false;

        if (overlayCanvasGroup != null)
            overlayCanvasGroup.alpha = 0f;

        if (finishedText != null)
            finishedText.gameObject.SetActive(false);
    }
}