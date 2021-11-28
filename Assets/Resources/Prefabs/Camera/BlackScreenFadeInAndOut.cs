using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreenFadeInAndOut : MonoBehaviour
{
    public float fadeIncrement;
    public float fadeSpeed;
    private bool _hidden;
    private bool fading = false;
    public bool IsEnabled
    {
        get
        {
            return _hidden;
        }
        set
        {
            _hidden = value;
        }
    }

    

    public void Fade()
    {
        StartCoroutine("FadeInAndOut");
    }

    IEnumerator FadeInAndOut()
    {
        if (!fading)
        {
            fading = true;
            Image fadeImage = this.GetComponent<Image>();
            bool fadeType = fadeImage.color.a == 1;
            while (fadeImage.color.a != (fadeType ? 0f : 1f))
            {
                if (!fadeType)
                {
                    if (fadeImage.color.a + fadeIncrement > 1)
                    {
                        fadeImage.color = new Color(0, 0, 0, 1);
                    }
                    else
                    {
                        fadeImage.color = new Color(0, 0, 0, fadeImage.color.a + fadeIncrement);
                    }
                }
                else
                {
                    if (fadeImage.color.a + fadeIncrement < 0)
                    {
                        fadeImage.color = new Color(0, 0, 0, 0);
                    }
                    else
                    {
                        fadeImage.color = new Color(0, 0, 0, fadeImage.color.a - fadeIncrement);
                    }
                }
                yield return new WaitForSeconds(fadeSpeed);
            }
            fading = false;
        }
    }
}
