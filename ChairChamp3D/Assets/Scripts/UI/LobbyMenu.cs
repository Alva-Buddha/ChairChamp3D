using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    public Slider npcSlider;
    public TextMeshProUGUI displayedCount;

    void Start()
    {
        // Set the initial value of the slider and the text
        npcSlider.value = 1;
        displayedCount.text = npcSlider.value.ToString();

        // Add a listener to the slider
        npcSlider.onValueChanged.AddListener(UpdateNPC);
    }

    void UpdateNPC(float value)
    {
        // Update the text to display the slider's value
        displayedCount.text = value.ToString();
    }
}