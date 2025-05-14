using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public float maxHealth = 100f;           // Maximum health value
    private float currentHealth;             // Current health at runtime

    public Slider healthSlider;              // UI slider displaying health

    public float respawnTime = 3f;           // Delay before respawning after death
    public Transform respawnPoint;           // Location to respawn the player

    private HomingMissile[] missiles;        // All homing missiles to retarget on respawn

    // Initialize health, UI, and cache missiles
    void Start()
    {
        currentHealth = maxHealth;                                 // Start at full health
        missiles = FindObjectsOfType<HomingMissile>();             // Grab all existing missiles
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;                     // Match slider max to health
            healthSlider.value = currentHealth;                    // Show starting health
        }
    }

    // Public method to apply damage and handle death
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;                                   // Subtract damage
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Keep within [0, max]

        if (healthSlider != null)
            healthSlider.value = currentHealth;                    // Update UI

        if (IsDead())                                              // If health dropped to zero
            Die();                                                 // Trigger death
    }

    // Check if health is depleted
    public bool IsDead()
    {
        return currentHealth <= 0f;                                // True when no health remains
    }

    // Disable player, then schedule respawn
    private void Die()
    {
        gameObject.SetActive(false);                               // Hide/disable player
        Invoke(nameof(Respawn), respawnTime);                      // Call Respawn after delay
    }

    // Restore health, reposition, re-enable, and retarget missiles
    private void Respawn()
    {
        currentHealth = maxHealth;                                 // Reset to full health
        if (healthSlider != null)
            healthSlider.value = currentHealth;                    // Reset UI slider

        if (respawnPoint != null)
            transform.position = respawnPoint.position;            // Move to respawn location

        gameObject.SetActive(true);                                // Show/enable player

        foreach (var missile in missiles)                          // Retarget each missile
            missile.SetTarget(transform);
    }
}
