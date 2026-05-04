using UnityEngine;

public class SongSelector : MonoBehaviour
{
    public static SongSelector Instance { get; private set; }

    public int SelectedSongIndex { get; set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}