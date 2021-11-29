using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StamanaContainer : MonoBehaviour
{

    public Slider stamanaBarBase;
    public Vector2 offset;

    [Header("Bar Variables")]
    public int numStamanaBars = 1;
    public List<Slider> stamanaBars;

    private void Start()
    {
        ResizeStamanaBars();
    }

    public void UpdateStamana(float value)
    {
        int barIndex = -1;
        for (int i = 0; i < numStamanaBars; i++)
        {
            if (stamanaBars[i].value < 1)
            {
                if (stamanaBars[i].value == 0 && value  < 0)
                    i--;
                barIndex = i;
                break;
            }
            barIndex = i;
        }

        float difference = stamanaBars[barIndex].value + (value * Time.deltaTime);
        if (barIndex >= 0)
        {
            if (difference < 0)
            {
                stamanaBars[barIndex].value = 0;
                if ((barIndex - 1) > 0)
                    stamanaBars[barIndex - 1].value = 1 - (value * Time.deltaTime) + stamanaBars[barIndex].value;
            }
            else if (difference > 1)
            {
                stamanaBars[barIndex].value = 1;
                if (stamanaBars.Count < barIndex + 1)
                    stamanaBars[barIndex + 1].value = (value * Time.deltaTime) + stamanaBars[barIndex].value - 1;
            }
            else
            {
                stamanaBars[barIndex].value = stamanaBars[barIndex].value + (value * Time.deltaTime);
            }
        }
    }

    public void UseUpStamana()
    {
        int barIndex = -1;
        for (int i = numStamanaBars - 1; i >= 0; i--)
        {
            if (stamanaBars[i].value > 0)
            {
                barIndex = i;
                break;
            }
            barIndex = i;
        }
        stamanaBars[barIndex].value = 0;
    }

    public bool HasStamana()
    {
        if (numStamanaBars > 0)
        {
            if (stamanaBars[0].value > 0)
            {
                return true;
            }
        }
        return false;
    }

    public void ResizeStamanaBars()
    {
        while (stamanaBars.Count > 0)
        {
            GameObject.Destroy(stamanaBars[0].gameObject);
            stamanaBars.RemoveAt(0);
        }

        for (int i = 0; i < numStamanaBars; i++)
        {
            GameObject bar = GameObject.Instantiate(stamanaBarBase.gameObject);
            bar.transform.SetParent(this.transform);
            RectTransform rectTransform = bar.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(offset.x + (rectTransform.rect.width * i), offset.y);
            bar.GetComponent<Slider>().value = 1;
            stamanaBars.Add(bar.GetComponent<Slider>());
        }
    }
}

