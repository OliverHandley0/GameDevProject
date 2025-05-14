using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelLockingMenu : MonoBehaviour
{
    public Button[] levelButtons;        // Buttons for each level
    public GameObject level2Indicator;   // Visual indicator for Level 2
    public TMP_Text level2Text;          // Text label for Level 2
    public GameObject level3Indicator;   // Visual indicator for Level 3
    public TMP_Text level3Text;          // Text label for Level 3

    void Start()
    {
        // Disable all buttons, then enable those up to the unlocked count
        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);
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

    // Loads the scene corresponding to the given level index
    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene($"Level{levelIndex}");
    }

    // Unlocks the next level, updates PlayerPrefs, button and indicators
    public void UnlockNextLevel()
    {
        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);
        if (unlockedLevels < levelButtons.Length)
        {
            unlockedLevels++;
            PlayerPrefs.SetInt("UnlockedLevels", unlockedLevels);
            PlayerPrefs.Save();

            levelButtons[unlockedLevels - 1].interactable = true;
            levelButtons[unlockedLevels - 1].onClick.AddListener(() => LoadLevel(unlockedLevels));

            UpdateIndicators();
        }
    }

    // Moves indicator Y positions and sets TMP font sizes when levels are unlocked
    public void UpdateIndicators()
    {
        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);

        // Level 2 indicator and text
        if (level2Indicator != null)
        {
            Vector3 pos2 = level2Indicator.transform.localPosition;
            float y2 = (unlockedLevels >= 2) ? 0f : pos2.y;
            level2Indicator.transform.localPosition = new Vector3(pos2.x, y2, pos2.z);
        }
        if (level2Text != null && unlockedLevels >= 2)
        {
            level2Text.fontSize = 20;
        }

        // Level 3 indicator and text
        if (level3Indicator != null)
        {
            Vector3 pos3 = level3Indicator.transform.localPosition;
            float y3 = (unlockedLevels >= 3) ? 0f : pos3.y;
            level3Indicator.transform.localPosition = new Vector3(pos3.x, y3, pos3.z);
        }
        if (level3Text != null && unlockedLevels >= 3)
        {
            level3Text.fontSize = 20;
        }
    }
}
