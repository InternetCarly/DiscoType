using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class dancefloorAnim : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite frameOne;
    public Sprite frameTwo;

    [Header("Timing")]
    [Tooltip("How long each frame is displayed in seconds")]
    public float frameInterval = 0.5f;

    private Image image;
    private float timer = 0f;
    private bool showingFrameOne = true;

    void Awake()
    {
        image = GetComponent<Image>();

        if (frameOne == null || frameTwo == null)
        {
            Debug.LogError("SpriteOscillator: Assign both sprites in the Inspector!");
            return;
        }

        image.sprite = frameOne;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= frameInterval)
        {
            timer = 0f;
            showingFrameOne = !showingFrameOne;
            image.sprite = showingFrameOne ? frameOne : frameTwo;
        }
    }
}