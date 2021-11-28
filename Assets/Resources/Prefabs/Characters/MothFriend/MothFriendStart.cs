using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class MothFriendStart : MonoBehaviour
{
    public float str;
    public Quaternion lookRotation;
    public Transform followObject;
    public bool follow;
    public float speedDampiner;

    private void Update()
    {
        if (follow)
        {
            this.transform.position = Vector3.MoveTowards(followObject.position, this.transform.position,
            Vector3.Distance(this.transform.position, followObject.position) / speedDampiner);
            this.transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, str);
        }
    }

    public void FlyAway()
    {
        followObject.GetComponent<PathCreation.Examples.PathFollower>().speed = 10;
        follow = true;
        this.GetComponent<Animator>().SetBool("FlyAway", true);
    }

}
