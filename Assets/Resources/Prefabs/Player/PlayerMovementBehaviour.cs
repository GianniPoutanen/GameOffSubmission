using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBehaviour : MonoBehaviour
{
    public CharacterController controller;

    public float walkSpeed = 6f;
    public float runSpeed = 6f;
    public float jumpHeight;
    public float gravity = -9.81f;
    [Range(1f, 10f)]
    public float gravityFactor;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;


    Vector3 velocity;
    public bool isGrounded = false;
    bool isRunning = false;
    bool isInAir = false;

    private void Start()
    {
        GameAssets.Instance.playerCharacter = this.gameObject;
    }


    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -0.5f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertiacal = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertiacal).normalized;
        if (direction.magnitude > 0.1f && !GameAssets.Instance.dialogueManager.InDialog)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isRunning = true;
                controller.Move(moveDir.normalized * runSpeed * Time.deltaTime);
            }
            else
            {
                isRunning = false;
                controller.Move(moveDir.normalized * walkSpeed * Time.deltaTime);
            }
        }

        if ((Input.GetButtonDown("Jump") || gameObject.GetComponent<Climb>().isClimbing) && !GameAssets.Instance.dialogueManager.InDialog)
        {
            gameObject.GetComponent<Climb>().isClimbing = false;
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity * gravityFactor);
            isInAir = true;
        }

        velocity.y += gravity * gravityFactor * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
}
