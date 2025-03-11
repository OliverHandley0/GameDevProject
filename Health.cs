using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public Slider healthSlider;

    public float respawnTime = 3f;
    public Transform respawnPoint;

    private HomingMissile[] missiles;

    void Start()
    {
        currentHealth = maxHealth; // Set initial health
        missiles = FindObjectsOfType<HomingMissile>(); // Find all homing missiles
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth; // Set slider max value
            healthSlider.value = currentHealth; // Set slider to current health
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // Reduce health by damage
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Ensure health is within bounds

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth; // Update slider
        }

        if (IsDead()) // Check if player is dead
        {
            Die();
        }
    }

    public bool IsDead()
    {
        return currentHealth <= 0; // Return true if health is zero or less
    }

    private void Die()
    {
        gameObject.SetActive(false); // Disable the player object
        Invoke("Respawn", respawnTime); // Start respawn after delay
    }

    private void Respawn()
    {
        currentHealth = maxHealth; // Reset health to max
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth; // Reset slider to max health
        }

        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position; // Move player to respawn point
        }

        gameObject.SetActive(true); // Enable player object

        foreach (var missile in missiles) // Update missiles to target player again
        {
            missile.SetTarget(transform);
        }
    }
}
