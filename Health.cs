using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public float maxHealth = 100f; // Maximum health of the character
    private float currentHealth;

    public Slider healthSlider; // Reference to the UI Slider (health bar)

    public float respawnTime = 3f; // Time before player respawns
    public Transform respawnPoint; // The point where the player will respawn

    private HomingMissile[] missiles; // Array to store all missiles targeting the player

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        missiles = FindObjectsOfType<HomingMissile>(); // Get all homing missiles in the scene
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (IsDead())
        {
            Die();
        }
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    private void Die()
    {
        gameObject.SetActive(false);
        Invoke("Respawn", respawnTime);
    }

    private void Respawn()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }

        gameObject.SetActive(true);

        // After respawn, update all missiles to target the new player
        foreach (var missile in missiles)
        {
            missile.SetTarget(transform); // Set the missile's target to the new player
        }
    }
}
