using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Move Settings")]
    public float velocity = 5f;                  // Base movement speed
    public float sprintAddition = 3.5f;          // Extra speed added when sprinting

    [Header("Jump Settings")]
    public float jumpForce = 18f;                // Initial force applied when jumping
    public float jumpTime = 0.85f;               // Duration of upward force during jump
    public float gravity = 9.8f;                 // Gravity strength applied when falling

    [Header("Fall Adjustment")]
    [Range(0f, 1f)]
    public float fallMultiplier = 0.6f;          // Controls faster fall speed when falling

    [Header("Dash Settings")]
    public bool dashEnabled = true;              // Enables or disables dash mechanic
    public float dashSpeed = 15f;                // Speed applied during dash
    public float dashDuration = 0.2f;            // How long dash lasts in seconds
    public float dashCooldown = 1f;              // Cooldown before next dash is allowed

    [Header("Dash VFX")]
    public ParticleSystem dashParticle;          // Particle system played during dash

    [Header("Audio Clips")]
    public AudioClip walkClip;                   // Footstep sound for walking
    public AudioClip runClip;                    // Footstep sound for running
    public AudioClip jumpClip;                   // Sound played when jumping
    public float footstepInterval = 0.5f;        // Time between footstep sounds

    [Header("Jump VFX")]
    public ParticleSystem jumpParticle;          // Particle effect played when jumping

    [Header("UI / Double Jump")]
    public bool isUIActive = false;              // Determines if UI is active, disables control if true

    // Internal state
    private CharacterController cc;              // Reference to the character controller
    private Animator animator;                   // Reference to the animator
    private AudioSource audioSource;             // Reference to the audio source

    private float inputHorizontal;               // Raw horizontal input axis
    private float inputVertical;                 // Raw vertical input axis
    private bool inputJump;                      // True if jump button was pressed this frame
    private bool inputSprint;                    // True if sprint button is held
    private bool inputCrouch;                    // True if crouch button was pressed this frame

    private bool isCrouching = false;            // True if currently crouching
    private bool isSprinting = false;            // True if currently sprinting
    private bool isJumping = false;              // True if currently jumping
    private bool canDoubleJump = false;          // True if allowed to perform a double jump

    private float directionY = 0f;               // Vertical movement delta (used for jumping/falling)
    private float jumpElapsedTime = 0f;          // Tracks time spent during current jump

    private float footstepTimer = 0f;            // Timer between footstep sounds
    private enum WalkState { Idle, Walking, Sprinting } // State enum for footsteps
    private WalkState currentState = WalkState.Idle;   // Current movement state for footsteps

    // Dash state
    private float dashTimeLeft = 0f;             // Remaining time for active dash
    private float dashCooldownTimer = 0f;        // Time left before another dash is allowed
    private Vector3 dashDirection;               // Direction in which the dash is applied


    void Start()
    {
        // Cache required components
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Warn if key components or particles are missing
        if (animator == null) Debug.LogWarning("Animator missing.");
        if (audioSource == null) Debug.LogWarning("AudioSource missing.");
        if (jumpParticle == null) Debug.LogWarning("Jump particle not assigned.");
        if (dashParticle == null) Debug.LogWarning("Dash particle not assigned.");
    }

    void Update()
    {
        // Lock or unlock cursor if UI is active
        if (isUIActive)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Capture player input
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

        // Handle dash start
        if (dashEnabled && dashTimeLeft <= 0f && Input.GetKeyDown(KeyCode.E) && dashCooldownTimer <= 0f)
        {
            dashTimeLeft = dashDuration;
            dashDirection = transform.forward;
            animator?.SetTrigger("dash");
            PlayDashVFX();
        }

        // Count down dash duration
        if (dashTimeLeft > 0f)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f)
                dashCooldownTimer = dashCooldown;
        }

        // Handle grounded movement animation logic
        if (cc.isGrounded && animator != null)
        {
            animator.SetBool("crouch", isCrouching);
            bool moving = cc.velocity.magnitude > 0.9f;
            animator.SetBool("run", moving);
            isSprinting = moving && inputSprint;
            animator.SetBool("sprint", isSprinting);
        }
        animator?.SetBool("air", !cc.isGrounded);

        // Footstep sound logic
        HandleFootsteps();

        // Handle jump or double jump
        if (cc.isGrounded)
        {
            if (inputJump)
                PerformJump();
        }
        else if (inputJump && canDoubleJump)
        {
            // Perform double jump
            isJumping = true;
            jumpElapsedTime = 0f;
            canDoubleJump = false;
            directionY = jumpForce * Time.deltaTime;
            animator?.SetTrigger("doubleJump");
            PlayClipOneShot(jumpClip);
            PlayJumpVFX();
        }

        // Detect head collision to cancel jump
        HeadHittingDetect();
    }

    void FixedUpdate()
    {
        if (isUIActive) return;

        // If dashing, override all other movement
        if (dashEnabled && dashTimeLeft > 0f)
        {
            cc.Move(dashDirection * dashSpeed * Time.deltaTime);
            return;
        }

        // Apply movement speed modifiers
        float add = isSprinting ? sprintAddition : 0f;
        if (isCrouching) add = -velocity * 0.5f;

        float dirX = inputHorizontal * (velocity + add) * Time.deltaTime;
        float dirZ = inputVertical * (velocity + add) * Time.deltaTime;

        // Reset falling when grounded
        if (cc.isGrounded && directionY < 0f)
            directionY = -1f;

        // Apply jump arc over time
        if (isJumping)
        {
            if (isSprinting)
                directionY += jumpForce * 0.2f;

            directionY = Mathf.SmoothStep(
                jumpForce,
                jumpForce * 0.30f,
                jumpElapsedTime / jumpTime
            ) * Time.deltaTime;

            jumpElapsedTime += Time.deltaTime;
            if (jumpElapsedTime >= jumpTime)
            {
                isJumping = false;
                jumpElapsedTime = 0f;
            }
        }

        // Reduce vertical speed when crouching
        if (isCrouching)
            directionY *= 0.75f;

        // Apply gravity with fall multiplier
        if (directionY < 0f)
            directionY -= gravity * fallMultiplier * Time.deltaTime;
        else
            directionY -= gravity * Time.deltaTime;

        // Calculate movement direction relative to camera
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 hMove = right * dirX;
        Vector3 vMove = forward * dirZ;

        // Rotate character toward movement direction
        if (dirX != 0f || dirZ != 0f)
        {
            float angle = Mathf.Atan2(hMove.x + vMove.x, hMove.z + vMove.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, angle, 0), 0.15f);
        }

        // Apply final motion vector
        Vector3 motion = (hMove + vMove) + Vector3.up * directionY;
        cc.Move(motion);
    }

    private void HandleFootsteps()
    {
        // Determine if player is moving on the ground
        bool isMoving = cc.isGrounded && cc.velocity.magnitude > 0.9f && !isJumping;
        WalkState newState = WalkState.Idle;
        if (isMoving)
            newState = isSprinting ? WalkState.Sprinting : WalkState.Walking;

        // Reset timer when state changes
        if (newState != currentState)
        {
            footstepTimer = footstepInterval;
            currentState = newState;
        }

        // Play footstep sound periodically
        if (currentState != WalkState.Idle)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                footstepTimer = footstepInterval;
                var clip = (currentState == WalkState.Sprinting) ? runClip : walkClip;
                PlayClipOneShot(clip);
            }
        }
    }

    private void PerformJump()
    {
        // Begin jump sequence
        isJumping = true;
        jumpElapsedTime = 0f;
        canDoubleJump = true;
        directionY = jumpForce * Time.deltaTime;
        animator?.SetTrigger("jump");
        PlayClipOneShot(jumpClip);
        PlayJumpVFX();
    }

    private void PlayClipOneShot(AudioClip clip)
    {
        // Play single audio clip
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    private void PlayJumpVFX()
    {
        // Trigger jump particle effect
        if (jumpParticle != null)
            jumpParticle.Play();
    }

    private void PlayDashVFX()
    {
        // Trigger dash particle effect
        if (dashParticle != null)
            dashParticle.Play();
    }

    private void HeadHittingDetect()
    {
        // Cancel jump if head hits ceiling
        float headHitDistance = 1.1f;
        Vector3 ccCenter = transform.TransformPoint(cc.center);
        float rayDist = (cc.height / 2f) * headHitDistance;
        if (Physics.Raycast(ccCenter, Vector3.up, rayDist))
        {
            jumpElapsedTime = 0f;
            isJumping = false;
        }
    }
}
