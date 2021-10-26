using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Range(0f, 3f)]
    public float zoomLevel;
    private float startingZ;
    public GameObject followObject;

    // Start is called before the first frame update
    void Start()
    {
        startingZ = this.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.localScale.x != zoomLevel)
            this.transform.localScale = new Vector3(zoomLevel, zoomLevel, zoomLevel);
        if (followObject != null)
            this.transform.position = new Vector3(followObject.transform.position.x, followObject.transform.position.y, startingZ);
    }
}
