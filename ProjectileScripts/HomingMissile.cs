using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float speed = 0.1f;                // Movement speed of the missile
    public float turnSpeed = 2f;              // How fast the missile turns to follow the target
    public float damage = 10f;                // Damage dealt on impact
    private Transform target;                 // Current target to home in on
    private bool isTracking = true;           // Whether the missile is actively tracking
    public float stopTrackingDistance = -4f;  // Distance threshold at which tracking stops
    private Vector3 lastDirection;            // Direction missile was moving when tracking stopped
    private Quaternion fixedRotation;         // Rotation to maintain after tracking stops

    void Update()
    {
        // Main movement logic: if we have a target, track or coast; otherwise find player
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position); // Distance to target

            // Stop tracking when within the stopTrackingDistance
            if (distanceToTarget <= stopTrackingDistance && isTracking)
            {
                isTracking = false;               // Disable tracking
                lastDirection = transform.forward; // Remember current forward direction
                fixedRotation = transform.rotation; // Remember current rotation
            }

            if (isTracking)
            {
                // Calculate the precise point on the target to aim at
                Vector3 targetPosition = target.position;
                Collider targetCollider = target.GetComponent<Collider>();
                if (targetCollider != null)
                    targetPosition = targetCollider.bounds.center; // Use collider center if available
                else
                    targetPosition += new Vector3(0, 0.5f, 0);       // Slight vertical offset otherwise

                // Turn smoothly toward the target
                Vector3 direction = targetPosition - transform.position; 
                Vector3 newDirection = Vector3.RotateTowards(
                    transform.forward, 
                    direction, 
                    turnSpeed * Time.deltaTime, 
                    0f
                );
                transform.rotation = Quaternion.LookRotation(newDirection);

                // Move forward toward the target
                transform.position = Vector3.MoveTowards(
                    transform.position, 
                    targetPosition, 
                    speed * Time.deltaTime
                );
            }
            else
            {
                // After tracking stops, continue straight in last known direction
                transform.position += lastDirection * speed * Time.deltaTime;
                transform.rotation = fixedRotation; // Keep fixed rotation
            }
        }
        else
        {
            // Automatically assign the player as target if none set
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                SetTarget(player.transform);
        }
    }

    // Assigns a new target and resets tracking state
    public void SetTarget(Transform newTarget)
    {
        if (newTarget == null)
        {
            Debug.LogError("SetTarget was given a null target!");
            return;
        }

        target = newTarget;    // Update target reference
        isTracking = true;     // Resume tracking
        Debug.Log($"New Target Set: {target.name} at position {target.position}");
    }

    // Handle collisions: damage player or self-destruct on blocks
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager playerHealth = other.GetComponent<HealthManager>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage); // Inflict damage on player
            Destroy(gameObject);                 // Destroy missile on hit
        }
        else if (other.CompareTag("Block"))
        {
            Destroy(gameObject);                 // Destroy missile on block collision
        }
    }
}
