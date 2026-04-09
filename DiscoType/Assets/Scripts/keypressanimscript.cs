using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keypressanimscript : MonoBehaviour
{
    public Sprite[] frames;
    public Sprite[] errorFrames; // Drag your ErrorAnimation sprites here

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    private bool isPlayingError = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (frames.Length > 0)
            spriteRenderer.sprite = frames[0];
    }

    public void OnCorrectKey()
    {
        if (isPlayingError) return; // Don't advance during error animation

        if (frames.Length == 0) return;
        currentFrame = (currentFrame + 1) % frames.Length;
        spriteRenderer.sprite = frames[currentFrame];
    }

    public void OnWrongKey()
    {
        if (isPlayingError) return; // Don't restart if already playing
        StartCoroutine(PlayErrorAnimation());
    }

    IEnumerator PlayErrorAnimation()
    {
        isPlayingError = true;

        for (int i = 0; i < errorFrames.Length; i++)
        {
            spriteRenderer.sprite = errorFrames[i];
            yield return new WaitForSeconds(0.05f); // Adjust speed as needed
        }

        // Return to current normal frame when error animation finishes
        spriteRenderer.sprite = frames[currentFrame];
        isPlayingError = false;
    }
}