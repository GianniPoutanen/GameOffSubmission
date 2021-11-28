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
    public int maxJumps = 1;
    public float jumpTimeBuffer;
    float timeSpentJumping;

    public Animator animator;

    int currentJumps = 0;
    Vector3 velocity;
    public bool isGrounded = false;
    bool isRunning = false;
    bool isInAir = false;

    [Header("Step Sounds")]
    public AudioClip[] stepSounds;
    private void Start()
    {
        GameAssets.Instance.playerCharacter = this.gameObject;
    }


    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        animator.SetBool("IsGrounded", isGrounded);


        if (isInAir)
        {
            timeSpentJumping += Time.deltaTime;
        }
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask) && (!isInAir || timeSpentJumping > jumpTimeBuffer);
        if (isGrounded)
        {
            timeSpentJumping = 0;
            currentJumps = 0;
            isInAir = false;
            if (velocity.y < 0)
            {
                velocity.y = -0.5f;
            }
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
            if (++currentJumps <= maxJumps)
            {
                if (isInAir)
                    animator.Play("DoubleJump");

                gameObject.GetComponent<Climb>().isClimbing = false;
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity * gravityFactor);
                isInAir = true;
            }
        }

        velocity.y += gravity * gravityFactor * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        animator.SetFloat("velocityY", velocity.y);

        //Sort Animation
        if (isGrounded && direction.magnitude > 0.1f)
        {
            animator.SetFloat("Speed", isRunning ? 2f : 1f);
        }
        else{
            animator.SetFloat("Speed", 0f);

        }


        animator.SetBool("IsClimbing", gameObject.GetComponent<Climb>().isClimbing);
        if (gameObject.GetComponent<Climb>().isClimbing)
        {
            if (Input.GetKey(KeyCode.W)) 
            {
                animator.SetFloat("ClimbYVelocity", 1f);
            }
            else if (Input.GetKey(KeyCode.S)) 
            {
                animator.SetFloat("ClimbYVelocity", -1f);
            }
            else 
            {
                animator.SetFloat("ClimbYVelocity", 0f);
            }
        }
    }


    private int stepSoundCounter = 0;
    public void PlayStepSound()
    {
        if (stepSounds.Length > 0)
        {
            int randStepIndex = Random.Range(0, stepSounds.Length);
            //MakeSureRandom
            stepSoundCounter = randStepIndex == stepSoundCounter ? (randStepIndex + 1) % stepSounds.Length : randStepIndex;
            SoundManager.PlaySound(stepSounds[stepSoundCounter]);
            Debug.Log(stepSoundCounter);
        }
    }
}
