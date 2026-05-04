using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Character Images")]
    public RectTransform character0;
    public RectTransform character1;
    public RectTransform character2;

    [Header("Character Unlock Images")]
    [Tooltip("Locked silhouette images for each character")]
    public Sprite[] lockedSprites;
    [Tooltip("Unlocked images for each character")]
    public Sprite[] unlockedSprites;

    [Header("Character Data")]
    [Tooltip("Names shown in the text box when each character is centered")]
    public string[] characterNames = { "Character 1", "Character 2", "Character 3" };
    [Tooltip("Which characters are unlocked — check the box to unlock")]
    public bool[] characterUnlocked = { true, false, false };

    [Header("UI")]
    public TMP_Text characterNameText;
    public string lockedMessage = "This character has not been unlocked yet.";
    public float unlockedFontSize = 48f;
    public float lockedFontSize = 28f;

    [Header("Scene Navigation")]
    public string tutorialSceneName = "GameTutorialScene";
    public string songSelectSceneName = "SongSelectScene";

    [Header("Podium Positions")]
    public Vector2 leftPosition   = new Vector2(-300f, 0f);
    public Vector2 centerPosition = new Vector2(0f,    0f);
    public Vector2 rightPosition  = new Vector2(300f,  0f);

    [Header("Podium Scale Settings")]
    public float frontScale    = 1.2f;
    public float backScale     = 0.85f;
    public float animationSpeed = 8f;

    [Header("Podium Color Settings")]
    public Color frontColor  = new Color(1f,   1f,   1f,   1f);
    public Color backColor   = new Color(0.6f, 0.6f, 0.6f, 1f);
    public Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    [Header("Arrows")]
    public Button leftArrow;
    public Button rightArrow;

    [Header("Select Button")]
    public Button letsGoButton;
    public string nextSceneName = "NextSceneHere";

    [Tooltip("Color of the button when a locked character is centered")]
    public Color buttonLockedColor   = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color buttonUnlockedColor = new Color(1f,   1f,   1f,   1f);

    private int[] slotAssignments = new int[] { 0, 1, 2 };
    private RectTransform[] characters;
    private Vector2[] slotPositions;
    private Vector3[] targetScales;
    private Color[] targetColors;

    void Start()
    {
        // Unlock character 2 if tutorial is completed
        if (GameProgress.HasCompletedTutorial)
            characterUnlocked[2] = true;

        characters    = new RectTransform[] { character0, character1, character2 };
        slotPositions = new Vector2[] { leftPosition, centerPosition, rightPosition };
        targetScales  = new Vector3[3];
        targetColors  = new Color[3];

        leftArrow.onClick.AddListener(ShiftLeft);
        rightArrow.onClick.AddListener(ShiftRight);
        letsGoButton.onClick.AddListener(OnLetsGo);

        UpdateCharacterSprites();
        ApplySlots(instant: true);
        UpdateUI();
    }

    void Update()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].anchoredPosition = Vector2.Lerp(
                characters[i].anchoredPosition,
                slotPositions[GetSlotOfCharacter(i)],
                Time.deltaTime * animationSpeed
            );

            characters[i].localScale = Vector3.Lerp(
                characters[i].localScale,
                targetScales[i],
                Time.deltaTime * animationSpeed
            );

            Image img = characters[i].GetComponent<Image>();
            if (img != null)
                img.color = Color.Lerp(img.color, targetColors[i], Time.deltaTime * animationSpeed);
        }
    }

    void UpdateCharacterSprites()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            Image img = characters[i].GetComponent<Image>();
            if (img == null) continue;

            bool unlocked = characterUnlocked[i];

            if (unlocked && unlockedSprites != null && i < unlockedSprites.Length && unlockedSprites[i] != null)
                img.sprite = unlockedSprites[i];
            else if (!unlocked && lockedSprites != null && i < lockedSprites.Length && lockedSprites[i] != null)
                img.sprite = lockedSprites[i];
        }
    }

    void ShiftLeft()
    {
        int temp           = slotAssignments[0];
        slotAssignments[0] = slotAssignments[1];
        slotAssignments[1] = slotAssignments[2];
        slotAssignments[2] = temp;

        ApplySlots();
        UpdateUI();
    }

    void ShiftRight()
    {
        int temp           = slotAssignments[2];
        slotAssignments[2] = slotAssignments[1];
        slotAssignments[1] = slotAssignments[0];
        slotAssignments[0] = temp;

        ApplySlots();
        UpdateUI();
    }

    int GetSlotOfCharacter(int characterIndex)
    {
        for (int slot = 0; slot < slotAssignments.Length; slot++)
            if (slotAssignments[slot] == characterIndex)
                return slot;

        return 0;
    }

    void ApplySlots(bool instant = false)
    {
        int layerOffset = 2;

        for (int characterIndex = 0; characterIndex < characters.Length; characterIndex++)
        {
            int slot      = GetSlotOfCharacter(characterIndex);
            bool isCenter = (slot == 1);
            bool unlocked = characterUnlocked[characterIndex];

            if (isCenter)
                targetColors[characterIndex] = unlocked ? frontColor : lockedColor;
            else
                targetColors[characterIndex] = unlocked ? backColor : lockedColor;

            targetScales[characterIndex] = Vector3.one * (isCenter ? frontScale : backScale);

            int siblingIndex = layerOffset + (isCenter ? 2 : slot == 0 ? 0 : 1);
            characters[characterIndex].SetSiblingIndex(siblingIndex);

            if (instant)
            {
                characters[characterIndex].anchoredPosition = slotPositions[slot];
                characters[characterIndex].localScale        = targetScales[characterIndex];

                Image img = characters[characterIndex].GetComponent<Image>();
                if (img != null) img.color = targetColors[characterIndex];
            }
        }
    }

    void UpdateUI()
    {
        int centeredCharacter = slotAssignments[1];
        bool unlocked         = characterUnlocked[centeredCharacter];

        if (characterNameText != null)
        {
            characterNameText.text     = unlocked ? characterNames[centeredCharacter] : lockedMessage;
            characterNameText.fontSize = unlocked ? unlockedFontSize : lockedFontSize;
        }

        letsGoButton.interactable = unlocked;

        Image buttonImage = letsGoButton.GetComponent<Image>();
        if (buttonImage != null)
            buttonImage.color = unlocked ? buttonUnlockedColor : buttonLockedColor;
    }

    void OnLetsGo()
    {
        int selectedIndex = slotAssignments[1];

        if (!characterUnlocked[selectedIndex])
        {
            Debug.LogWarning("Tried to select a locked character!");
            return;
        }

        CharacterSelector.SelectedCharacterIndex = selectedIndex;
        CharacterSelector.SelectedCharacterName  = characterNames[selectedIndex];

        Debug.Log($"Selected: {CharacterSelector.SelectedCharacterName} (index {selectedIndex})");

        string destination = GameProgress.HasCompletedTutorial
            ? songSelectSceneName
            : tutorialSceneName;

        SceneLoader loader = FindObjectOfType<SceneLoader>();
        if (loader != null)
            loader.LoadByName(destination);
        else
            SceneManager.LoadScene(destination);
    }
}

public static class CharacterSelector
{
    public static int    SelectedCharacterIndex { get; set; }
    public static string SelectedCharacterName  { get; set; }
}