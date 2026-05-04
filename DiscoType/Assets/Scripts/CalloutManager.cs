using UnityEngine;
using UnityEngine.UI;

public class CalloutManager : MonoBehaviour
{
    [Header("Callout Images")]
    public Image[] calloutImages;

    [Header("Settings")]
    public float displayDuration = 1.5f;
    public float punchScale = 1.3f;
    public float punchSpeed = 12f;
    public float settleSpeed = 6f;

    [Header("Position Randomization")]
    public Vector2[] spawnPositions;

    private int wordsSinceLastCallout = 0;
    private int wordsPerCallout = 5;
    private bool isShowing = false;

    void Start()
    {
        // Hide all callouts at start
        foreach (var img in calloutImages)
            if (img != null)
                img.gameObject.SetActive(false);
    }

    // Call this from StatsTracker whenever a word is completed
    public void OnWordCompleted()
    {
        wordsSinceLastCallout++;
        if (wordsSinceLastCallout >= wordsPerCallout && !isShowing)
        {
            wordsSinceLastCallout = 0;
            StartCoroutine(ShowCallout());
        }
    }

    System.Collections.IEnumerator ShowCallout()
    {
        isShowing = true;

        // Pick a random callout image
        int randomIndex = Random.Range(0, calloutImages.Length);
        Image callout = calloutImages[randomIndex];

        if (callout == null)
        {
            isShowing = false;
            yield break;
        }

        // Pick a random spawn position if any are set
        if (spawnPositions != null && spawnPositions.Length > 0)
        {
            RectTransform rt = callout.GetComponent<RectTransform>();
            rt.anchoredPosition = spawnPositions[Random.Range(0, spawnPositions.Length)];
        }

        callout.gameObject.SetActive(true);
        RectTransform rect = callout.GetComponent<RectTransform>();
        rect.localScale = Vector3.zero;

        // Punch in
        float elapsed = 0f;
        float punchDuration = 1f / punchSpeed;
        while (elapsed < punchDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / punchDuration);
            float eased = EaseOutBack(t);
            rect.localScale = Vector3.one * Mathf.Lerp(0f, punchScale, eased);
            yield return null;
        }

        // Settle
        elapsed = 0f;
        float settleDuration = 1f / settleSpeed;
        while (elapsed < settleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / settleDuration);
            rect.localScale = Vector3.one * Mathf.Lerp(punchScale, 1f, t);
            yield return null;
        }

        rect.localScale = Vector3.one;

        // Hold
        yield return new WaitForSeconds(displayDuration);

        // Punch out
        elapsed = 0f;
        while (elapsed < settleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / settleDuration);
            rect.localScale = Vector3.one * Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        callout.gameObject.SetActive(false);
        isShowing = false;
    }

    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}