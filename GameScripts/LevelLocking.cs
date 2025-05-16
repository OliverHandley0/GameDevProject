using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelLockingMenu : MonoBehaviour
{
    private const string UnlockedKey = "UnlockedLevels";

    public Button[] levelButtons;        // Buttons for each level
    public GameObject level2Indicator;   // Visual indicator for Level 2
    public TMP_Text level2Text;          // Text label for Level 2
    public GameObject level3Indicator;   // Visual indicator for Level 3
    public TMP_Text level3Text;          // Text label for Level 3

    void Start()
    {
        PlayerPrefs.SetInt(UnlockedKey, 1);
        PlayerPrefs.Save();
        Debug.Log($"[LevelLockingMenu] Forced UnlockedLevels → 1");
        int unlockedLevels = PlayerPrefs.GetInt(UnlockedKey, 1);
        Debug.Log($"[LevelLockingMenu] Start detected UnlockedLevels = {unlockedLevels}");
        for (int i = 0; i < levelButtons.Length; i++)
            levelButtons[i].interactable = false;

        for (int i = 0; i < unlockedLevels && i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            levelButtons[i].interactable = true;
            levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
        }

        // Adjust indicators and text sizes based on unlocked levels
        UpdateIndicators();
    }


    // to reset the unlocked count back to one (so Level 1 remains unlocked)
    public static void ResetUnlockedLevels()
    {
        PlayerPrefs.SetInt(UnlockedKey, 1);   // Reset to 1
        PlayerPrefs.Save();                   // Persist changes
        Debug.Log("[LevelLockingMenu] UnlockedLevels reset to 1");
    }

    // Loads the scene corresponding to the given level index
    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene($"Level{levelIndex}");
    }


    public void UnlockNextLevel()
    {
        int unlockedLevels = PlayerPrefs.GetInt(UnlockedKey, 1);
        if (unlockedLevels < levelButtons.Length)
        {
            unlockedLevels++;
            PlayerPrefs.SetInt(UnlockedKey, unlockedLevels);
            PlayerPrefs.Save();
            Debug.Log($"[LevelLockingMenu] UnlockNextLevel → {unlockedLevels}");

            // Enable the newly unlocked button
            levelButtons[unlockedLevels - 1].interactable = true;
            int levelIndex = unlockedLevels;
            levelButtons[unlockedLevels - 1]
                .onClick.AddListener(() => LoadLevel(levelIndex));

            UpdateIndicators();
        }
    }

    // Moves indicator Y positions and sets TMP font sizes when levels are unlocked
    public void UpdateIndicators()
    {
        int unlockedLevels = PlayerPrefs.GetInt(UnlockedKey, 1);

        // Level 2 indicator and text
        if (level2Indicator != null && unlockedLevels >= 2)
        {
            Vector3 pos2 = level2Indicator.transform.localPosition;
            level2Indicator.transform.localPosition = new Vector3(pos2.x, -10f, pos2.z);
            level2Text.fontSize = 20;
        }

        // Level 3 indicator and text
        if (level3Indicator != null && unlockedLevels >= 3)
        {
            Vector3 pos3 = level3Indicator.transform.localPosition;
            level3Indicator.transform.localPosition = new Vector3(pos3.x, 0f, pos3.z);
            level3Text.fontSize = 20;
        }
    }
}
