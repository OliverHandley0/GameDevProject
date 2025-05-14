using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    public int damageAmount = 10; // Amount of damage to deal to the player on collision

    // Triggered when this GameObject collides with another
    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject; // Reference to the other collider’s GameObject

        // If we hit an object tagged "DestroyB", destroy ourselves
        if (other.CompareTag("DestroyB"))
        {
            Destroy(gameObject);
            return; 
        }

        // If we hit the player, deal damage
        if (other.CompareTag("Player"))
        {
            var healthManager = other.GetComponent<HealthManager>(); // Try to get the player's health component
            if (healthManager != null)
            {
                healthManager.TakeDamage(damageAmount); // Reduce player health
            }
        }
    }
}
