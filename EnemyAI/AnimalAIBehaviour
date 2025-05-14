using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AnimalSimpleAI : MonoBehaviour
{
    // References and settings
    public Transform player;                // Who this AI is targeting
    public float detectionRadius = 100f;    // How close the player must be to start chasing
    public float wanderRadius = 20f;        // How far the AI will wander around
    public float wanderInterval = 5f;       // How often it picks a new wander target
    public float walkSpeed = 3f;            // Speed when just wandering
    public float chaseSpeed = 5f;           // Speed when chasing the player
    public float gravity = -9.81f;          // Gravity applied so it stays grounded

    [Header("Attack Settings")]
    public float attackRange = 1.5f;        // How close to get before hitting
    public float damage = 10f;              // Damage per attack
    public float attackCooldown = 1f;       // Time between hits

    [Header("Audio")]
    public AudioSource audioSource;         // Where to play sounds
    public AudioClip damageSound;          // Sound to play when it hits

    // Internal state
    private CharacterController cc;         // For moving the character
    private Animator animator;              // For playing animations
    private Vector3 targetPos;              // Current wander destination
    private float wanderTimer;              // Timer since last wander pick
    private Vector3 verticalVelocity;       // Tracks falling velocity
    private float lastAttackTime;           // Last time we hit the player

    // Two simple states: wandering or attacking
    private enum State { Walk, Attack }
    private State currentState = State.Walk;

    void Start()
    {
        // Cache components
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Pick an initial wander spot and reset attack timer
        ChooseNewWanderTarget();
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        // Measure distance to player
        float dist = Vector3.Distance(transform.position, player.position);

        // Apply gravity if grounded
        if (cc.isGrounded && verticalVelocity.y < 0)
            verticalVelocity.y = -1f;
        verticalVelocity.y += gravity * Time.deltaTime;

        // State machine: either wandering or attacking
        switch (currentState)
        {
            case State.Walk:
                HandleWander(dist);
                break;
            case State.Attack:
                HandleChase(dist);
                break;
        }
    }

    // Wandering behavior
    private void HandleWander(float dist)
    {
        // Time for new wander target or reached current one?
        if (wanderTimer >= wanderInterval || Vector3.Distance(transform.position, targetPos) < 1f)
            ChooseNewWanderTarget();
        wanderTimer += Time.deltaTime;

        // Move towards the wander spot
        MoveAndRotate(targetPos, walkSpeed);

        // Play walking animation
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);

        // If player is close enough, switch to attack mode
        if (dist <= detectionRadius)
            EnterAttack();
    }

    // Chasing/attacking behavior
    private void HandleChase(float dist)
    {
        // Move towards the player
        MoveAndRotate(player.position, chaseSpeed);

        // Play attacking animation
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);

        // If player runs away, go back to wandering
        if (dist > detectionRadius)
            ExitAttack();
    }

    // Pick a random point to wander to
    private void ChooseNewWanderTarget()
    {
        Vector2 rand = Random.insideUnitCircle * wanderRadius;
        targetPos = transform.position + new Vector3(rand.x, 0, rand.y);
        wanderTimer = 0f;
    }

    // Handles actual movement and turning
    private void MoveAndRotate(Vector3 destination, float speed)
    {
        Vector3 dir = destination - transform.position;
        dir.y = 0; // ignore vertical when rotating
        if (dir.magnitude > 0.1f)
        {
            // Calculate motion including gravity
            Vector3 horizontal = dir.normalized * speed;
            Vector3 motion = (horizontal + verticalVelocity) * Time.deltaTime;
            cc.Move(motion);

            // Smoothly turn to face movement direction
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir.normalized),
                Time.deltaTime * 5f
            );
        }
        else
        {
            // Just apply gravity if we're basically at the spot
            cc.Move(verticalVelocity * Time.deltaTime);
        }
    }

    // Switch to attack state
    private void EnterAttack()
    {
        currentState = State.Attack;
        wanderTimer = 0f;
    }

    // Switch back to wander state
    private void ExitAttack()
    {
        currentState = State.Walk;
        ChooseNewWanderTarget();
    }

    // Called when something stays in our trigger collider
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Only attack if cooldown has passed
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            HealthManager playerHealth = other.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                // Deal damage
                playerHealth.TakeDamage(damage);

                // Play hit sound if set up
                if (audioSource != null && damageSound != null)
                {
                    audioSource.PlayOneShot(damageSound);
                }
            }
        }
    }
}
