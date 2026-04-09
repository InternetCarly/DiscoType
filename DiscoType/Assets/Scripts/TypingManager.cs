using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private int currentIndex = 0;
    private bool isFinished = false;
    private bool isError = false;

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
        // These two lines were missing entirely
        if (typedChar == '\b' || typedChar == '\n' || typedChar == '\r')
            return;

        char expected = phrase[currentIndex]; // This was missing too

        if (typedChar == expected)
        {
            isError = false;
            currentIndex++;
            animScript.OnCorrectKey();
            statsTracker.OnKeypressCorrect();

            if (currentIndex >= phrase.Length)
            {
                isFinished = true;
                statsTracker.OnPhraseCompleted();
                OnPhraseCompleted();
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

    void OnPhraseCompleted()
    {
        phraseText.text = $"<color=#888888>{phrase}</color>";
        Debug.Log("Phrase complete!");
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