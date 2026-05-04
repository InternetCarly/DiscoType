using UnityEngine;
using TMPro;

public class ResultsManager : MonoBehaviour
{
    [Header("References")]
    public keypressanimscript animScript;
    public TMP_Text resultsText;

    [Header("Animation Settings")]
    public float frameInterval = 0.1f;

    void Start()
    {
        if (animScript != null)
            animScript.StartLoopAnimation(frameInterval);
        else
            Debug.LogError("ResultsManager: animScript is not assigned!");

        if (resultsText != null)
        {
            resultsText.text =
                $"Accuracy: {SessionStats.Accuracy}%\n" +
                $"Words per Minute: {SessionStats.WPM}\n" +
                $"Total Words Typed: {SessionStats.TotalWords}\n" +
                $"Total Errors Made: {SessionStats.TotalErrors}\n" +
                $"Longest Streak: {SessionStats.HighestStreak} words";
        }
    }
}