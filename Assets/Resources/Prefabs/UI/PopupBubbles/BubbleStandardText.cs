using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BubbleStandardText : MonoBehaviour
{
    public TMPro.TMP_Text messageText;

    public void ClearText()
    {
        messageText.text = "";
    }
}
