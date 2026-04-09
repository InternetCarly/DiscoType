using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypingManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI phraseText;

    [Header("Settings")]
    public string phrase = "The quick brown fox jumps over the lazy dog.";

    private int currentIndex = 0;
    private bool isFinished = false;
    private bool isError = false;  // Tracks if the user made a mistake

    void Start()
    {
        UpdateDisplay();
    }

    void Update()
    {
        if (isFinished) return;

        foreach (char c in Input.inputString)
        {
            HandleInput(c);
        }
    }

    void HandleInput(char typedChar)
    {
        if (typedChar == '\b' || typedChar == '\n' || typedChar == '\r')
            return;

        char expected = phrase[currentIndex];

        if (typedChar == expected)
        {
            isError = false;  // Clear the error state on correct key
            currentIndex++;

            if (currentIndex >= phrase.Length)
            {
                isFinished = true;
                OnPhraseCompleted();
            }
            else
            {
                UpdateDisplay();
            }
        }
        else
        {
            isError = true;  // Set error state, stays red until correct key
            UpdateDisplay();
        }
    }

    void UpdateDisplay()
    {
        string typed     = phrase.Substring(0, currentIndex);
        string current   = phrase.Substring(currentIndex, 1);
        string remaining = phrase.Substring(currentIndex + 1);

        // Switch highlight color based on error state
        string markColor = isError ? "#FF444480" : "#4A90D9AA";

        phraseText.text =
            $"<color=#888888>{typed}</color>" +
            $"<mark={markColor}><color=#FFFFFF>{current}</color></mark>" +
            $"<color=#FFFFFF>{remaining}</color>";
    }

    void OnPhraseCompleted()
    {
        phraseText.text = $"<color=#888888>{phrase}</color>";
        Debug.Log("Phrase complete!");
    }
}