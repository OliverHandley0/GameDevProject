using UnityEngine;

public class MeshDamager : MonoBehaviour
{
    public float damage = 25f;                 // Damage dealt to the player on contact

    public float speed = 2f;                   // Horizontal movement speed
    public float distance = 2.5f;              // Total distance to travel before reversing

    private Vector3 startPos;                  // Starting position for oscillation
    private bool reversing = false;            // Are we currently returning to start?

    // Record initial position
    void Start()
    {
        startPos = transform.position;
    }

    // Handle back-and-forth movement and continuous Z-axis rotation
    void Update()
    {
        float moveStep = speed * Time.deltaTime;                // How far to move this frame
        float directionSign = Mathf.Sign(distance);             // +1 if forward, -1 if distance is negative

        if (!reversing)
        {
            // Move away from start
            transform.position += Vector3.right * moveStep * directionSign;
            float moved = transform.position.x - startPos.x;
            if (Mathf.Abs(moved) >= Mathf.Abs(distance))
                reversing = true;                              // Start returning once limit reached
        }
        else
        {
            // Move back toward start
            transform.position -= Vector3.right * moveStep * directionSign;
            float moved = transform.position.x - startPos.x;
            if (Mathf.Abs(moved) <= 0.05f)
                reversing = false;                             // Switch to outbound once near start
        }

        // Spin around Z axis for a visual effect
        transform.Rotate(0f, 0f, 360f * Time.deltaTime);
    }

    // Inflict damage on player or destroy self on hitting blocks
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<HealthManager>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }
        else if (other.CompareTag("Block"))
        {
            Destroy(gameObject);
        }
    }
}
