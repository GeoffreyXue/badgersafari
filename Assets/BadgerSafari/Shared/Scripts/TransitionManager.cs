using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages transitions between scenes using a fade screen.
/// </summary>
public class TransitionManager : MonoBehaviour
{
    public FadeScreen fadeScreen;

    public void GoToScene(int sceneIndex) {
        StartCoroutine(GoToSceneRoutine(sceneIndex));
    }

    IEnumerator GoToSceneRoutine(int sceneIndex) {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }
}
