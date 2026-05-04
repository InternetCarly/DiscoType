using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Song Audio Clips")]
    [Tooltip("Add clips in the same order as your song select screen — index 0, 1, 2")]
    public AudioClip[] songClips;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        int index = SongSelector.Instance.SelectedSongIndex;
        Debug.Log($"GameAudioManager: SelectedSongIndex is {index}");

        if (songClips == null || songClips.Length == 0)
        {
            Debug.LogError("GameAudioManager: No song clips assigned!");
            return;
        }

        if (index >= songClips.Length)
        {
            Debug.LogWarning($"GameAudioManager: Index {index} out of range, defaulting to 0.");
            index = 0;
        }

        audioSource.clip = songClips[index];
        audioSource.Play();
    }
}