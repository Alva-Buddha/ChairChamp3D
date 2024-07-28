using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Static instance of ScoreManager which allows it to be accessed by any other script.
    public int score; // Variable used to track the player's score.

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
            // Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a ScoreManager.
            Destroy(gameObject);
        }
        #endregion

        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // This method is called whenever a scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TutorialScene")
        {
            ResetScore();
        }
    }


    // Other methods to manipulate score
    public void IncreaseScore(int amount)
    {
        score += amount;
    }

    public void DecreaseScore(int amount)
    {
        score -= amount;
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetScore()
    {
        score = 0;
    }
}