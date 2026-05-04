using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class VinylPeek : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public RectTransform vinylRect;
    public AudioSource audioSource;
    public TMP_Text trackNameText;

    [Header("Track Settings")]
    public AudioClip trackClip;
    public string trackName = "Track Name Here";

    [Header("Peek Settings")]
    public float peekDistance = 120f;
    public float animationSpeed = 6f;
    public bool spinVinyl = true;
    public float spinSpeed = 90f;

    public System.Action onHoverEnter;

    [Header("Stroke Settings")]
    public Color strokeColor = new Color(1f, 0.4f, 0.7f, 1f);
    public float strokeWidth = 3f;

    private Vector2 hiddenPosition;
    private Vector2 peekedPosition;
    private Vector2 targetPosition;
    private Vector2 velocity = Vector2.zero;
    private Outline outline;
    private float currentAlpha = 0f;
    private float targetAlpha = 0f;

    public bool IsHovered { get; private set; }

    void Start()
    {
        if (vinylRect == null)
        {
            Debug.LogError("VinylPeek: Assign the Vinyl RectTransform in the Inspector!");
            return;
        }
        hiddenPosition = vinylRect.anchoredPosition;
        peekedPosition = hiddenPosition + new Vector2(peekDistance, 0f);
        targetPosition = hiddenPosition;

        outline = vinylRect.gameObject.GetComponent<Outline>();
        if (outline == null)
            outline = vinylRect.gameObject.AddComponent<Outline>();

        outline.effectColor     = new Color(strokeColor.r, strokeColor.g, strokeColor.b, 0f);
        outline.effectDistance  = new Vector2(strokeWidth, strokeWidth);
        outline.useGraphicAlpha = false;

        if (trackNameText != null)
            trackNameText.text = "";
    }

    void Update()
    {
        if (vinylRect == null) return;

        vinylRect.anchoredPosition = Vector2.SmoothDamp(
            vinylRect.anchoredPosition,
            targetPosition,
            ref velocity,
            1f / animationSpeed
        );

        if (spinVinyl && IsHovered)
            vinylRect.Rotate(0f, 0f, -spinSpeed * Time.deltaTime);

        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * animationSpeed);
        outline.effectColor = new Color(strokeColor.r, strokeColor.g, strokeColor.b, currentAlpha);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsHovered      = true;
        targetPosition = peekedPosition;
        targetAlpha    = 1f;

        onHoverEnter?.Invoke();

        if (audioSource != null && trackClip != null)
        {
            audioSource.clip = trackClip;
            audioSource.Play();
        }

        if (trackNameText != null)
            trackNameText.text = trackName;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovered      = false;
        targetPosition = hiddenPosition;
        targetAlpha    = 0f;

        if (audioSource != null)
            audioSource.Stop();

        if (trackNameText != null)
            trackNameText.text = "";
    }
}