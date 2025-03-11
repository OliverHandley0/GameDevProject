using UnityEditor.VersionControl;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [Tooltip("Speed ​​at which the character moves. It is not affected by gravity or jumping.")]
    public float velocity = 5f; // Basic movement speed
    [Tooltip("This value is added to the speed value while the character is sprinting.")]
    public float sprintAdittion = 3.5f; // Speed boost when sprinting
    [Tooltip("The higher the value, the higher the character will jump.")]
    public float jumpForce = 18f; // Force of the jump
    [Tooltip("Stay in the air. The higher the value, the longer the character floats before falling.")]
    public float jumpTime = 0.85f; // Time spent in air while jumping
    [Space]
    [Tooltip("Force that pulls the player down. Changing this value causes all movement, jumping and falling to be changed as well.")]
    public float gravity = 9.8f; // Gravity pulling the player down

    float jumpElapsedTime = 0; // Timer for jump duration

    bool isJumping = false; // Check if character is jumping
    bool isSprinting = false; // Check if character is sprinting
    bool isCrouching = false; // Check if character is crouching

    float inputHorizontal; // Horizontal movement input (A/D or Left Stick)
    float inputVertical; // Vertical movement input (W/S or Left Stick)
    bool inputJump; // Jump input (Space or Button)
    bool inputCrouch; // Crouch input (Left Control or Button)
    bool inputSprint; // Sprint input (Shift or Button)

    Animator animator; // Animator component to control animations
    CharacterController cc; // CharacterController component for movement

    void Start()
    {
        cc = GetComponent<CharacterController>(); // Get the CharacterController
        animator = GetComponent<Animator>(); // Get the Animator

        if (animator == null)
            Debug.LogWarning("Hey buddy, you don't have the Animator component in your player. Without it, the animations won't work.");
    }

    void Update()
    {
        inputHorizontal = Input.GetAxis("Horizontal"); // Get horizontal input
        inputVertical = Input.GetAxis("Vertical"); // Get vertical input
        inputJump = Input.GetAxis("Jump") == 1f; // Check if jump button is pressed
        inputSprint = Input.GetAxis("Fire3") == 1f; // Check if sprint button is pressed
        inputCrouch = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.JoystickButton1); // Check if crouch button is pressed

        if (inputCrouch)
            isCrouching = !isCrouching; // Toggle crouch

        if (cc.isGrounded && animator != null)
        {
            animator.SetBool("crouch", isCrouching); // Update crouch animation

            float minimumSpeed = 0.9f;
            animator.SetBool("run", cc.velocity.magnitude > minimumSpeed); // Update run animation

            isSprinting = cc.velocity.magnitude > minimumSpeed && inputSprint; // Check if sprinting
            animator.SetBool("sprint", isSprinting); // Update sprint animation
        }

        if (animator != null)
            animator.SetBool("air", cc.isGrounded == false); // Update air animation

        if (inputJump && cc.isGrounded) // If jump is pressed and player is grounded
        {
            isJumping = true; // Start jumping
        }

        HeadHittingDetect(); // Detect if head hits a ceiling
    }

    private void FixedUpdate()
    {
        float velocityAdittion = 0;
        if (isSprinting)
            velocityAdittion = sprintAdittion; // Add sprint speed
        if (isCrouching)
            velocityAdittion = -(velocity * 0.50f); // Reduce speed while crouching

        float directionX = inputHorizontal * (velocity + velocityAdittion) * Time.deltaTime; // Horizontal movement
        float directionZ = inputVertical * (velocity + velocityAdittion) * Time.deltaTime; // Vertical movement
        float directionY = 0; // Vertical movement (jumping/falling)

        if (isJumping) // If the player is jumping
        {
            if (isSprinting)
                directionY += jumpForce * 0.2f; // Add extra force when sprinting

            directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime; // Smooth jump

            jumpElapsedTime += Time.deltaTime; // Increase jump time
            if (jumpElapsedTime >= jumpTime) // If jump time is over
            {
                isJumping = false; // Stop jumping
                jumpElapsedTime = 0; // Reset jump time
            }
        }

        if (isCrouching) // If crouching, reduce vertical speed
        {
            directionY *= 0.75f;
        }

        directionY = directionY - gravity * Time.deltaTime; // Apply gravity

        Vector3 forward = Camera.main.transform.forward; // Get forward direction from the camera
        Vector3 right = Camera.main.transform.right; // Get right direction from the camera

        forward.y = 0; // Ignore vertical component
        right.y = 0; // Ignore vertical component

        forward.Normalize(); // Normalize the forward direction
        right.Normalize(); // Normalize the right direction

        forward = forward * directionZ; // Apply movement in the forward direction
        right = right * directionX; // Apply movement in the right direction

        if (directionX != 0 || directionZ != 0) // If there is any movement
        {
            float angle = Mathf.Atan2(forward.x + right.x, forward.z + right.z) * Mathf.Rad2Deg; // Calculate angle to rotate
            Quaternion rotation = Quaternion.Euler(0, angle, 0); // Create rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f); // Smoothly rotate the character
        }

        Vector3 verticalDirection = Vector3.up * directionY; // Vertical direction (jumping/falling)
        Vector3 horizontalDirection = forward + right; // Horizontal movement

        Vector3 moviment = verticalDirection + horizontalDirection; // Combine vertical and horizontal movement
        cc.Move(moviment); // Move the character
    }

    void HeadHittingDetect()
    {
        float headHitDistance = 1.1f; // Distance to check for head hitting
        Vector3 ccCenter = transform.TransformPoint(cc.center); // Get the center of the character
        float hitCalc = cc.height / 2f * headHitDistance; // Calculate the head hit range

        if (Physics.Raycast(ccCenter, Vector3.up, hitCalc)) // Check if there is an obstacle above the player
        {
            jumpElapsedTime = 0; // Reset jump timer
            isJumping = false; // Stop jumping if head hits something
        }
    }
}
