using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;   // for Image and UI components

[RequireComponent(typeof(Collider))]
public class OpenAndSceneTrigger : MonoBehaviour
{
    [Header("Flash Settings")]
    public GameObject openFlashPrefab;    // Prefab for the opening flash effect
    public float flashDuration = 10f;     // How long the flash object lives
    public Vector3 flashSpawnPosition = new Vector3(-226f, 51.4f, -15f); // Where to spawn the flash

    [Header("Scene Settings")]
    public string sceneToLoad = "MainMenu"; // Name of the scene to load after delay
    public float sceneLoadDelay = 90f;      // Seconds before loading the next scene

    [Header("UI Overlay")]
    public GameObject overlayImagePrefab;  // Fullscreen image prefab to show on trigger
    public float overlayDuration = 5f;     // Seconds to display the overlay

    // Ensure the collider is set as a trigger in the editor
    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    // Main trigger handler: spawn effects, enable dash, show overlay, unlock level, schedule scene load
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnOpenFlash();

            var controller = other.GetComponent<ThirdPersonController>();
            if (controller != null)
                controller.dashEnabled = true;  // Enable player dash ability

            if (overlayImagePrefab != null)
                StartCoroutine(ShowOverlay());

            UnlockNextLevel();
            StartCoroutine(DelayedSceneLoad());
        }
        else if (other.CompareTag("Block"))
        {
            Destroy(gameObject); // Remove trigger if hit by a block
        }
    }

    // Instantiates the flash effect and schedules its destruction
    private void SpawnOpenFlash()
    {
        if (openFlashPrefab == null) return;
        var flash = Instantiate(openFlashPrefab, flashSpawnPosition, Quaternion.identity);
        Destroy(flash, flashDuration);
        Debug.Log($"[OpenAndSceneTrigger] Spawned OpenFlash at {flashSpawnPosition}");
    }

    // Displays a fullscreen UI overlay for a set duration
    private IEnumerator ShowOverlay()
    {
        // Find or create a Canvas for UI overlay
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var go = new GameObject("OverlayCanvas");
            canvas = go.AddComponent<Canvas>();
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // Instantiate overlay image under the canvas and stretch it fullscreen
        var overlay = Instantiate(overlayImagePrefab, canvas.transform);
        var rect = overlay.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;    // bottom-left corner
        rect.anchorMax = Vector2.one;     // top-right corner
        rect.offsetMin = rect.offsetMax = Vector2.zero; // no padding

        yield return new WaitForSeconds(overlayDuration);
        Destroy(overlay);
    }

    // Increments the PlayerPrefs "UnlockedLevels" value
    private void UnlockNextLevel()
    {
        int unlocked = PlayerPrefs.GetInt("UnlockedLevels", 1);
        PlayerPrefs.SetInt("UnlockedLevels", unlocked + 1);
        PlayerPrefs.Save();
        Debug.Log($"[OpenAndSceneTrigger] Unlocked Levels incremented to {unlocked + 1}");
    }

    // Waits for sceneLoadDelay seconds before loading the target scene
    private IEnumerator DelayedSceneLoad()
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        LoadTargetScene();
    }

    // Loads the configured scene name or logs a warning if unset
    private void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"[OpenAndSceneTrigger] Loading Scene: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("[OpenAndSceneTrigger] sceneToLoad is empty or not set!");
        }
    }
}
