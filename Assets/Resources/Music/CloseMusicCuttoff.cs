using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMusicCuttoff : MonoBehaviour
{
    private AudioClip song;
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = this.GetComponent<AudioSource>();
        song = source.clip;
    }

    // Update is called once per frame
    void Update()
    {
        // not doing anything :(
    }
}
