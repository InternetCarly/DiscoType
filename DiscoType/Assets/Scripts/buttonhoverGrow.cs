using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonHoverGrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Hover Settings")]
    public float scaleFactor = 1.1f;
    public float duration = 0.15f;

    [Header("Click Settings")]
    public float clickShrinkScale = 0.9f;
    public float clickDuration = 0.08f;
    public AudioClip clickSound;

    private Vector3 originalScale;
    private AudioSource audioSource;

    void Start()
    {
        originalScale = transform.localScale;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(originalScale * scaleFactor, duration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originalScale, duration);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Shrink on press
        transform.DOScale(originalScale * clickShrinkScale, clickDuration);

        // Play click sound
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Spring back to hover scale if still hovering, otherwise back to normal
        bool stillHovered = RectTransformUtility.RectangleContainsScreenPoint(
            GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera
        );

        Vector3 releaseScale = stillHovered ? originalScale * scaleFactor : originalScale;
        transform.DOScale(releaseScale, duration);
    }
}