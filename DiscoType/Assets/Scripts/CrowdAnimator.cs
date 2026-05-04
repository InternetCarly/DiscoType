using UnityEngine;
using UnityEngine.UI;

public class CrowdAnimator : MonoBehaviour
{
    [Header("References")]
    public Image crowdImage;
    public SongSlider songSlider;

    [Header("Frames")]
    public Sprite frame1;
    public Sprite frame2;

    [Header("Settings")]
    public float frameInterval = 0.2f;

    private float timer = 0f;
    private bool showingFrame1 = true;

    void Update()
    {
        if (!songSlider.IsRunning) return;

        timer += Time.deltaTime;
        if (timer >= frameInterval)
        {
            timer = 0f;
            showingFrame1 = !showingFrame1;
            crowdImage.sprite = showingFrame1 ? frame1 : frame2;
        }
    }
}