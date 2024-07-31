using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChairSpawner : MonoBehaviour
{
    [Header("Spawn parameters")]
    [Tooltip("The chair prefab to spawn")]
    public GameObject chairPrefab = null;
    [Tooltip("Change in chair spawn count for level vs lobby prefs")]
    public int chairCountChange = 0;
    [Tooltip("Parent for spawned chair objects")]
    public GameObject chairParent = null;
    [Tooltip("Location of 1st spawn")]
    public Vector3 spawnLocation = new Vector3(0, 0, 5);

    //Note - this is set from playerprefs in Lobby Slider
    [Tooltip("The number of chairs to spawn - is set from Lobby Slider via PlayerPrefs")]
    public int numberOfChairs = 4;

    // Game Manager to read chair count from
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        // Start the coroutine to set up the game manager and spawn chairs
        StartCoroutine(InitializeAndSpawnChairs());
    }

    // Coroutine to initialize the game manager and spawn chairs
    private IEnumerator InitializeAndSpawnChairs()
    {
        // Wait until the end of the frame to ensure GameManager has started
        yield return new WaitForEndOfFrame();

        // Get the GameManager component and return error if not found
        SetupGameManager();

        // Initialize number of Chairs from chair count in GameManager
        numberOfChairs = gameManager.totalChairs + chairCountChange;

        // Loop to spawn chairs
        for (int i = 0; i < numberOfChairs; i++)
        {
            // Instantiate chair prefab
            GameObject chair = Instantiate(chairPrefab, spawnLocation, Quaternion.identity);
            // Ensure chair's Z is facing away from the center as defined by (0, chair.transform.position.y, 0)
            chair.transform.LookAt(new Vector3(0, chair.transform.position.y, 0));
            chair.transform.Rotate(0, 180, 0);

            // Set parent of chair to chairParent
            chair.transform.parent = chairParent.transform;

            // Set spawnLocation for next chair by rotating the spawnLocation vector around the y-axis
            spawnLocation = Quaternion.Euler(0, 360 / numberOfChairs, 0) * spawnLocation;
        }
    }

    // Function to get the GameManager component
    private void SetupGameManager()
    {
        // Find the GameManager object in the scene
        GameObject gameManagerObject = GameObject.Find("GameManager");
        if (gameManagerObject != null)
        {
            gameManager = gameManagerObject.GetComponent<GameManager>();
        }
        else
        {
            Debug.LogError("No GameObject with name 'GameManager' found in the scene.");
        }
    }
}
