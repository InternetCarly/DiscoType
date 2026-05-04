using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keypressanimscript : MonoBehaviour
{
    [Header("Character Normal Frames")]
    public Sprite[] character0Frames;
    public Sprite[] character1Frames;
    public Sprite[] character1FramesB;
    public Sprite[] character2Frames;

    [Header("Character Error Frames")]
    public Sprite[] character0ErrorFrames;
    public Sprite[] character1ErrorFrames;
    public Sprite[] character2ErrorFrames;

    [Header("Character Height Overrides")]
    public float character0Height = 3f;
    public float character1Height = 3f;
    public float character2Height = 3f;

    [Header("Character 1 Animation Settings")]
    [Tooltip("How many times each animation loops before switching to the next")]
    public int loopsBeforeSwitch = 2;

    private NormalizeCharacterHeight normalizer;
    private SpriteRenderer spriteRenderer;
    private Sprite[] frames;
    private Sprite[] errorFrames;
    private int currentFrame = 0;
    private bool isPlayingError = false;

    private bool isCharacter1 = false;
    private Sprite[] character1ActiveFrames;
    private Sprite[] character1InactiveFrames;
    private int loopCount = 0;
    private bool usingFramesA = true;

    // Added for looping animation
    private bool isLooping = false;
    private float loopFrameInterval = 0.1f;
    private float loopFrameTimer = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalizer     = GetComponent<NormalizeCharacterHeight>();

        switch (CharacterSelector.SelectedCharacterIndex)
        {
            case 0:
                frames      = character0Frames;
                errorFrames = character0ErrorFrames;
                if (normalizer != null) normalizer.targetHeight = character0Height;
                break;
            case 1:
                isCharacter1             = true;
                character1ActiveFrames   = character1Frames;
                character1InactiveFrames = character1FramesB;
                frames      = character1ActiveFrames;
                errorFrames = character1ErrorFrames;
                if (normalizer != null) normalizer.targetHeight = character1Height;
                break;
            case 2:
                frames      = character2Frames;
                errorFrames = character2ErrorFrames;
                if (normalizer != null) normalizer.targetHeight = character2Height;
                break;
            default:
                frames      = character2Frames;
                errorFrames = character2ErrorFrames;
                if (normalizer != null) normalizer.targetHeight = character2Height;
                break;
        }

        if (frames == null || frames.Length == 0)
        {
            Debug.LogWarning($"No frames for character {CharacterSelector.SelectedCharacterIndex}, defaulting to character 2.");
            frames      = character2Frames;
            errorFrames = character2ErrorFrames;
            if (normalizer != null) normalizer.targetHeight = character2Height;
        }

        if (frames != null && frames.Length > 0)
        {
            spriteRenderer.sprite = frames[0];
            normalizer?.NormalizeHeight();
        }
    }

    // Added for looping animation
    void Update()
    {
        if (!isLooping) return;
        loopFrameTimer += Time.deltaTime;
        if (loopFrameTimer >= loopFrameInterval)
        {
            loopFrameTimer = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            spriteRenderer.sprite = frames[currentFrame];
            normalizer?.NormalizeHeight();
        }
    }

    // Added for looping animation
    public void StartLoopAnimation(float frameInterval = 0.1f)
    {
        isLooping         = true;
        loopFrameInterval = frameInterval;
        loopFrameTimer    = 0f;
        currentFrame      = 0;
        isPlayingError    = false;
    }

    public void OnCorrectKey()
    {
        if (isPlayingError || isLooping) return;
        if (frames == null || frames.Length == 0) return;

        currentFrame++;

        if (currentFrame >= frames.Length)
        {
            currentFrame = 0;

            if (isCharacter1)
            {
                loopCount++;

                if (loopCount >= loopsBeforeSwitch)
                {
                    loopCount    = 0;
                    usingFramesA = !usingFramesA;
                    frames = usingFramesA ? character1ActiveFrames : character1InactiveFrames;
                }
            }
        }

        spriteRenderer.sprite = frames[currentFrame];
        normalizer?.NormalizeHeight();
    }

    public void OnWrongKey()
    {
        if (isPlayingError || isLooping) return;
        StartCoroutine(PlayErrorAnimation());
    }

    IEnumerator PlayErrorAnimation()
    {
        isPlayingError = true;

        if (errorFrames != null)
        {
            for (int i = 0; i < errorFrames.Length; i++)
            {
                spriteRenderer.sprite = errorFrames[i];
                normalizer?.NormalizeHeight();
                yield return new WaitForSeconds(0.05f);
            }
        }

        spriteRenderer.sprite = frames[currentFrame];
        normalizer?.NormalizeHeight();
        isPlayingError = false;
    }
}