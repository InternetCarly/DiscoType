using UnityEngine;

public class StatsTracker : MonoBehaviour
{
    public CalloutManager calloutManager; // Add this

    private int totalKeystrokes = 0;
    private int errorCount = 0;
    private float startTime = -1f;
    private bool hasStarted = false;
    private int totalWords = 0;
    private int currentStreak = 0;
    private int highestStreak = 0;
    private bool lastWasCorrect = true;

    public void OnKeypressCorrect(char typedChar)
    {
        StartTimerIfNeeded();
        totalKeystrokes++;
        lastWasCorrect = true;

        if (typedChar == ' ')
        {
            totalWords++;
            currentStreak++;
            if (currentStreak > highestStreak)
                highestStreak = currentStreak;

            if (calloutManager != null)
                calloutManager.OnWordCompleted();
        }
    }

    public void OnKeypressError()
    {
        StartTimerIfNeeded();
        totalKeystrokes++;
        errorCount++;

        if (lastWasCorrect)
        {
            currentStreak = 0;
            lastWasCorrect = false;
        }
    }

    public void OnPhraseCompleted()
    {
        totalWords++;
        currentStreak++;
        if (currentStreak > highestStreak)
            highestStreak = currentStreak;
    }

    void StartTimerIfNeeded()
    {
        if (!hasStarted)
        {
            startTime = Time.time;
            hasStarted = true;
        }
    }

    public int GetWPM()
    {
        if (!hasStarted) return 0;
        float elapsedMinutes = (Time.time - startTime) / 60f;
        if (elapsedMinutes <= 0) return 0;
        int correctKeystrokes = totalKeystrokes - errorCount;
        float words = correctKeystrokes / 5f;
        return Mathf.FloorToInt(words / elapsedMinutes);
    }

    public int GetAccuracy()
    {
        if (totalKeystrokes == 0) return 100;
        int correctKeystrokes = totalKeystrokes - errorCount;
        float accuracy = (float)correctKeystrokes / totalKeystrokes * 100f;
        return Mathf.FloorToInt(accuracy);
    }

    public int GetTotalWords()    => totalWords;
    public int GetHighestStreak() => highestStreak;
    public int GetMistakes()      => errorCount;
}