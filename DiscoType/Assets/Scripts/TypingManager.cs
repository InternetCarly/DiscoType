using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TypingManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI phraseText;

    [Header("Animation")]
    public keypressanimscript animScript;

    [Header("Settings")]
    public string phrase = "The quick brown fox jumps over the lazy dog.";

    [Header("Scrolling")]
    public RectTransform phraseTextRect;
    public float scrollPadding = 200f;

    [Header("Stats")]
    public StatsTracker statsTracker;

    // Called by TutorialManager when phrase is complete
    public Action onPhraseCompleted;

    private int currentIndex = 0;
    private bool isFinished = false;
    private bool isError = false;
    private bool acceptingInput = false;  // Locked until GO!

    // Add these anywhere in TypingManager.cs
public int CurrentIndex  => currentIndex;
public int PhraseLength  => phrase.Length;

    void Start()
    {
        UpdateDisplay();
    }

    void Update()
    {
        // Don't track any keypresses until gameplay is enabled
        if (isFinished || !acceptingInput) return;

        foreach (char c in Input.inputString)
        {
            HandleInput(c);
        }
    }

    // Called by TutorialManager after GO! to unlock typing
    public void StartTyping()
    {
        acceptingInput = true;
        isFinished = false;
        currentIndex = 0;
        isError = false;
        UpdateDisplay();
    }

    // Called by TutorialManager to re-lock input (e.g. on restart)
    public void StopTyping()
    {
        acceptingInput = false;
    }

    void HandleInput(char typedChar)
    {
        if (typedChar == '\b' || typedChar == '\n' || typedChar == '\r')
            return;

        char expected = phrase[currentIndex];

        if (typedChar == expected)
        {
            isError = false;
            currentIndex++;
            animScript.OnCorrectKey();
            statsTracker.OnKeypressCorrect(typedChar);

            if (currentIndex >= phrase.Length)
            {
                isFinished = true;
                acceptingInput = false;
                statsTracker.OnPhraseCompleted();
                FinishPhrase();
            }
            else
            {
                UpdateDisplay();
            }
        }
        else
        {
            isError = true;
            animScript.OnWrongKey();
            statsTracker.OnKeypressError();
            UpdateDisplay();
        }
    }

    void UpdateDisplay()
    {
        string typed     = phrase.Substring(0, currentIndex);
        string current   = phrase.Substring(currentIndex, 1);
        string remaining = phrase.Substring(currentIndex + 1);
        string markColor = isError ? "#FF444480" : "#4A90D9AA";

        phraseText.text =
            $"<color=#888888>{typed}</color>" +
            $"<mark={markColor}><color=#FFFFFF>{current}</color></mark>" +
            $"<color=#FFFFFF>{remaining}</color>";

        ScrollToCurrentIndex();
    }

    void FinishPhrase()
    {
        phraseText.text = $"<color=#888888>{phrase}</color>";
        Debug.Log("Phrase complete!");

        // Notify TutorialManager that gameplay is done
        onPhraseCompleted?.Invoke();
    }

    private void ScrollToCurrentIndex()
    {
        if (currentIndex >= phrase.Length) return;

        phraseText.ForceMeshUpdate();
        TMP_CharacterInfo charInfo = phraseText.textInfo.characterInfo[currentIndex];
        float charX = charInfo.origin;
        float targetX = Mathf.Min(0, -(charX - scrollPadding));
        phraseTextRect.anchoredPosition = new Vector2(targetX, phraseTextRect.anchoredPosition.y);
    }
}