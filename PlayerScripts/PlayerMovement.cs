using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Move Settings")]
    public float velocity = 5f;            // Base movement speed
    public float sprintAddition = 3.5f;    // Extra speed when sprinting

    [Header("Jump Settings")]
    public float jumpForce = 18f;          // Initial upward force for jumps
    public float jumpTime = 0.85f;         // Duration over which jump force applies
    public float gravity = 9.8f;           // Downward acceleration

    [Header("Fall Adjustment")]
    [Range(0f, 1f)]
    public float fallMultiplier = 0.6f;    // Gravity scale when falling

    [Header("Dash Settings")]
    public bool dashEnabled = true;        // Enable or disable dash ability
    public float dashSpeed = 15f;          // Speed during dash
    public float dashDuration = 0.2f;      // How long dash lasts
    public float dashCooldown = 1f;        // Time before dash can be used again

    [Header("Dash VFX")]
    public ParticleSystem dashParticle;    // Particle effect for dashing

    [Header("Audio Clips")]
    public AudioClip walkClip;             // Footstep sound when walking
    public AudioClip runClip;              // Footstep sound when sprinting
    public AudioClip jumpClip;             // Sound played on jump
    public float footstepInterval = 0.5f;  // Time between footstep sounds

    [Header("Jump VFX")]
    public ParticleSystem jumpParticle;    // Particle effect on jump

    [Header("UI / Double Jump")]
    public bool isUIActive = false;        // If true, input and camera are locked for UI

    // Internal references and state
    private CharacterController cc;
    private Animator animator;
    private AudioSource audioSource;

    private float inputHorizontal;
    private float inputVertical;
    private bool inputJump;
    private bool inputSprint;
    private bool inputCrouch;

    private bool isCrouching = false;
    private bool isSprinting = false;
    private bool isJumping = false;
    private bool canDoubleJump = false;

    private float directionY = 0f;         // Vertical movement component
    private float jumpElapsedTime = 0f;    // Timer for jump arc

    private float footstepTimer = 0f;
    private enum WalkState { Idle, Walking, Sprinting }
    private WalkState currentState = WalkState.Idle;

    // Dash timers and direction
    private float dashTimeLeft = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;

    // Cache components and validate setup
    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Handle input, UI lock, dash initiation, jump triggering, and animations
    void Update()
    {
        // Lock or unlock cursor based on UI state
        if (isUIActive)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Read player inputs
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetButtonDown("Jump");
        inputSprint = Input.GetAxis("Fire3") == 1f;
        inputCrouch = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.JoystickButton1);

        // Toggle crouch state
        if (inputCrouch)
            isCrouching = !isCrouching;

        // Update dash cooldown
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        // Start dash if eligible
        if (dashEnabled && dashTimeLeft <= 0f && Input.GetKeyDown(KeyCode.E) && dashCooldownTimer <= 0f)
        {
            dashTimeLeft = dashDuration;
            dashDirection = transform.forward;
            animator?.SetTrigger("dash");
            dashParticle?.Play();
        }

        // Countdown dash duration
        if (dashTimeLeft > 0f)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f)
                dashCooldownTimer = dashCooldown;
        }

        // Update animator booleans for grounded states
        if (cc.isGrounded)
        {
            animator?.SetBool("crouch", isCrouching);
            bool moving = cc.velocity.magnitude > 0.9f;
            animator?.SetBool("run", moving);
            isSprinting = moving && inputSprint;
            animator?.SetBool("sprint", isSprinting);
        }
        animator?.SetBool("air", !cc.isGrounded);

        // Play footstep sounds if needed
        HandleFootsteps();

        // Jump logic: initial jump or double jump
        if (cc.isGrounded)
        {
            if (inputJump)
                PerformJump();
        }
        else if (inputJump && canDoubleJump)
        {
            isJumping = true;
            jumpElapsedTime = 0f;
            canDoubleJump = false;
            directionY = jumpForce * Time.deltaTime;
            animator?.SetTrigger("doubleJump");
            audioSource?.PlayOneShot(jumpClip);
            jumpParticle?.Play();
        }

        // Detect if head hits ceiling to cancel jump
        HeadHittingDetect();
    }

    // Apply physics-based movement here
    void FixedUpdate()
    {
        if (isUIActive) return;

        // If currently dashing, override normal movement
        if (dashEnabled && dashTimeLeft > 0f)
        {
            cc.Move(dashDirection * dashSpeed * Time.deltaTime);
            return;
        }

        // Calculate horizontal movement with sprint or crouch modifiers
        float add = isSprinting ? sprintAddition : 0f;
        if (isCrouching) add = -velocity * 0.5f;

        float dx = inputHorizontal * (velocity + add) * Time.deltaTime;
        float dz = inputVertical * (velocity + add) * Time.deltaTime;

        // Reset vertical direction when grounded
        if (cc.isGrounded && directionY < 0f)
            directionY = -1f;

        // Compute jump arc over jumpTime
        if (isJumping)
        {
            if (isSprinting)
                directionY += jumpForce * 0.2f;
            directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime;
            jumpElapsedTime += Time.deltaTime;
            if (jumpElapsedTime >= jumpTime)
            {
                isJumping = false;
                jumpElapsedTime = 0f;
            }
        }

        // Reduce upward momentum when crouching
        if (isCrouching)
            directionY *= 0.75f;

        // Apply gravity with fall multiplier
        if (directionY < 0f)
            directionY -= gravity * fallMultiplier * Time.deltaTime;
        else
            directionY -= gravity * Time.deltaTime;

        // Build move vector relative to camera orientation
        Vector3 forward = Camera.main.transform.forward; forward.y = 0f; forward.Normalize();
        Vector3 right   = Camera.main.transform.right;   right.y = 0f; right.Normalize();
        Vector3 move    = right * dx + forward * dz + Vector3.up * directionY;

        // Rotate character toward movement direction if moving
        if (dx != 0f || dz != 0f)
        {
            float angle = Mathf.Atan2(dx, dz) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, angle, 0), 0.15f);
        }

        cc.Move(move);
    }

    // Manages footstep sound playback based on movement state
    private void HandleFootsteps()
    {
        bool moving = cc.isGrounded && cc.velocity.magnitude > 0.9f && !isJumping;
        WalkState newState = moving
            ? (isSprinting ? WalkState.Sprinting : WalkState.Walking)
            : WalkState.Idle;

        if (newState != currentState)
        {
            footstepTimer = footstepInterval;
            currentState = newState;
        }

        if (currentState != WalkState.Idle)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                footstepTimer = footstepInterval;
                audioSource?.PlayOneShot(currentState == WalkState.Sprinting ? runClip : walkClip);
            }
        }
    }

    // Initiates the first jump
    private void PerformJump()
    {
        isJumping = true;
        jumpElapsedTime = 0f;
        canDoubleJump = true;
        directionY = jumpForce * Time.deltaTime;
        animator?.SetTrigger("jump");
        audioSource?.PlayOneShot(jumpClip);
        jumpParticle?.Play();
    }

    // Cancel jump if head collides with ceiling
    private void HeadHittingDetect()
    {
        Vector3 ccCenter = transform.TransformPoint(cc.center);
        float rayDist = (cc.height / 2f) * 1.1f;
        if (Physics.Raycast(ccCenter, Vector3.up, rayDist))
        {
            isJumping = false;
            jumpElapsedTime = 0f;
        }
    }
}
