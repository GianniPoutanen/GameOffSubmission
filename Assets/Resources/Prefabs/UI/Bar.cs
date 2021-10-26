using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    public virtual void SetFillAmount(float fillAmount)
    {
        this.GetComponent<IBar>().SetFill(fillAmount);
    }
}
