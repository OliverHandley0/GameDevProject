using UnityEngine;

public class CannonWithHomingMissile : MonoBehaviour
{
    public GameObject missilePrefab; // Missile prefab to be fired
    public Transform firePoint; // Where the missile will be fired from
    private Transform playerTarget; // The target (player) for the missile

    public float fireRate = 1f; // Time between missile fires
    private float nextFireTime = 0f; // Time when the next missile can be fired
    public float fireRange = 100f; // Range within which the cannon can fire

    void Start()
    {
        HealthManager player = FindObjectOfType<HealthManager>(); // Find the player
        if (player != null)
        {
            playerTarget = player.transform; // Set player as the target
        }
    }

    void Update()
    {
        if (playerTarget != null) // If player target is assigned
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position); // Calculate distance to player
            if (distanceToPlayer <= fireRange && Time.time >= nextFireTime) // Check if within range and can fire
            {
                Debug.Log($"Firing missile! Player distance: {distanceToPlayer:F2}"); // Log missile firing
                FireMissile(); // Fire the missile
                nextFireTime = Time.time + fireRate; // Update next fire time
            }
        }
    }

    public void FireMissile()
    {
        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation); // Create missile
        HomingMissile missileScript = missile.GetComponent<HomingMissile>(); // Get missile script
        missileScript.SetTarget(playerTarget); // Set missile target to player
    }

    public void SetTarget(Transform newTarget)
    {
        playerTarget = newTarget; // Change target to a new one
    }
}
