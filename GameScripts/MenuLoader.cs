using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    // Triggered when a collider enters this trigger zone
    private void OnTriggerEnter(Collider other)
    {
        // Only act if the player enters
        if (other.CompareTag("Player"))
        {
            UnlockNextLevel();                    // Increment and save unlocked level count
            SceneManager.LoadScene("MainMenu");   // Load the main menu scene
        }
    }

    // Increments the "UnlockedLevels" value in PlayerPrefs
    private void UnlockNextLevel()
    {
        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1); // Current unlocked level count (default 1)
        PlayerPrefs.SetInt("UnlockedLevels", unlockedLevels + 1);     // Store incremented count
        PlayerPrefs.Save();                                           // Persist changes
    }
}
