using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float speed = 0.1f; // Speed of the missile
    public float turnSpeed = 2f; // Turn speed to follow the target
    public float damage = 10f;
    private Transform target; // Target the missile will follow
    private bool isTracking = true; // Whether the missile is still tracking the target

    public float stopTrackingDistance = -4f; // Distance at which tracking stops
    private Vector3 lastDirection; // The direction the missile was heading when tracking stops
    private Quaternion fixedRotation; // The rotation to keep after stopping tracking

    void Update()
    {
        if (target != null) // If a target is assigned
        {
            // Calculate the distance between the missile and the target
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= stopTrackingDistance && isTracking) // Stop tracking if within the threshold distance
            {
                isTracking = false; // Stop tracking
                lastDirection = transform.forward; // Store the current direction of the missile
                fixedRotation = transform.rotation; // Store the current rotation to fix it later
            }

            if (isTracking)
            {
                Vector3 targetPosition = target.position; // Get the target position

                Collider targetCollider = target.GetComponent<Collider>(); // Check if target has a collider
                if (targetCollider != null)
                {
                    targetPosition = targetCollider.bounds.center; // Use the collider center
                }
                else
                {
                    targetPosition += new Vector3(0, 0.5f, 0); // Slightly adjust position if no collider
                }

                Vector3 direction = targetPosition - transform.position; // Get direction to target
                float step = speed * Time.deltaTime; // Calculate movement step
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, turnSpeed * Time.deltaTime, 0f); // Turn towards target
                transform.rotation = Quaternion.LookRotation(newDirection); // Apply rotation
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, step); // Move towards target
            }
            else
            {
                // Once tracking is stopped, continue moving in the same direction without updating the target
                transform.position += lastDirection * speed * Time.deltaTime;

                // Fix the rotation in the direction it was facing when tracking stopped
                transform.rotation = fixedRotation;
            }
        }
        else
        {
            GameObject player = GameObject.FindWithTag("Player"); // Find player if no target
            if (player != null)
            {
                SetTarget(player.transform); // Set player as target
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        if (newTarget == null) // Check if target is valid
        {
            Debug.LogError("SetTarget was given a null target!"); // Log error if target is null
            return;
        }

        target = newTarget; // Set new target
        isTracking = true; // Resume tracking
        Debug.Log($"New Target Set: {target.name} at position {target.position}"); // Log new target
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if missile hits the player
        {
            HealthManager playerHealth = other.GetComponent<HealthManager>(); // Get player health
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Deal damage to player
                Destroy(gameObject); // Destroy missile
            }
        }
        else if (other.CompareTag("Block")) // Check if missile hits a block
        {
            Destroy(gameObject); // Destroy missile
        }
    }
}
