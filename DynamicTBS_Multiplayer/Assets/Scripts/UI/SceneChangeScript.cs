using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeScript : MonoBehaviour
{
    public float transitiontime = 1f;

    public void LoadNextLevel(int nextScene)
    {
        StartCoroutine(Loadlevel(nextScene));
    }

    IEnumerator Loadlevel(int levelIndex)
    {
        yield return new WaitForSeconds(transitiontime);
        SceneManager.LoadScene(levelIndex);
    }
}
