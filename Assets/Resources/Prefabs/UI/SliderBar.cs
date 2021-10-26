using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : Bar, IBar
{
    public Slider silderBar;
    public void SetFill(float fillAmount)
    {
        silderBar.value = fillAmount;
    }
}

public interface IBar
{
    public void SetFill(float fillAmount);
}