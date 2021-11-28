using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class StartCamZoomIn : MonoBehaviour
{
    public bool opening;
    public Cinemachine.CinemachineVirtualCamera startCam;
    public Cinemachine.CinemachineFreeLook characterCam;
    public Image openShadeInImage;
    public bool startOpenScript = false;

    public bool soundLock = false;
    public bool dialogueLock = false;

    public AudioClip[] stepSounds;

    // Start is called before the first frame update
    void Start()
    {
        if (!opening)
        {
            openShadeInImage.gameObject.SetActive(false);
            startCam.Priority = 1;
            characterCam.m_YAxis.m_InputAxisName = "Mouse Y";
            characterCam.m_XAxis.m_InputAxisName = "Mouse X";
        }
        else
        {
            this.GetComponent<DialogueRunner>().AddCommandHandler("PlayStepSound", PlayStepSound);
            openShadeInImage.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (opening)
        {
            if (openShadeInImage.color.a != 1)
            {
                startCam.Priority = 1;
                openShadeInImage.gameObject.SetActive(false);
            }
            if (Camera.main.transform.position == characterCam.transform.position)
            {
                // Unlock Controls
                characterCam.m_YAxis.m_InputAxisName = "Mouse Y";
                characterCam.m_XAxis.m_InputAxisName = "Mouse X";
            }
        }

        if (!dialogueLock && Input.GetKeyDown(KeyCode.Space))
        {
            this.GetComponent<DialogueRunner>().Dialogue.Continue();
        }
    }

    public void FinishOpeningScript()
    {
        startOpenScript = true;
    }

    public void DialogLockSwitch()
    {
        if (!soundLock)
        dialogueLock = !dialogueLock;
    }

    public void PlayStepSound(string[] empty)
    {
        StartCoroutine("StepSound");
    }

    IEnumerator StepSound()
    {
        soundLock = true;
        dialogueLock = true;
        for (int i = 0; i < 6; i++)
        {
            if (stepSounds.Length > 0)
                SoundManager.PlaySound(stepSounds[Random.Range(0, stepSounds.Length - 1)]);
            yield return new WaitForSeconds(0.2f);
        }
        dialogueLock = false;
    }
}
