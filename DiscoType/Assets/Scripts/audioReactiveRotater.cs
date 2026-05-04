using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class AudioReactiveRotator : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The AudioSource playing your music track")]
    public AudioSource audioSource;

    [Header("Rotation Settings")]
    public float rotationSpeed = 90f;         // Degrees per second
    public bool clockwise = true;

    [Header("Audio Reactive Scale")]
    public float baseScale = 1f;              // Resting scale
    public float maxScaleBoost = 0.4f;        // Max additional scale at peak volume
    public float scaleSmoothSpeed = 8f;       // How snappy the scale reacts
    public float sensitivity = 10f;           // Amplifies the volume sample

    [Header("Sample Settings")]
    [Tooltip("FFT sample size — must be a power of 2")]
    public int sampleSize = 256;
    [Tooltip("Which audio channel to sample (0 = left, 1 = right)")]
    public int audioChannel = 0;

    private RectTransform rectTransform;
    private float[] samples;
    private float currentScale;
    private float targetScale;

    public void StopReacting()
{
    enabled = false;
}

void Awake()
{
    rectTransform = GetComponent<RectTransform>();
    samples       = new float[sampleSize];
    currentScale  = baseScale;
    targetScale   = baseScale;

    // If no AudioSource assigned, try to grab it from BackgroundMusicManager
    if (audioSource == null && BackgroundMusicManager.Instance != null)
    {
        audioSource = BackgroundMusicManager.Instance.audioSource;
        Debug.Log("AudioReactiveRotator: Using BackgroundMusicManager audio source.");
    }
}

    void Update()
    {
        Rotate();
        SampleAudio();
        ReactToAudio();
    }

    void Rotate()
    {
        float direction = clockwise ? -1f : 1f;
        rectTransform.Rotate(0f, 0f, direction * rotationSpeed * Time.deltaTime);
    }

    void SampleAudio()
    {
        if (audioSource == null || !audioSource.isPlaying)
        {
            targetScale = baseScale;
            return;
        }

        // Get frequency data from the audio source
        audioSource.GetSpectrumData(samples, audioChannel, FFTWindow.BlackmanHarris);

        // Average the lower frequency bands (bass/mids drive the pulse nicely)
        float sum = 0f;
        int bassBands = sampleSize / 8; // Focus on lower frequencies
        for (int i = 0; i < bassBands; i++)
            sum += samples[i];

        float average = sum / bassBands;
        float boosted = average * sensitivity;
        float clamped = Mathf.Clamp(boosted, 0f, 1f);

        targetScale = baseScale + (clamped * maxScaleBoost);
    }

    void ReactToAudio()
    {
        currentScale = Mathf.Lerp(currentScale, targetScale, Time.deltaTime * scaleSmoothSpeed);
        rectTransform.localScale = Vector3.one * currentScale;
    }
}