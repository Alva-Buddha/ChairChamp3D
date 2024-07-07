using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;// Static instance of GameManager which allows it to be accessed by any other script
    public int score;  // Tracks the player's score
    public int unoccupiedChairs; // Tracks the number of unoccupied chairs
    public int playerChairs; // Tracks the number of chairs the player has acquired
    public int npcChairs; // Tracks the number of chairs the NPCs have acquired
    public float roundStartTimerFrom = 10.0f; // Music will stop at a certain time between the From and the To variables
    public float roundStartTimerTo = 20.0f; // Music will stop at a certain time between the From and the To variables
    public Button startButton; // Reference the start button in the UI

    private AudioSource musicSource; // Tracks the music object for before the round starts
    private bool gameStarted = false; // Tracks whether or not the game has started

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

        // Ensure the game is not paused when the scene starts
        ResumeGame();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set unoccupiedChairs to the number of "Chair" prefabs in the scene
        unoccupiedChairs = GameObject.FindGameObjectsWithTag("Chair").Length;
        // Set the audio source on the camera to the music source variable
        musicSource = Camera.main.GetComponent<AudioSource>();
        // Play music on clicking the start button
        startButton.onClick.AddListener(() => {
            gameStarted = true;
            StartCoroutine(PlayAndStopMusic());
        });
    }

    //Plays music when the game starts, then stops it at a semi-random time range
    IEnumerator PlayAndStopMusic()
    {
        musicSource.Play();
        yield return new WaitForSeconds(Random.Range(roundStartTimerFrom, roundStartTimerTo));
        musicSource.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        // Don't start the game until the start button is clicked
        if (!gameStarted) return;
        
        //If all chairs are occupied, pause the game and bring up the round end UI
        if (unoccupiedChairs == 0)
        {
            PauseGame();
        }

        //restart the game when pressing the R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            Destroy(gameObject);  // Destroy the old GameManager instance
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// Function to pause the game
    /// </summary>
    void PauseGame()
    {
        Time.timeScale = 0;
    }

    /// <summary>
    /// Function to resume/unpause the game
    /// </summary>
    void ResumeGame()
    {
        Time.timeScale = 1;
    }
}
