using UnityEngine;

public class Safety : MonoBehaviour
{
    public bool isOutOfBounds;
    public LayerMask outOfBoundsMask;
    public float secondsBetweenCheckpoints = 0.5f;
    public CharacterController controller;

    PlayerMovementBehaviour movementBehaviour;
    float time;
    Vector3 checkpoint;
    bool isTeleporting = false;

    void Start() {
        movementBehaviour = gameObject.GetComponent<PlayerMovementBehaviour>();
        checkpoint = gameObject.transform.position;
    }

    void Update()
    {
        time += Time.deltaTime;
        if (isTeleporting)
        {
            movementBehaviour.enabled = true;
            isTeleporting = false;
        }
        if (isOutOfBounds)
        {
            movementBehaviour.enabled = false;
            gameObject.transform.position = checkpoint;
            isOutOfBounds = false;
            isTeleporting = true;
        }
        // not in water and on the ground, leave a marker every defined number of seconds
        else if (!isOutOfBounds && movementBehaviour.isGrounded && time >= secondsBetweenCheckpoints)
        {
            time = 0;
            checkpoint = gameObject.transform.position;
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (outOfBoundsMask == (outOfBoundsMask | (1 << collider.gameObject.layer)))
        {
            isOutOfBounds = true;
        }
    }
}
