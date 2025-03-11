using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float speed = 0.1f;
    public float turnSpeed = 2f;
    private Transform target;

    void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position;

            // Adjust target height using Collider bounds if available
            Collider targetCollider = target.GetComponent<Collider>();
            if (targetCollider != null)
            {
                targetPosition = targetCollider.bounds.center;
            }
            else
            {
                targetPosition += new Vector3(0, 0.5f, 0); // Fallback height adjustment
            }

            Vector3 direction = targetPosition - transform.position;
            float step = speed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, turnSpeed * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }
        else
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                SetTarget(player.transform);
            }
        }
    }


    public void SetTarget(Transform newTarget)
    {
        if (newTarget == null)
        {
            Debug.LogError("SetTarget was given a null target!");
            return;
        }

        target = newTarget;
        Debug.Log($"New Target Set: {target.name} at position {target.position}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager playerHealth = other.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10f);
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Block"))
        {
            Destroy(gameObject);
        }
    }
}
