using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    public Slider npcSlider;
    public TextMeshProUGUI displayedCount;
    private const string NPCCountPrefsKey = "NPCCount";

    [Tooltip("NPC Count value to be set in playerprefs")]
    public int NPCCount;

    void Start()
    {
        // Load the saved slider value from PlayerPrefs using load NPC count function
        npcSlider.value = GetNPCCount();
        NPCCount = (int)npcSlider.value;

        // Set the initial text to display the slider's value
        displayedCount.text = npcSlider.value.ToString();

        // Add a listener to the slider
        npcSlider.onValueChanged.AddListener(UpdateNPC);
    }

    void UpdateNPC(float value)
    {
        // Update the text to display the slider's value
        displayedCount.text = value.ToString();

        NPCCount = (int)value;

        // Save the slider value to PlayerPrefs
        PlayerPrefs.SetInt(NPCCountPrefsKey, NPCCount);
        PlayerPrefs.Save();

        // Check if the playerprefs key is correctly set
        if (PlayerPrefs.HasKey(NPCCountPrefsKey))
        {
            NPCCount = PlayerPrefs.GetInt(NPCCountPrefsKey);
            Debug.Log("NPCCountPrefsKey set to " + NPCCount);
        }
    }

    //Function to get NPC count from playerprefs
    public int GetNPCCount()
    {
        // Load the saved slider value from PlayerPrefs
        if (PlayerPrefs.HasKey(NPCCountPrefsKey))
        {
            return PlayerPrefs.GetInt(NPCCountPrefsKey);
        }
        else
        {
            Debug.LogError("NPCCountPrefsKey not found in PlayerPrefs, value set to 0");
            return 0; // Default value if no saved value exists
        }
    }
}
