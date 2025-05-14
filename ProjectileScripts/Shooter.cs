using UnityEngine;
using System.Collections;

public class CannonWithHomingMissile : MonoBehaviour
{
    public GameObject missilePrefab;         // Prefab of the homing missile to spawn
    public Transform firePoint;              // Point from which the missile is fired
    public Transform objectToRotate;         // The part of the cannon that rotates toward target

    public float fireRate = 1f;              // Time in seconds between missile launches
    private float nextFireTime = 0f;         // Timestamp when next missile can be fired
    public float fireRange = 100f;           // Maximum distance to target for firing

    public CharacterAnimator characterAnimator;  // Reference to play animations before firing
    public float rotationSpeed = 5f;         // How quickly the cannon rotates to face the target

    private float currentRotationSpeed;      // Runtime rotation speed (modified during firing)
    private Transform playerTarget;          // The current player target to track
    private bool isAnimationOffsetActive = false;  // Flag to offset rotation while animating

    // Initialize references and defaults
    void Start()
    {
        if (characterAnimator == null)
            characterAnimator = FindObjectOfType<CharacterAnimator>();  // Auto-find animator if not set

        if (objectToRotate == null)
            objectToRotate = transform;  // Default to this transform if none specified

        currentRotationSpeed = rotationSpeed;  // Start with base rotation speed
    }

    // Called once per frame: find target, rotate toward it, and fire when in range
    void Update()
    {
        playerTarget = FindNearestPlayer();  // Look up the closest player each frame

        if (playerTarget != null)
        {
            // Compute flat direction toward the target
            Vector3 direction = playerTarget.position - objectToRotate.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)  // Only rotate if there's meaningful direction
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                if (isAnimationOffsetActive)
                    targetRotation *= Quaternion.Euler(0f, 90f, 0f);  // Apply yaw offset during animation

                // Smoothly interpolate rotation toward the target orientation
                objectToRotate.rotation = Quaternion.Slerp(
                    objectToRotate.rotation,
                    targetRotation,
                    Time.deltaTime * currentRotationSpeed
                );
            }

            // Check distance and rate-limit firing
            float distance = Vector3.Distance(transform.position, playerTarget.position);
            if (distance <= fireRange && Time.time >= nextFireTime)
            {
                StartCoroutine(HandleAnimationAndFire());  // Play prep animation and then fire
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    // Finds the nearest GameObject tagged "Player" in the scene
    private Transform FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform nearest = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                nearest = player.transform;
            }
        }

        return nearest;
    }

    // Handles the pre-fire animation, rotation slowdown, missile launch, then resets state
    private IEnumerator HandleAnimationAndFire()
    {
        isAnimationOffsetActive = true;            // Offset rotation during animation

        yield return new WaitForSeconds(0.3f);     // Short delay before animation starts

        if (characterAnimator != null)
        {
            characterAnimator.PlayWave();          // Trigger wave animation
            Debug.Log("Wave animation started.");
        }

        yield return new WaitForSeconds(3f);       // Wait for animation to play

        currentRotationSpeed = rotationSpeed / 4f; // Slow down rotation for dramatic effect
        FireMissile();                             // Instantiate and launch the missile
        yield return new WaitForSeconds(2f);       // Hold slow rotation for a moment

        isAnimationOffsetActive = false;           // Remove the rotation offset
        currentRotationSpeed = rotationSpeed;      // Restore original rotation speed
    }

    // Instantiates the missile prefab, sets its homing target, and logs launch
    public void FireMissile()
    {
        if (missilePrefab == null || firePoint == null || playerTarget == null)
            return;  // Abort if any critical reference is missing

        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
        HomingMissile homing = missile.GetComponent<HomingMissile>();
        if (homing != null)
            homing.SetTarget(playerTarget);  // Tell the missile which target to home in on

        Debug.Log("Missile launched!");
    }

    // Allows external scripts to override the current target
    public void SetTarget(Transform newTarget)
    {
        playerTarget = newTarget;
    }
}
