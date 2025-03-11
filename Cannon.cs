using UnityEngine;

public class CannonWithHomingMissile : MonoBehaviour
{
    public GameObject missilePrefab; // Prefab for the homing missile
    public Transform firePoint; // The point where the missile is fired from
    private Transform playerTarget; // Reference to the player's position

    public float fireRate = 1f; // Time interval between missile fires (in seconds)
    private float nextFireTime = 0f; // Time tracking for the next missile fire
    public float fireRange = 100f; // Maximum distance to fire a missile

    void Start()
    {
        // Get the initial target (player) from the HealthManager
        HealthManager player = FindObjectOfType<HealthManager>();
        if (player != null)
        {
            playerTarget = player.transform;
        }
    }

    void Update()
    {
        if (playerTarget != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            // Only fire if the player is within range and the cooldown is ready
            if (distanceToPlayer <= fireRange && Time.time >= nextFireTime)
            {
                Debug.Log($"Firing missile! Player distance: {distanceToPlayer:F2}");
                FireMissile();
                nextFireTime = Time.time + fireRate; // Set the next fire time
            }
        }
    }

    public void FireMissile()
    {
        // Instantiate a missile at the fire point and set its target to the player
        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
        HomingMissile missileScript = missile.GetComponent<HomingMissile>();
        missileScript.SetTarget(playerTarget); // Set the player as the missile's target
    }

    // Method to update the cannon's target (used after player respawn)
    public void SetTarget(Transform newTarget)
    {
        playerTarget = newTarget; // Update the cannon's target to the new player
    }
}
