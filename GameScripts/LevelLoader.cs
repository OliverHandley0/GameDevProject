using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public GameObject uiPanel;    // UI panel to hide before loading the next level

    // Hides UI, updates player controller state, and loads "Level1"
    public void LoadLevel1()
    {
        if (uiPanel != null)
            uiPanel.SetActive(false);  // Deactivate UI panel

        GameObject player = GameObject.FindWithTag("Player");  // Find the player object
        if (player != null)
        {
            ThirdPersonController controller = player.GetComponent<ThirdPersonController>();
            if (controller != null)
                controller.isUIActive = false;  // Tell player controller that UI is no longer active
        }

        SceneManager.LoadScene("Level1");  // Switch to scene named "Level1"
    }
}
