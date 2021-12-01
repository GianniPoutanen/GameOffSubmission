using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBehaviour : MonoBehaviour
{

    public CharacterController controller;

    [Header("Movement")]
    public float walkSpeed = 6f;
    public float runSpeed = 6f;
    public float jumpHeight;
    public float doubleJumpHeight;
    public float turnSmoothTime = 0.1f;
    [Header("Gravity Scaling")]
    public float gravity = -9.81f;
    [Range(1f, 10f)]
    public float gravityFactor;

    float turnSmoothVelocity;

    [Header("Ground Checking")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Jumping")]
    public int maxJumps = 1;
    public float jumpTimeBuffer;
    public float slideFriction = 0.3f;
    float timeSpentJumping;

    public Animator animator;

    int currentJumps = 0;
    Vector3 velocity;
    public bool isGrounded = false;
    bool isRunning = false;
    bool isInAir = false;

    [Header("Step Sounds")]
    public AudioClip[] stepSounds;

    [Header("Stamana Variables")]
    public StamanaContainer stamanaContainer;
    public float stamanaDrainSpeed = 1;
    public float stamanaRechargeTime;
    private float stamanaChargeTimer;
    private float stamanaRechargeSpeed = 1;
    private Vector3 hitNormal;
    private bool isSliding;

    private void Start()
    {
        GameAssets.Instance.playerCharacter = this.gameObject;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        hitNormal = hit.normal;
    }

    private void Update()
    {
        int actualMaxJumps = maxJumps;
        if (UnlocksManager.Instance.unlockedSkills.Contains(UnlocksManager.eSkillType.DoubleJump))
        {
            actualMaxJumps = maxJumps + 1;
        }
        if (actualMaxJumps != stamanaContainer.numStamanaBars)
        {
            stamanaContainer.numStamanaBars = actualMaxJumps;
            stamanaContainer.ResizeStamanaBars();
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        animator.SetBool("IsGrounded", isGrounded);


        if (isInAir)
        {
            timeSpentJumping += Time.deltaTime;
        }

        handleSlope();

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask) &&
            (!isInAir || timeSpentJumping > jumpTimeBuffer) &&
            !isSliding;

        if (isGrounded)
        {
            timeSpentJumping = 0;
            currentJumps = 0;
            isInAir = false;
            if (velocity.y < 0)
            {
                velocity.y = -2f;
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
            if (Input.GetKey(KeyCode.LeftShift) && stamanaContainer.HasStamana())
            {
                isRunning = true;
                controller.Move(moveDir.normalized * runSpeed * Time.deltaTime);
                if (isGrounded)
                    stamanaContainer.UpdateStamana(-stamanaDrainSpeed);
                stamanaChargeTimer = stamanaRechargeTime;

            }
            else
            {
                if (isGrounded)
                {
                    isRunning = false;
                    controller.Move(moveDir.normalized * walkSpeed * Time.deltaTime);
                }
                else if (isRunning)
                {
                    controller.Move(moveDir.normalized * runSpeed * Time.deltaTime);
                }
                else
                {
                    controller.Move(moveDir.normalized * walkSpeed * Time.deltaTime);
                }
            }
        }

        if ((Input.GetButtonDown("Jump") || gameObject.GetComponent<Climb>().isClimbing) && !GameAssets.Instance.dialogueManager.InDialog)
        {
            if (stamanaContainer.HasStamana())//++currentJumps <= maxJumps)
            {
                if (isInAir)
                    animator.Play("DoubleJump");

                gameObject.GetComponent<Climb>().isClimbing = false;
                // Calc Jump height
                if (isInAir)
                    velocity.y = Mathf.Sqrt(doubleJumpHeight * -2 * gravity * gravityFactor);
                else
                    velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity * gravityFactor);

                isInAir = true;
                stamanaContainer.UseUpStamana();
                stamanaChargeTimer = stamanaRechargeTime;
            }
        }

        velocity.y += gravity * gravityFactor * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        animator.SetFloat("velocityY", velocity.y);

        //Sort Animation
        if (isGrounded && direction.magnitude > 0.1f && !GameAssets.Instance.dialogueManager.InDialog)
        {
            animator.SetFloat("Speed", isRunning ? 2f : 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        if (stamanaChargeTimer > 0)
        {
            stamanaChargeTimer -= Time.deltaTime;
        }
        else
        {
            stamanaContainer.UpdateStamana(stamanaRechargeSpeed);
        }
    }

    private void handleSlope()
    {

        if (isSliding)
        {
            if (Vector3.Angle (Vector3.up, hitNormal) > controller.slopeLimit)
            {
                velocity.y += gravity;
                velocity.x += (1f - hitNormal.y) * hitNormal.x * (-gravity - slideFriction);
                velocity.z += (1f - hitNormal.y) * hitNormal.z * (-gravity - slideFriction);
            }
            else
            {
                velocity.y = -2f;
                velocity.x = 0f;
                velocity.z = 0f;
            }
        }

        isSliding = (Vector3.Angle (Vector3.up, hitNormal) > controller.slopeLimit);
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
