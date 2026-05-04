using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SkewScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Hover Squeeze Settings")]
    [Tooltip("How much the width shrinks on hover (0.9 = 90% of original width)")]
    public float squeezedScaleX = 0.9f;
    public float animationSpeed = 8f;

    [Header("Click Settings")]
    public string sceneToLoad = "SceneNameHere";
    public AudioClip clickSoundEffect;
    public int songIndex = 0;

    [Header("Click Animation")]
    public float shrinkScale = 0.85f;
    public float shrinkSpeed = 12f;

    private RectTransform rectTransform;
    private Vector3 defaultScale;
    private Vector3 targetScale;
    private AudioSource audioSource;
    private bool isClicked = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        defaultScale  = rectTransform.localScale;
        targetScale   = defaultScale;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        float speed = isClicked ? shrinkSpeed : animationSpeed;
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.deltaTime * speed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isClicked) return;
        targetScale = new Vector3(
            defaultScale.x * squeezedScaleX,
            defaultScale.y,
            defaultScale.z
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isClicked) return;
        targetScale = defaultScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isClicked) return;
        isClicked   = true;
        targetScale = defaultScale * shrinkScale;

        if (clickSoundEffect != null)
        {
            audioSource.clip = clickSoundEffect;
            audioSource.Play();
        }

        if (SongSelector.Instance != null)
        {
            SongSelector.Instance.SelectedSongIndex = songIndex;
            Debug.Log($"SkewScript: selected songIndex {songIndex}");
        }
        else
        {
            Debug.LogError("SkewScript: SongSelector.Instance is null!");
        }

        StartCoroutine(LoadSceneAfterDelay());
    }

    IEnumerator LoadSceneAfterDelay()
    {
        float delay = clickSoundEffect != null ? clickSoundEffect.length : 0.3f;
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneToLoad);
    }
}