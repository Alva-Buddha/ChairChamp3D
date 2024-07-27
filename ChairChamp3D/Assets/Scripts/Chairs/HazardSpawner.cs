using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    [Header("Spawn parameters")]
    [Tooltip("Array of Hazard Prefabs to select randomly from")]
    public GameObject[] hazardPrefabs = null;
    [Tooltip("Parent for spawned hazard objects")]
    public GameObject hazardParent = null;
    [Tooltip("Inner radius of Hazard Spawn")]
    public float innerRadius = 10.0f;
    [Tooltip("Outer radius of Hazard Spawn")]
    public float outerRadius = 20.0f;
    [Tooltip("TimeGap between appearance of Hazards")]
    public float timeGap = 10.0f;
    [Tooltip("Duration of Hazards")]
    public float hazardDuration = 5.0f;

    [Tooltip("Should the Hazard appear during music")]
    public bool musicHazard = false;

    [Tooltip("Variable to monitor spawn position - do not set")]
    public Vector3 spawnPositionCheck;

    //The GameManager to read music state from
    private GameManager gameManager;

    //Hazard timer
    private float hazardTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Get the GameManager component
        SetupGameManager();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if music is player and if the hazard should appear during music
        if (gameManager.musicPlaying && musicHazard)
        {
            //Increment the hazard timer
            hazardTimer += Time.deltaTime;

            //If the hazard timer is greater than the time gap
            if (hazardTimer > timeGap)
            {
                //Spawn a hazard
                SpawnHazard();
                //Reset the hazard timer
                hazardTimer = 0.0f;
            }
        }
        else if (!gameManager.musicPlaying)
        {
            //Increment the hazard timer
            hazardTimer += Time.deltaTime;

            //If the hazard timer is greater than the time gap
            if (hazardTimer > timeGap)
            {
                //Spawn a hazard
                SpawnHazard();
                //Reset the hazard timer
                hazardTimer = 0.0f;
            }
        }
    }

    //Function to spawn a random hazard at a random position between inner radius and outer radius
    private void SpawnHazard()
    {
        //Randomly select a hazard prefab
        GameObject hazardPrefab = hazardPrefabs[Random.Range(0, hazardPrefabs.Length)];

        //Randomly select a position between inner radius and outer radius
        Vector3 spawnPosition = Random.insideUnitCircle.normalized * Random.Range(innerRadius, outerRadius);
        spawnPosition.z = spawnPosition.y;
        spawnPosition.y = hazardPrefab.transform.position.y;
        spawnPositionCheck = spawnPosition;

        //Instantiate the hazard prefab at the spawn position
        GameObject hazard = Instantiate(hazardPrefab, spawnPosition, Quaternion.identity);

        //Set the parent of the hazard to the hazard parent
        hazard.transform.parent = hazardParent.transform;

        //Start coroutine to destroy the hazard after the hazard duration
        StartCoroutine(DestroyHazard(hazard));
    }

    // Coroutine to destroy the hazard after the hazard duration
    IEnumerator DestroyHazard(GameObject hazard)
    {
        //Wait for the hazard duration
        yield return new WaitForSeconds(hazardDuration);

        //Destroy the hazard
        Destroy(hazard);
    }

    //Function to set up GameManager if it is not already set up. Throws an error if none exists
    private void SetupGameManager()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }
    }
}
