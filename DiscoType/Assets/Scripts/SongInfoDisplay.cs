using UnityEngine;
using TMPro;

public class SongInfoDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text songTitleText;
    public TMP_Text artistText;

    [Header("Song Data")]
    public string[] songTitles  = { "Song One",   "Song Two",   "Song Three"  };
    public string[] artistNames = { "Artist One", "Artist Two", "Artist Three" };

    void Start()
    {
        int index = SongSelector.Instance.SelectedSongIndex;

        if (songTitleText != null && index < songTitles.Length)
            songTitleText.text = songTitles[index];

        if (artistText != null && index < artistNames.Length)
            artistText.text = artistNames[index];
    }
}