using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Settings")]
    public float delay = 2f;

    public void LoadByName(string sceneName)
    {
        StartCoroutine(LoadByNameDelayed(sceneName));
    }

    public void LoadNextScene()
    {
        StartCoroutine(LoadNextSceneDelayed());
    }

    IEnumerator LoadByNameDelayed(string sceneName)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadNextSceneDelayed()
    {
        yield return new WaitForSeconds(delay);
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextIndex);
    }
}