using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class CountdownTimer : MonoBehaviour
{
    [Header("References")]
    public GameObject countdownPanel;
    public TMP_Text countdownText;

    [Header("Countdown Settings")]
    public string[] steps = { "READY", "3", "2", "1", "GO!" };
    public float defaultStepDelay = 0.8f;
    public float readyDelay = 1f;
    public float goDelay = 0.7f;

    [Header("Punch Animation")]
    public float punchScaleAmount = 1.4f;
    public float punchRotationAmount = 8f;
    public float punchInSpeed = 12f;
    public float punchOutSpeed = 6f;
    public float goScaleAmount = 1.8f;
    public float readyScaleAmount = 1.2f;

    public Action onGo;
    public Action onComplete;

    void Awake()
    {
        if (countdownPanel != null)
            countdownPanel.SetActive(false);
    }

    public void StartCountdown()
    {
        StartCoroutine(RunCountdown());
    }

    IEnumerator RunCountdown()
    {
        if (countdownPanel != null)
            countdownPanel.SetActive(true);

        for (int i = 0; i < steps.Length; i++)
        {
            string step = steps[i];
            countdownText.text = step;

            float displayTime    = GetDisplayTime(step);
            float targetScale    = GetTargetScale(step);
            float rotDir         = (i % 2 == 0) ? 1f : -1f;
            float targetRotation = (step == "READY" || step == "GO!") ? 0f : punchRotationAmount * rotDir;

            yield return StartCoroutine(PunchAnimation(
                countdownText.rectTransform,
                targetScale,
                targetRotation,
                displayTime
            ));

            // Fire onGo AFTER the GO! animation finishes
            if (step == "GO!")
                onGo?.Invoke();
        }

        countdownText.rectTransform.localScale    = Vector3.one;
        countdownText.rectTransform.localRotation = Quaternion.identity;

        if (countdownPanel != null)
            countdownPanel.SetActive(false);

        onComplete?.Invoke();
    }

    float GetDisplayTime(string step)
    {
        if (step == "READY") return readyDelay;
        if (step == "GO!")   return goDelay;
        return defaultStepDelay;
    }

    float GetTargetScale(string step)
    {
        if (step == "GO!")   return goScaleAmount;
        if (step == "READY") return readyScaleAmount;
        return punchScaleAmount;
    }

    IEnumerator PunchAnimation(RectTransform target, float targetScale, float targetRotation, float holdTime)
    {
        target.localScale    = Vector3.zero;
        target.localRotation = Quaternion.identity;

        float elapsed         = 0f;
        float punchInDuration = 1f / punchInSpeed;

        while (elapsed < punchInDuration)
        {
            elapsed += Time.deltaTime;
            float t     = Mathf.Clamp01(elapsed / punchInDuration);
            float eased = EaseOutBack(t);
            target.localScale    = Vector3.one * Mathf.Lerp(0f, targetScale, eased);
            target.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, targetRotation, eased));
            yield return null;
        }

        elapsed = 0f;
        float settleStart    = targetScale;
        float rotStart       = targetRotation;
        float settleDuration = 1f / punchOutSpeed;

        while (elapsed < settleDuration)
        {
            elapsed += Time.deltaTime;
            float t     = Mathf.Clamp01(elapsed / settleDuration);
            float eased = EaseOutQuad(t);
            target.localScale    = Vector3.one * Mathf.Lerp(settleStart, 1f, eased);
            target.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(rotStart, 0f, eased));
            yield return null;
        }

        float remainingHold = holdTime - (punchInDuration + settleDuration);
        if (remainingHold > 0f)
            yield return new WaitForSeconds(remainingHold);
    }

    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    float EaseOutQuad(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }
}