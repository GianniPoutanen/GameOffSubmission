using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextScroll : MonoBehaviour
{

    public TextMeshPro messageText;
    private int charIndex;

    private float timer;
    public float timerPerChar;

    private int stringIndex = 0;

    public TextBlock textBlock;

    public bool writing = true;


    private void Start()
    {
        messageText.text = "";
    }

    public void PlayText(string text)
    {
        textBlock = new TextBlock(text);
        StartCoroutine(HandleMessageScroll());
    }



    private IEnumerator HandleMessageScroll()
    {
        if (messageText != null && textBlock != null)
        {
            while (writing)
            {
                timer -= Time.deltaTime;
                if (messageText.text != textBlock.textToWrite)
                {
                    if (timerPerChar > 0)
                    {
                        if (timer <= 0f)
                        {
                            timer = timerPerChar;
                            charIndex++;
                            if (textBlock.textToWrite[messageText.text.Length] == ' ')
                                messageText.text += textBlock.textToWrite;
                            messageText.text += textBlock.textToWrite;
                        }
                    }
                    else
                    {
                        messageText.text = textBlock.textToWrite;
                    }
                }
                else
                {
                    writing = false;
                    timer = textBlock.textSpeed;
                }

                if (Input.GetKeyDown(KeyCode.Space)) 
                {
                    messageText.text = textBlock.textToWrite;
                }
            }
        }
        yield return null;
    }

    public float GetTextMaxWidth()
    {
        string tempText = messageText.text;

        messageText.text = textBlock.textToWrite;
        messageText.ForceMeshUpdate();

        float width = messageText.GetRenderedValues(true).x;

        messageText.text = tempText;
        messageText.ForceMeshUpdate();

        return width;
    }
}

public class TextBlock
{
    [SerializeField]
    [TextArea(4, 20)]
    public string textToWrite;
    public float textSpeed;

    public TextBlock(string text)
    {
        textToWrite = text;
        textSpeed = 0.5f;
    }

    public TextBlock(string text, float speed)
    {
        textToWrite = text;
        textSpeed = speed;
    }
}
