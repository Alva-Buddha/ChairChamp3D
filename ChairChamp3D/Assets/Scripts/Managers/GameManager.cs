using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;// Static instance of GameManager which allows it to be accessed by any other script
    public static bool GameIsPaused = false; // Static variable used to track whether or not the game is paused

    public int score;  // Variable used to track the player's score
    public int unoccupiedChairs; // Variable used to track the number of unoccupied chairs
    public int playerChairs; // Variable used to track the number of chairs the player has acquired
    public int npcChairs; // Variable used to track the number of chairs the NPCs have acquired
    public float roundStartTimerFrom = 10.0f; // Music will stop at a certain time between the From and the To variables
    public float roundStartTimerTo = 20.0f; // Music will stop at a certain time between the From and the To variables
    public bool musicPlaying = false; // Variable used to track if the music is playing
    public bool roundStarted = false; // Variable used to track if the starting music has stopped and the round has started

    public GameObject chairSpawner = null; // Variable used to track the chair spawner object
    public GameObject pauseMenuUI; // Variable used to track the pause menu ui

    private AudioManager audioManager; // Variable used to track the audio manager object for before the round starts

    void Awake()
    {
        #region Singleton
        // Check if instance already exists
        if (Instance == null)
        {
            // If not, set instance to this
            Instance = this;
        }
        // If instance already exists and it's not this
        else if (Instance != this)
        {
            // Then destroy this, meaning there can only ever be one instance of a GameManager
            Destroy(gameObject);
        }
        #endregion
        // Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        // Find the chair spawner object in the scene
        chairSpawner = GameObject.Find("ChairSpawner"); 

        // Ensure the game is not paused when the scene starts
        ResumeGame();
    }

    void Start()
    {
        #region Set up chairs
        unoccupiedChairs = GameObject.FindGameObjectsWithTag("Chair").Length;
        // Set unoccupiedChairs to the number of "Chair" prefabs in the scene
        if (unoccupiedChairs == 0 || chairSpawner != null)
        {
            unoccupiedChairs = chairSpawner.GetComponent<ChairSpawner>().numberOfChairs;
        }
        #endregion

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

        // Start playing the pre-round music on a timer
        StartCoroutine(PlayAndStopMusic());
        #endregion
    }

    // Coroutine that plays music when the round starts, then stops it at a time range specified in the public variables
    IEnumerator PlayAndStopMusic()
    {
        audioManager.PlayPreRoundMusic();
        musicPlaying = true;
        while (musicPlaying)
        {
            if (GameIsPaused)
            {
                yield return null; // Pauses the coroutine when the game is paused
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(roundStartTimerFrom, roundStartTimerTo));
                audioManager.StopMusic();
                musicPlaying = false;
                roundStarted = true;
                audioManager.PlayLevelThemeMusic();
            }
        }
    }

    void Update()
    {     
        // Pause/Unpause the game
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (GameIsPaused)
            {
                ResumeGame();
                pauseMenuUI.SetActive(false);
            }
            else
            {
                PauseGame();
                pauseMenuUI.SetActive(true);
            }
        }

        // If all chairs are occupied, round is over; pause the game, play music and bring up the round end UI
        if (unoccupiedChairs == 0)
        {
            PauseGame();
        }

        // Restart the game when pressing the R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    /// <summary>
    /// Function to pause the game & music
    /// </summary>
    void PauseGame()
    {
        // Pause control
        Time.timeScale = 0f;
        GameIsPaused = true;

        // Music control
        if (audioManager != null)
        {
            audioManager.PauseMusic();
        }
    }

    /// <summary>
    /// Function to resume/unpause the game & music
    /// </summary>
    public void ResumeGame()
    {
        // Pause control
        Time.timeScale = 1f;
        GameIsPaused = false;

        // Music control
        if (audioManager != null)
        {
            if (roundStarted == false)
            {
                audioManager.UnpauseMusic();
                musicPlaying = true;
            }
            else
            {
                audioManager.UnpauseMusic();
            }
        }
    }

    /// <summary>
    /// Function to restart the game
    /// </summary>
    public void RestartGame()
    {
        GameIsPaused = false;
        Destroy(gameObject);  // Destroy the old GameManager instance
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Function to go back to the main menu
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Function to quit the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
