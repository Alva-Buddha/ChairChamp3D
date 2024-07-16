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

    private AudioSource musicSource; // Variable used to track the music object for before the round starts

    void Awake()
    {
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
        // Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        chairSpawner = GameObject.Find("ChairSpawner"); // Find the chair spawner object in the scene

        // Ensure the game is not paused when the scene starts
        ResumeGame();
    }

    // Start is called before the first frame update
    void Start()
    {

        unoccupiedChairs = GameObject.FindGameObjectsWithTag("Chair").Length;
        // Set unoccupiedChairs to the number of "Chair" prefabs in the scene
        if (unoccupiedChairs == 0 || chairSpawner != null)
        {
            unoccupiedChairs = chairSpawner.GetComponent<ChairSpawner>().numberOfChairs;
        }
        musicSource = Camera.main.GetComponent<AudioSource>();
        StartCoroutine(PlayAndStopMusic());
    }

    //Plays music when the game starts, then stops it at a semi-random time range
    IEnumerator PlayAndStopMusic()
    {
        musicSource.Play();
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
                musicSource.Stop();
                musicPlaying = false;
                roundStarted = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {     
        // If the pause button is pressed, pause/resume the game depending on if it's already paused or not
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

        //If all chairs are occupied, pause the game and bring up the round end UI
        if (unoccupiedChairs == 0)
        {
            PauseGame();
        }

        //restart the game when pressing the R key
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
        if (musicSource != null)
        {
            musicSource.Pause();
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
        if (roundStarted == false && musicSource != null)
        {
            musicSource.UnPause();
            musicPlaying = true;
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
