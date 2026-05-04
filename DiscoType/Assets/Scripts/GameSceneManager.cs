using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [Header("References")]
    public CountdownTimer countdownTimer;
    public GameObject overlayPanel;
    public TypingManager typingManager;
    public SongSlider songSlider;
    public AudioSource musicSource; // Drag your Song AudioSource here

    void Start()
    {
        if (overlayPanel != null)
            overlayPanel.SetActive(true);

        if (typingManager != null)
            typingManager.StopTyping();

        if (musicSource != null)
            musicSource.Stop(); // Make sure it doesn't play on awake

        countdownTimer.onGo = () =>
        {
            if (overlayPanel != null)
                overlayPanel.SetActive(false);
        };

        countdownTimer.onComplete = () =>
        {
            Debug.Log("Countdown complete — starting typing");

            if (typingManager != null)
                typingManager.StartTyping();
            else
                Debug.LogError("GameSceneManager: TypingManager is null!");

            if (songSlider != null)
                songSlider.StartSlider();
            else
                Debug.LogError("GameSceneManager: SongSlider is null!");

            if (musicSource != null)
                musicSource.Play(); // Start audio exactly when slider starts
        };

        countdownTimer.StartCountdown();
    }
}