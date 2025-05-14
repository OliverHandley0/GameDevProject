using UnityEngine;
using System.Collections;

public class RepeatedCloner : MonoBehaviour
{
    public GameObject prefabToClone;      // Prefab to instantiate repeatedly
    public float spawnInterval = 1f;      // Time between spawns in seconds

    private Coroutine _spawnRoutine;      // Reference to the running spawn coroutine

    // Called on the first frame; starts spawning if prefab is set
    private void Start()
    {
        if (prefabToClone == null)
        {
            enabled = false;              // Disable this script if no prefab assigned
            return;
        }

        _spawnRoutine = StartCoroutine(SpawnRoutine());  // Begin repeating spawn loop
    }

    // Coroutine that spawns the prefab at regular intervals
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            Instantiate(prefabToClone, transform.position, Quaternion.identity); // Create clone
            yield return new WaitForSeconds(spawnInterval);                      // Wait before next
        }
    }

    // Stop the spawn coroutine when this component is disabled
    private void OnDisable()
    {
        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);
    }
}
