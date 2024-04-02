using System.Collections;
using UnityEngine;

/// <summary>
/// Fades the screen in and out.
/// </summary>
public class FadeScreen : MonoBehaviour
{
    public bool fadeOnStart = true;
    public float fadeDuration = 2.0f;
    public Color fadeColor = Color.black;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (fadeOnStart)
        {
            FadeIn();
        }
    }

    public void FadeIn() {
        Fade(1, 0);
    }

    public void FadeOut() {
        Fade(0, 1);
    }

    public void Fade(float alphaIn, float alphaOut)
    {
        StartCoroutine(FadeTo(alphaIn, alphaOut));
    }

    public IEnumerator FadeTo(float alphaIn, float alphaOut)
    {
        for (float t = 0.0f; t < fadeDuration; t += Time.deltaTime)
        {
            Color newColor = fadeColor;
            newColor.a = Mathf.Lerp(alphaIn, alphaOut, t);
    
            rend.material.color = newColor;
            yield return null;
        }

        Color finalColor = fadeColor;
        finalColor.a = alphaOut;
        rend.material.color = finalColor;
    }
}
