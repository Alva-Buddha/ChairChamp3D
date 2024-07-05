using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;// Static instance of GameManager which allows it to be accessed by any other script
    public int score;  // Variable used to track the player's score
    public int unoccupiedChairs = 2; // Variable used to track the number of unoccupied chairs
    public int playerChairs;
    public int npcChairs;

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
    }

    // Update is called once per frame
    void Update()
    {
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
