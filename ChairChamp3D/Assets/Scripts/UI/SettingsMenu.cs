using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    // Variables to store an array of possible resolutions to change between
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    private void Start()
    {
        #region Resolution settings
        // Set up resolution options and clear the current list
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        // Create new list of strings which will be used to set up the resolution options
        List<string> options = new List<string>();

        // Variable used in the following for loop to make sure the current resolution is set automatically
        int currentResolutionIndex = 0;

        // Loop through each element in the "resolutions" array and create a string from the width and height, then add it to the list of options
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // Update the variable if the width and height of the current resolution being looked at matches our current resolution
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // Add resolution options from the "options" variable to the dropdown
        resolutionDropdown.AddOptions(options);

        // Update the current resolution value and refresh the display of it
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        #endregion
    }

    // Change the resolution of the game
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // Change quality level of the graphics
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    // Toggle fullscreen mode
    public void SetFullscreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}
