using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Follower : MonoBehaviour
{
    public float destinationWaitTime;
    private float destinationWaitTimer;
    private Transform currentFollowObject;
    public Transform[] followObjects;
    public DialogueManager dialogueManager;
    public Animator animator;
    public bool isMoving;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        dialogueManager = GameObject.Find("Dialogue").GetComponent<DialogueManager>();
        currentFollowObject = followObjects[Random.Range(0, followObjects.Length - 1)];
    }

    // Update is called once per frame
    void Update()
    {
        if (!dialogueManager.InDialog)
        {
            this.GetComponent<NavMeshAgent>().SetDestination(currentFollowObject.transform.position);
            isMoving = this.GetComponent<NavMeshAgent>().velocity != Vector3.zero;

            if (!isMoving)
            {
                destinationWaitTimer -= Time.deltaTime;
                if (destinationWaitTimer < 0)
                {
                    currentFollowObject = followObjects[Random.Range(0, followObjects.Length - 1)];
                }
            }
            else 
            {
                destinationWaitTimer = destinationWaitTime;
            }


            if (animator != null)
            {
                animator.SetBool("Moving", isMoving);
            }
        }
    }
}
