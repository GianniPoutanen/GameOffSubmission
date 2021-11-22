using UnityEngine;

public class Climb : MonoBehaviour
{
    public CharacterController controller;
    public Animation anim;
    public bool isClimbing;
    public float climbSpeed = 3;
    public float rotationSpeed = 5;
    public float reach = 3;
    public LayerMask climbableMask;

    float delta;
    bool isJumping;
    Transform helper;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        helper = new GameObject().transform;
        helper.name = "climb helper";
        if (CheckForClimb())
        {
            StartClimb();
        }
    }

    bool isClimbableObject(GameObject gameObject)
    {
        return (climbableMask == (climbableMask | (1 << gameObject.layer)));
    }

    bool CheckForClimb()
    {
        RaycastHit hit;
        Vector3 originTop = transform.position;
        originTop.y += 1.4f;
        Vector3 originBottom = transform.position;
        originBottom.y -= 0.6f;
        Vector3 dir = transform.forward;
        // check if player is in front of a wall
        if (Physics.Raycast(originTop, dir, out hit, reach) || Physics.Raycast(originBottom, dir, out hit, reach))
        {
            helper.transform.rotation = Quaternion.LookRotation(-hit.normal);
            // check if in front of a climbable layer and the gradient is sufficient for climbing
            return isClimbableObject(hit.transform.gameObject) && helper.transform.localEulerAngles.x < controller.slopeLimit;
        }
        return false;
    }

    void StartClimb()
    {
        isClimbing = true;
        // disable standard movement
        gameObject.GetComponent<PlayerMovementBehaviour>().enabled = false;
    }

    void EndClimb()
    {
        // reenable standard movement when done climbing
        gameObject.GetComponent<PlayerMovementBehaviour>().enabled = true;
    }

    void EndJump()
    {
        isJumping = false;
    }

    void Update()
    {
        delta = Time.deltaTime;
        Tick();
    }

    public void Tick()
    {
        if (isJumping)
        {
            return;
        }
        else if (!isClimbing)
        {
            if (CheckForClimb())
            {
                StartClimb();
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump"))
            {
                // when jumping, bypass regular climbing behaviour for 1 second
                isJumping = true;
                Invoke("EndJump", 1);
                EndClimb();
            }
            else
            {
                Vector3 dir = transform.forward;
                float hor = Input.GetAxis("Horizontal");
                float vert = Input.GetAxis("Vertical");
                float m = Mathf.Abs(hor) + Mathf.Abs(vert);

                Vector3 h = helper.right * hor;
                Vector3 v = helper.up * vert;
                Vector3 moveDir = (h + v).normalized;

                AdjustRotation(moveDir);

                controller.Move(moveDir.normalized * climbSpeed * Time.deltaTime);

                if (!CheckForClimb())
                {
                    EndClimb();
                }
            }
        }
    }

    void HandleClimbRotation(RaycastHit hit)
    {
        helper.rotation = Quaternion.LookRotation(-hit.normal);
        transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * rotationSpeed);
    }

    void AdjustRotation(Vector3 moveDir)
    {
        // shoot ray in direction of movement and rotate to face the hit object
        Vector3 origin = transform.position;
        Vector3 dir = moveDir;
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, reach))
        {
            if (isClimbableObject(hit.transform.gameObject))
            {
                HandleClimbRotation(hit);
            }
        }

        // shoot ray forward and rotate to face the hit object
        origin += moveDir * reach;
        dir = helper.forward;
        float dis2 = 0.5f;
        if (Physics.Raycast(origin, dir, out hit, reach))
        {
            if (isClimbableObject(hit.transform.gameObject))
            {
                HandleClimbRotation(hit);
            }
        }

        // shoot ray up and rotate to face the hit object if angle is smaller then a given threshold
        origin += dir * dis2;
        dir = -Vector3.up;
        if (Physics.Raycast(origin, dir, out hit, dis2))
        {
            float angle = Vector3.Angle(-helper.up, hit.normal);
            if (isClimbableObject(hit.transform.gameObject) && angle < 40)
            {
                HandleClimbRotation(hit);
            }
        }
    }
}
