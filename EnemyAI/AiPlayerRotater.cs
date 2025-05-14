using UnityEngine;

public class RotateTowardsNearestPlayer : MonoBehaviour
{
    public float rotationSpeed = 5f;    // Speed at which this object rotates toward the target

    // Called every frame: find the nearest player and rotate smoothly toward them
    void Update()
    {
        GameObject nearestPlayer = FindNearestPlayer();  
        if (nearestPlayer != null)
        {
            Vector3 direction = nearestPlayer.transform.position - transform.position;  // Vector from this object to player
            Quaternion targetRotation = Quaternion.LookRotation(direction);            // Desired rotation to face the player
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                Time.deltaTime * rotationSpeed  // Interpolate toward target based on rotationSpeed
            );
        }
    }

    // Searches all GameObjects tagged "Player" and returns the closest one
    GameObject FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");  // All players in scene
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;   

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(currentPos, player.transform.position);  // Distance to this player
            if (distance < minDistance)
            {
                minDistance = distance;  // Track smallest distance
                nearest = player;        // Remember closest player
            }
        }

        return nearest;  // May be null if no players found
    }
}
