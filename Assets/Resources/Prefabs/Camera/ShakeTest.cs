using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeTest : MonoBehaviour
{

    public CameraShake.ShakeProperties testProperties;

    IEnumerator currentShake;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
           FindObjectOfType<CameraShake>().StartShake(testProperties);
        }
    }


}
