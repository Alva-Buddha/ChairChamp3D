using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("Spawn parameters")]
    [Tooltip("The NPC prefab to spawn")]
    public GameObject NPCPrefab = null;
    [Tooltip("Change in NPC spawn count for level vs lobby prefs")]
    public int NPCCountChange = 0;
    [Tooltip("Parent for spawned NPC objects")]
    public GameObject NPCParent = null;
    [Tooltip("Transform of player spawn")]
    public GameObject player = null;

    private Vector3 spawnLocation;

    [Tooltip("The number of NPCs to spawn - note this is set from Lobby via playerprefs")]
    public int numberOfNPC = 4;

    // Game Manager to read NPC count from
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        // Start the coroutine to set up the game manager and spawn NPCs
        StartCoroutine(InitializeAndSpawnNPCs());
    }

    // Coroutine to initialize the game manager and spawn NPCs
    private IEnumerator InitializeAndSpawnNPCs()
    {
        // Wait until the end of the frame to ensure GameManager and ChairSpawner have started
        yield return new WaitForEndOfFrame();

        // Get the GameManager component and return error if not found
        SetupGameManager();

        // Initialize number of NPCs from NPC count in GameManager
        numberOfNPC = gameManager.NPCCount + NPCCountChange;
        Debug.Log("Number of NPCs from gameManager: " + gameManager.NPCCount);
        Debug.Log("Number of NPCs: " + numberOfNPC);

        // Check if player object is set to null, and if null, search for player tag and assign to object
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        spawnLocation = player.transform.position;

        // Loop to spawn NPCs
        for (int i = 0; i < numberOfNPC; i++)
        {
            // Set spawnLocation for next NPC by rotating the spawnLocation vector around the y-axis away from the player
            spawnLocation = Quaternion.Euler(0, 360 / (numberOfNPC + 1), 0) * spawnLocation;

            // Instantiate NPC prefab with different names and spawn locations
            GameObject NPC = Instantiate(NPCPrefab, spawnLocation, Quaternion.identity);

            // Set parent of NPC to NPCParent
            NPC.transform.parent = NPCParent.transform;

            // Set name of NPC to include index
            NPC.name = "NPC" + i;
        }
    }

    // Function to get the GameManager component
    private void SetupGameManager()
    {
        // Find the GameManager object in the scene
        GameObject gameManagerObject = GameObject.Find("GameManager");

        // If the GameManager object is found, get the GameManager component
        if (gameManagerObject != null)
        {
            gameManager = gameManagerObject.GetComponent<GameManager>();
        }
        // If the GameManager object is not found, log an error
        else
        {
            Debug.LogError("GameManager not found in scene");
        }
    }
}
