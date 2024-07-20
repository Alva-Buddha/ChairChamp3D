using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private AudioManager audioManager; // Variable used to track the audio manager object for before the round starts

    public void Start()
    {
        #region Set up audio
        // Find the AudioManager gameobject using the 'Audio' tag
        GameObject audioManagerObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioManagerObject != null)
        {
            audioManager = audioManagerObject.GetComponent<AudioManager>();
        }
        else
        {
            Debug.LogError("No GameObject with tag 'Audio' found in the scene.");
        }
        #endregion
    }

    // Function to proceed to the next scene in the build order (which should be the first level)
    public void PlayGame()
    {
        audioManager.StopMusic();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
