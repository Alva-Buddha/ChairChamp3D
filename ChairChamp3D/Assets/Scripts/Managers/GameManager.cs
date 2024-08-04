using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Static instance of GameManager which allows it to be accessed by any other script
    public static bool GameIsPaused = false; // Static variable used to track whether or not the game is paused

    public int score; // Variable used to track the player's score
    public int additionalChairs = 0; // Variable used to set the number of additional chairs to spawn
    public int totalChairs; // Variable used to track the total number of chairs
    public int unoccupiedChairs; // Variable used to track the number of unoccupied chairs
    public int playerChairs; // Variable used to track the number of chairs the player has acquired
    public int npcChairs; // Variable used to track the number of chairs the NPCs have acquired
    public float roundStartTimerFrom = 10.0f; // Music will stop at a certain time between the From and the To variables
    public float roundStartTimerTo = 20.0f; // Music will stop at a certain time between the From and the To variables
    public bool musicPlaying = false; // Variable used to track if the music is playing
    public bool roundStarted = false; // Variable used to track if the starting music has stopped and the round has started
    public bool hasRoundEnded = false; // Variable used to track if the round has ended

    public GameObject chairSpawner; // Variable used to track the chair spawner object
    public GameObject pauseMenuUI; // Variable used to track the pause menu ui
    public GameObject roundEndUI; // Variable used to track the round end ui
    public GameObject tutorial; // Variable used to track the tutorial popup ui

    private AudioManager audioManager; // Variable used to track the audio manager object for before the round starts
    private UIManager uiManager; // Variable used to track the UI manager object for before the round starts

    public int NPCCount; // Public variable to expose NPC count

    private const string NPCCountPrefsKey = "NPCCount"; // Key for PlayerPrefs

    // Awake is called before Start
    void Awake()
    {
        #region Singleton
        // Check if instance already exists
        if (Instance == null)
        {
            // If not, set instance to this
            Instance = this;
            // Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }
        // If instance already exists and it's not this
        else if (Instance != this)
        {
            // Then destroy this, meaning there can only ever be one instance of a GameManager
            Destroy(gameObject);
        }
        #endregion

        // Ensure the game is not paused when the scene starts
        ResumeGame();
    }

    void Start()
    {
        // Load NPCCount from PlayerPrefs
        if (PlayerPrefs.HasKey(NPCCountPrefsKey))
        {
            NPCCount = PlayerPrefs.GetInt(NPCCountPrefsKey);
        }
        else
        {
            NPCCount = 1; // Default value if no saved value exists
        }

        SetUpLevel();
        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            PlayTutorial();
        }
    }

    // Function to set up level
    private void SetUpLevel()
    {
        #region Set up chairs
        // Set unoccupiedChairs to the NPCCount value
        totalChairs = NPCCount + additionalChairs;
        unoccupiedChairs = totalChairs;
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

        #region Set up ui
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("The script 'UIManager' can't be found on any UI game objects called 'Canvas'.");
        }
        #endregion
    }

    // Function to play the tutorial popup on the Tutorial level
    private void PlayTutorial()
    {
        PauseGame();
        tutorial.SetActive(true);
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

        // If all chairs are occupied, round is over; pause the game, play round end music, and bring up the round end UI
        if ((unoccupiedChairs == 0 || playerChairs > 0) && hasRoundEnded == false)
        {
            EndRound();
        }

        // Restart the game when pressing the R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    /// <summary>
    /// Function to handle the end of the round
    /// </summary>
    void EndRound()
    {
        hasRoundEnded = true;

        // Enable UI
        roundEndUI.SetActive(true);
        uiManager.EndRound();

        // Pause control
        Time.timeScale = 0f;
        GameIsPaused = true;

        // Play end round music
        if (npcChairs == 0)
        {
            audioManager.PlayPlayerSoloWinAudio();
        }
        else
        {
            audioManager.PlayRoundEndAudio();
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
        Destroy(gameObject); // Destroy the old GameManager instance
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Function to go back to the main menu
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        Destroy(gameObject); // Destroy the old GameManager instance
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Function to load the next scene
    /// </summary>
    public void LoadNextScene()
    {
        Destroy(gameObject); // Destroy the old GameManager instance
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Function to quit the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
