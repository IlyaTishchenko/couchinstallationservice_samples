using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ScreenFade is a solid color canvas that is used to perform smooth transitions between UI and levels
public class ScreenFade : MonoBehaviour
{
    [SerializeField]
    Image FadeImage = null;

    [System.NonSerialized]
    public bool fadeOut = true;

    [System.NonSerialized]
    public bool isFading = false;

    public void SetColor(Color color)
    {
        FadeImage.color = color;
    }

    public void FadeIn(float xspeed = 1.0f)
    {
        StartCoroutine(FadeInCoroutine(xspeed));
    }

    public void FadeOut(float xspeed = 1.0f)
    {
        StartCoroutine(FadeOutCoroutine(xspeed));
    }

    IEnumerator FadeInCoroutine(float xspeed)
    {
        isFading = true;

        for (float ft = 0.0f; ft < 1.0f; ft += Time.deltaTime * xspeed)
        {
            var c = FadeImage.color;
            c.a = ft;
            FadeImage.color = c;
            yield return null;
        }

        var temp = FadeImage.color;
        temp.a = 1.0f;
        FadeImage.color = temp;

        fadeOut = false;

        isFading = false;
    }

    IEnumerator FadeOutCoroutine(float xspeed)
    {
        isFading = true;

        for (float ft = 1.0f; ft >= 0.0f; ft -= Time.deltaTime * xspeed)
        {
            var c = FadeImage.color;
            c.a = ft;
            FadeImage.color = c;
            yield return null;
        }

        var temp = FadeImage.color;
        temp.a = 0.0f;
        FadeImage.color = temp;

        fadeOut = true;

        isFading = false;
    }
}
