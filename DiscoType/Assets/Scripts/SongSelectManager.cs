using UnityEngine;
using UnityEngine.SceneManagement;

public class SongSelectManager : MonoBehaviour
{
    public VinylPeek[] vinylPeeks;
    public string nextSceneName = "GameScene";
    public SceneLoader sceneLoader;

    private VinylPeek lastHovered;

    void Start()
    {
        foreach (var peek in vinylPeeks)
        {
            var p = peek;
            p.onHoverEnter += () => lastHovered = p;
        }
    }

    public void OnSelectButton()
    {
        if (lastHovered != null)
        {
            Debug.Log($"OnSelectButton — lastHovered: {lastHovered.trackName}");
            sceneLoader.LoadByName(nextSceneName);
        }
        else
        {
            Debug.LogWarning("No song hovered — hover over an album before selecting!");
        }
    }
}