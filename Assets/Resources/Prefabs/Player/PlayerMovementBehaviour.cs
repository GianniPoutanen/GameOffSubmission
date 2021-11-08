using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBehaviour : MonoBehaviour
{
    public CharacterController controller;


    public float speed = 6f;
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
    bool isGrounded = false;


    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -1f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertiacal = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertiacal).normalized;
        if (direction.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity * gravityFactor);
        }


        velocity.y += gravity * gravityFactor * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

    }
}
