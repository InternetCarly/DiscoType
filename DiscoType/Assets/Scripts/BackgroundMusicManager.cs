using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance { get; private set; }

    [Header("References")]
    public AudioSource audioSource;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float volume = 0.25f;

    [Header("Scene Settings")]
    [Tooltip("Names of scenes where background music should play")]
    public string[] scenesWithMusic = { "Title Scene", "CharacterSelectScene", "ResultsScene" };

    [Tooltip("Names of scenes where background music should be silent")]
    public string[] scenesWithoutMusic = { "SongSelectScene", "GameScene", "GameTutorialScene" };

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.volume = volume;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnValidate()
    {
        if (audioSource != null)
            audioSource.volume = volume;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (audioSource == null)
        {
            Debug.LogError("BackgroundMusicManager: AudioSource is null after scene load!");
            return;
        }

        // Reset to default volume on every scene load
        volume = 0.25f;
        audioSource.volume = volume;

        Debug.Log($"BackgroundMusicManager: Scene loaded — '{scene.name}', isPlaying: {audioSource.isPlaying}, volume: {audioSource.volume}");

        foreach (string s in scenesWithoutMusic)
        {
            if (scene.name == s)
            {
                Debug.Log($"BackgroundMusicManager: Stopping music for scene '{scene.name}'");
                Stop();
                return;
            }
        }

        foreach (string s in scenesWithMusic)
        {
            if (scene.name == s)
            {
                Debug.Log($"BackgroundMusicManager: Playing music for scene '{scene.name}'");
                Play();
                return;
            }
        }

        Debug.LogWarning($"BackgroundMusicManager: Scene '{scene.name}' not found in either list!");
    }

    public void Play()
    {
        audioSource.volume = volume;
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void Stop()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        audioSource.volume = newVolume;
    }
}