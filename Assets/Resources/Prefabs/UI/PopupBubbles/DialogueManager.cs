using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    [Header("Popup Indicator Settings")]
    public GameObject dialoguePopupIndicator;
    public Vector2 indicatorOffset;


    [Space(5)]
    [Header("Bubble Settings")]
    public Vector2 textPadding;
    public GameObject bubbleIndicator;
    public GameObject dialogueBubble;
    public Vector2 bubbleOffset;
    public TMP_Text standardBubbleText;
    public Image bubbleImage;
    public float camOffsetMultiplyer;
    public float camBubbleOffsetMultiplyer;
    public float camOffset;

    [Space(5)]
    [Header("Dialogue Player Colours")]
    public Color bubbleColour = Color.white;
    public Color textColour = new Color(255f / 82f, 255f / 82f, 255f / 82f, 1f);
    public Color optionsColour = new Color(255f / 60f, 255f / 60f, 255f / 60f, 1f);


    [Space(5)]
    [Header("Object References")]
    public string playerCharactername;
    public TMP_Text dialogueText;
    public List<TMP_Text> optionTexts;
    public DialogueRunner dialogueRunner;
    public DialogueUI DUI;
    public DialogueSpeaker closestSpeaker;
    public List<DialogueSpeaker> allSpeakers = new List<DialogueSpeaker>();
    public GameObject currentSpeaker;
    private bool dialogOpen;

    [Space(5)]
    [Header("Yarn Controlled Properties")]
    private int numOptions = 0;
    private bool _lineFinished = false;
    public bool LineFinished
    {
        get { return _lineFinished; }
        set
        {
            _lineFinished = value;
        }
    }
    public float textSpeed;
    private string _fullText;
    public string fullText
    {
        set
        {
            _fullText = value;
        }
    }

    public bool InDialog
    {
        get
        {
            return dialogOpen;
        }
        set
        {
            if (!value)
            {
                //Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.Follow = GameAssets.Instance.playerCharacter.transform;
            }
            else
            {
                dialoguePopupIndicator.SetActive(false);
                //Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.Follow = null;
            }
            Debug.Log(value ? "Dialog Started" : "Dialog Ended");
            dialogOpen = value;
        }
    }


    private void Awake()
    {
        GameAssets.Instance.dialogueManager = this;
        SetupEvents();

        // Event handlers for when the dialogue bubble updates
        dialogueRunner.Dialogue.lineHandler += new Yarn.Dialogue.LineHandler(HandleLineBubbleScaling);
        dialogueRunner.Dialogue.optionsHandler += new Yarn.Dialogue.OptionsHandler(HandleOptionsBubbleScaling);
    }

    private void Update()
    {
        if (!InDialog)
        {
            if ((Input.GetKeyDown(KeyCode.E)))
            {
                if (closestSpeaker != null && GameAssets.Instance.playerCharacter.GetComponent<PlayerMovementBehaviour>().isGrounded)
                {
                    dialogueRunner.yarnScripts = new YarnProgram[] { closestSpeaker.yarnScript };
                    dialogueRunner.StartDialogue(closestSpeaker.StartNode);
                    if (currentSpeaker.GetComponent<DialogueSpeaker>() != null) 
                    {
                        currentSpeaker.GetComponent<DialogueSpeaker>().PlayVoiceClip();
                    }
                }
            }
        }
        else
        {
            SetDialogueBubblePosition();
            //SetBubbleSize();
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
            {
                if (LineFinished)
                {
                    Debug.Log("Continued Dialog");
                    dialogueRunner.Dialogue.Continue();
                    currentSpeaker.GetComponent<DialogueSpeaker>().PlayVoiceClip();
                    SetBubblePosition();
                }
                else
                {
                    DUI.textSpeed = 0;
                }
            }
        }

        if (dialoguePopupIndicator.activeSelf)
        {
            SetPopupIndicatorPosition();
            Vector3 screenPos = Camera.main.WorldToScreenPoint(closestSpeaker.transform.position);
        }
    }

    #region Handle Bubble Sizing and Positioning
    public Yarn.Dialogue.HandlerExecutionType HandleLineBubbleScaling(Yarn.Line line)
    {
        string stringID = line.ID;
        string text = (dialogueRunner as ILineLocalisationProvider).GetLocalisedTextForLine(line);

        string tempText = standardBubbleText.text;
        standardBubbleText.SetText(text);
        standardBubbleText.ForceMeshUpdate();

        Vector2 textSize = standardBubbleText.GetRenderedValues(true);
        dialogueBubble.GetComponent<RectTransform>().sizeDelta = textSize + textPadding;

        standardBubbleText.SetText(tempText);
        standardBubbleText.ForceMeshUpdate();

        return Yarn.Dialogue.HandlerExecutionType.PauseExecution;
    }

    public void HandleOptionsBubbleScaling(Yarn.OptionSet set)
    {
        Vector2 textSize = new Vector2();
        foreach (Yarn.OptionSet.Option option in set.Options)
        {
            string text = (dialogueRunner as ILineLocalisationProvider).GetLocalisedTextForLine(option.Line);

            string tempText = standardBubbleText.text;
            standardBubbleText.SetText(text);
            standardBubbleText.ForceMeshUpdate();

            if (standardBubbleText.GetRenderedValues(true).x > textSize.x)
                textSize = standardBubbleText.GetRenderedValues(true);

            standardBubbleText.SetText(tempText);
            standardBubbleText.ForceMeshUpdate();
        }
        dialogueBubble.GetComponent<RectTransform>().sizeDelta = new Vector2(textSize.x, textSize.y * set.Options.Length) + textPadding;
    }

    public void SetPopupIndicatorPosition()
    {
        Vector3 indicatorPosition;
        float camDistance = Vector3.Distance(Camera.main.gameObject.transform.position, closestSpeaker.transform.position);
        float camZoomOffset = (camOffset - camDistance) * camOffsetMultiplyer;
        if (Camera.main.WorldToScreenPoint(closestSpeaker.transform.position).x <
                Camera.main.WorldToScreenPoint(GameAssets.Instance.playerCharacter.transform.position).x)
        {
            indicatorPosition = Camera.main.WorldToScreenPoint(closestSpeaker.transform.position) + new Vector3(-indicatorOffset.x, indicatorOffset.y * camZoomOffset);
            dialoguePopupIndicator.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            indicatorPosition = Camera.main.WorldToScreenPoint(closestSpeaker.transform.position) + (Vector3)indicatorOffset * camZoomOffset;
            dialoguePopupIndicator.transform.localScale = new Vector3(1, 1, 1);
        }

        dialoguePopupIndicator.GetComponent<RectTransform>().anchoredPosition = indicatorPosition;
    }

    public void SetDialogueBubblePosition()
    {
        Vector3 indicatorPosition;

        float camDistance = Vector3.Distance(Camera.main.gameObject.transform.position, currentSpeaker.transform.position);
        float camZoomOffset = (camOffset - camDistance) * camBubbleOffsetMultiplyer;
        if (Camera.main.WorldToScreenPoint(closestSpeaker.transform.position).x <
                Camera.main.WorldToScreenPoint(GameAssets.Instance.playerCharacter.transform.position).x)
        {
            indicatorPosition = Camera.main.WorldToScreenPoint(currentSpeaker.transform.position) + (Vector3)bubbleOffset * camZoomOffset;
            bubbleIndicator.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            indicatorPosition = Camera.main.WorldToScreenPoint(currentSpeaker.transform.position) + new Vector3(-bubbleOffset.x, bubbleOffset.y * camZoomOffset);
            bubbleIndicator.transform.localScale = new Vector3(-1, 1, 1);
        }

        dialogueBubble.GetComponent<RectTransform>().anchoredPosition = indicatorPosition;
    }

    #endregion Handle Bubble Sizing and Positioning

    public void UpdateClosestDialogueAgent(DialogueSpeaker newAgent)
    {
        closestSpeaker = newAgent;
        if (closestSpeaker != null && !InDialog)
        {
            SetPopupIndicatorPosition();
            dialoguePopupIndicator.GetComponent<Image>().color = closestSpeaker.GetComponent<DialogueSpeaker>().bubbleColour;
            dialoguePopupIndicator.SetActive(true);
        }
        else
        {
            dialoguePopupIndicator.SetActive(false);
        }
    }


    public GameObject GetSpeakerByName(string name)
    {
        if (name == playerCharactername)
        {
            return GameAssets.Instance.playerCharacter;
        }

        foreach (DialogueSpeaker speaker in allSpeakers)
        {
            if (speaker.name == name)
            {
                return speaker.gameObject;
            }
        }
        return null;
    }

    #region Functions Called By YarnSpinner Classes

    public void SetBubblePosition()
    {
        if (InDialog && currentSpeaker != null)
        {
            SetDialogueBubblePosition();
        }
    }
    public void ClearCurentSpeaker()
    {
        currentSpeaker = null;
    }

    public void SetSpeakerInfo(string[] info)
    {
        currentSpeaker = GetSpeakerByName(info[0]);

        DialogueSpeaker speaker = currentSpeaker.GetComponent<DialogueSpeaker>();
        if (speaker != null)
        {
            bubbleIndicator.GetComponent<Image>().color = speaker.bubbleColour;
            bubbleImage.color = speaker.bubbleColour;
            dialogueText.color = speaker.textColour;
        }
        else
        {
            bubbleIndicator.GetComponent<Image>().color = bubbleColour;
            bubbleImage.color = bubbleColour;
            dialogueText.color = textColour;
            foreach (TMP_Text optionText in optionTexts)
            {
                optionText.color = optionsColour;
            }
        }
    }

    public void ResetTextSpeed()
    {
        DUI.textSpeed = textSpeed;
    }


    #region OneOffs - probs bad practice but quick

    public void SetupEvents()
    {

        dialogueRunner.AddCommandHandler("SetSpeaker", SetSpeakerInfo);
        dialogueRunner.AddCommandHandler("MothFlyAway", MothFriendFlyAway);
    }

    public void MothFriendFlyAway(string[] info)
    {
        if (currentSpeaker.name == "MothFriend")
        {
            currentSpeaker.GetComponent<MothFriendStart>().FlyAway();
        }
    }

    #endregion OneOffs - probs bad practice but quick


    #endregion Functions Called By YarnSpinner Classes
}

