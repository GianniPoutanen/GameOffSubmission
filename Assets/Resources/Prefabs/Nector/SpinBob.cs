using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinBob : MonoBehaviour
{
    public Transform itemObj;

    [Header("Movement")]
    public float spinSpeed = 80;
    public float bobSize = 0.2f;
    public float bobSpeedDampener = 2;
    private float bobPos = 0;

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = Quaternion.Euler(0,this.transform.rotation.eulerAngles.y + spinSpeed * Time.deltaTime, 0);
        //Calc Bob
        bobPos += Time.deltaTime;
        float bobCalc = (bobSize * Mathf.Cos((2 * Mathf.PI * bobPos) / bobSpeedDampener));
        itemObj.transform.localPosition = new Vector3(0, bobCalc, 0);
    }
}
