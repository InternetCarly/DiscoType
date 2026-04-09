using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsTracker : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI wpmText;
    public TextMeshProUGUI accuracyText;

    private int totalKeystrokes = 0;   // Every key pressed
    private int errorCount = 0;        // Only wrong keypresses
    private float startTime = -1f;     // Set on first keypress
    private bool hasStarted = false;
    private bool isStopped = false;

    // Called by TypingManager on every keypress (correct or not)
    public void OnKeypressCorrect()
    {
        StartTimerIfNeeded();
        totalKeystrokes++;
        UpdateDisplay();
    }

    public void OnKeypressError()
    {
        StartTimerIfNeeded();
        totalKeystrokes++;
        errorCount++;
        UpdateDisplay();
    }

    public void OnPhraseCompleted()
    {
        isStopped = true;
        UpdateDisplay(); // Final update
    }

    void StartTimerIfNeeded()
    {
        if (!hasStarted)
        {
            startTime = Time.time;
            hasStarted = true;
        }
    }

    void Update()
    {
        // Continuously update WPM while typing is in progress
        if (hasStarted && !isStopped)
            UpdateDisplay();
    }

    void UpdateDisplay()
    {
        wpmText.text = $"WPM: {CalculateWPM()}";
        accuracyText.text = $"Accuracy: {CalculateAccuracy()}%";
    }

    int CalculateWPM()
    {
        if (!hasStarted) return 0;

        float elapsedMinutes = (Time.time - startTime) / 60f;
        if (elapsedMinutes <= 0) return 0;

        // Standard WPM formula: every 5 characters = 1 word
        // Use correct keystrokes only (total - errors)
        int correctKeystrokes = totalKeystrokes - errorCount;
        float words = correctKeystrokes / 5f;

        return Mathf.FloorToInt(words / elapsedMinutes);
    }

    int CalculateAccuracy()
    {
        if (totalKeystrokes == 0) return 100; // No keypresses yet, show 100%

        // Accuracy = correct presses / total presses
        int correctKeystrokes = totalKeystrokes - errorCount;
        float accuracy = (float)correctKeystrokes / totalKeystrokes * 100f;

        return Mathf.FloorToInt(accuracy);
    }
}