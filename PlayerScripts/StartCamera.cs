using UnityEngine;
using System.Collections;

public class CameraTrigger : MonoBehaviour
{
    public GameObject player;               // Reference to the player GameObject
    public Camera thirdPersonCam;           // The regular third-person camera
    public Camera flythroughCam;            // The camera used for the flythrough
    public Transform[] flythroughPoints;    // Array of points defining the flythrough path
    public float timePerSegment = 2f;       // Duration to move between each flythrough point

    private bool triggered = false;         // Has the trigger been activated already?
    private float spawnTime;                // Time when this object was created

    // Initialize camera states and record creation time
    void Start()
    {
        flythroughCam.enabled = false;      // Ensure flythrough camera is off at start
        spawnTime = Time.time;              // Record the time to delay re-triggering
    }

    // Called when another collider enters this trigger zone
    void OnTriggerEnter(Collider other)
    {
        // Only start flythrough once, for the player, and after a short spawn delay
        if (!triggered && other.gameObject == player && Time.time > spawnTime + 1f)
        {
            triggered = true;               // Prevent further triggers
            StartCoroutine(DoFlythrough()); // Begin the cinematic flythrough
        }
    }

    // Coroutine: smoothly move flythroughCam through each point, then restore third-person view
    IEnumerator DoFlythrough()
    {
        thirdPersonCam.enabled = false;     // Turn off the gameplay camera
        flythroughCam.transform.position = flythroughPoints[0].position; // Jump to first point
        flythroughCam.transform.rotation = flythroughPoints[0].rotation;
        flythroughCam.enabled = true;       // Activate flythrough camera

        // Iterate through each subsequent point in the flythrough path
        for (int i = 1; i < flythroughPoints.Length; i++)
        {
            Vector3 startPos = flythroughCam.transform.position;       // Starting position
            Quaternion startRot = flythroughCam.transform.rotation;    // Starting rotation
            Vector3 endPos = flythroughPoints[i].position;             // Target position
            Quaternion endRot = flythroughPoints[i].rotation;          // Target rotation
            float elapsed = 0f;                                        // Time since segment start

            // Interpolate position and rotation over timePerSegment seconds
            while (elapsed < timePerSegment)
            {
                float t = elapsed / timePerSegment;                    // Normalized time [0,1]
                flythroughCam.transform.position = Vector3.Lerp(startPos, endPos, t);
                flythroughCam.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
                elapsed += Time.deltaTime;                             // Advance time
                yield return null;                                     // Wait until next frame
            }

            // Snap exactly to the endpoint to correct any minor interpolation error
            flythroughCam.transform.position = endPos;
            flythroughCam.transform.rotation = endRot;
        }

        // After completing the path, switch cameras back to gameplay mode
        flythroughCam.enabled = false;
        thirdPersonCam.enabled = true;
    }
}
