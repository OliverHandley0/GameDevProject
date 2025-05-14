using UnityEngine;

public class osci : MonoBehaviour
{
    public float damage = 25f; // Damage applied to the player on trigger

    // Called when another collider enters this trigger zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager playerHealth = other.GetComponent<HealthManager>(); // Try to get the player's health component
            if (playerHealth != null)
                playerHealth.TakeDamage(damage); // Deal damage to the player
        }
        else if (other.CompareTag("Block"))
        {
            Destroy(gameObject); // Destroy this object if it hits a block
        }
    }
}
