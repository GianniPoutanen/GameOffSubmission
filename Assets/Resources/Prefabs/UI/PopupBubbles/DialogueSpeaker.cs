using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSpeaker : MonoBehaviour
{
    [Header("Priority Settings")]
    public float interactDistance = 6f;
    public int priority = 0;
    [Header("Dialogue Settings")]
    public YarnProgram yarnScript;
    public string StartNode;

    [Header("Indicator Offset")]
    public Vector2 popupOffset;

    [Header("Dialogue Colours")]
    public Color bubbleColour = Color.white;
    public Color textColour = new Color(255f/82f, 255f / 82f, 255f / 82f,1);


    private void Start()
    {
        GameObject.Find("Dialogue").GetComponent<DialogueManager>().allSpeakers.Add(this);
        GameObject.Find("Dialogue").GetComponent<DialogueManager>().dialogueRunner.Add(yarnScript);
    }

    // Update is called once per frame
    void Update()
    {
        CheckClosestAgent();
    }

    public void CheckClosestAgent()
    {
        // Check if player is close enough and closer than a different agent
        if (Vector3.Distance(this.transform.position, GameAssets.Instance.playerCharacter.transform.position) < interactDistance)
        {
            if (GameAssets.Instance.dialogueManager.closestSpeaker == null || ((GameAssets.Instance.dialogueManager.closestSpeaker != this &&
                Vector3.Distance(GameAssets.Instance.dialogueManager.closestSpeaker.transform.position, GameAssets.Instance.playerCharacter.transform.position) >
                Vector3.Distance(this.transform.position, GameAssets.Instance.playerCharacter.transform.position))))
            {
                GameAssets.Instance.dialogueManager.UpdateClosestDialogueAgent(this);
            }
        }
        else if (GameAssets.Instance.dialogueManager.closestSpeaker == this)
        {
            GameAssets.Instance.dialogueManager.UpdateClosestDialogueAgent(null);
        }
    }
}
