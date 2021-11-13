using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePositionTest : MonoBehaviour
{
    public GameObject followObj;

    private void Update()
    {
        this.GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(followObj.transform.position);
        //this.transform.position = Camera.main.WorldToScreenPoint(followObj.transform.position);
    }
}
