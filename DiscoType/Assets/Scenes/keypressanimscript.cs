using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class keypressanimscript : MonoBehaviour
{
    public Sprite[] frames; // Drag your sprites into this list in the Inspector
    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (frames.Length > 0)
            spriteRenderer.sprite = frames[0];
    }

void Update() 
{
    if (Input.anyKeyDown && !Input.GetMouseButtonDown(0)  //detects any key press but ignores mouse clicks
                         && !Input.GetMouseButtonDown(1) 
                         && !Input.GetMouseButtonDown(2))
    {
        AdvanceFrame();
    }
}

    void AdvanceFrame()
    {
        if (frames.Length == 0) return;

        // Increment frame and loop back to 0 if at the end
        currentFrame = (currentFrame + 1) % frames.Length;
        spriteRenderer.sprite = frames[currentFrame];
    }
}
