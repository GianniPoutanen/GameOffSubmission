using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageBar : Bar, IBar
{
    public Image fillImage;

    public void SetFill(float fillAmount)
    {
        fillImage.fillAmount = fillAmount;
    }

}
