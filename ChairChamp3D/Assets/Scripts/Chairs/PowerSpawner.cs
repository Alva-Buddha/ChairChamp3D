using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerSpawner : MonoBehaviour
{
    [Header("Spawn parameters")]
    [Tooltip("PowerUp Prefab to spawn")]
    public GameObject PowerPrefab = null;
    [Tooltip("The number of powers to spawn")]
    public int numberOfPowers = 4;
    [Tooltip("Parent for spawned power objects")]
    public GameObject PowerParent = null;
    [Tooltip("Inner radius of Power Spawn")]
    public float innerRadius = 10.0f;
    [Tooltip("Outer radius of Power Spawn")]
    public float outerRadius = 20.0f;
    [Tooltip("Avoid distance")]
    public float avoidDistance = 1.0f;

    //The GameManager to read music state from
    private GameManager gameManager;

    //Power timer
    private float PowerTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Get the GameManager component
        SetupGameManager();
        //iterate through number of powers to spawn
        for (int i = 0; i < numberOfPowers; i++)
        {
            //Get spawn position
            SpawnPower();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Function to spawn a random Power at a random position between inner radius and outer radius
    private void SpawnPower()
    { 

        //Randomly select a position between inner radius and outer radius
        Vector3 spawnPosition = Random.insideUnitCircle.normalized * Random.Range(innerRadius, outerRadius);
        spawnPosition.z = spawnPosition.y;
        spawnPosition.y = PowerPrefab.transform.position.y;

        //Check if there is any other collider around spawn position within avoid distance
        Collider[] hitColliders = Physics.OverlapSphere(spawnPosition, avoidDistance);

        //if there is any collider around spawn position, spawn a new position
        while (hitColliders.Length > 0)
        {
            spawnPosition = Random.insideUnitCircle.normalized * Random.Range(innerRadius, outerRadius);
            spawnPosition.z = spawnPosition.y;
            spawnPosition.y = PowerPrefab.transform.position.y;
            hitColliders = Physics.OverlapSphere(spawnPosition, avoidDistance);
        }

        //Instantiate the Power prefab at the spawn position
        GameObject Power = Instantiate(PowerPrefab, spawnPosition, Quaternion.identity);

        //Set the parent of the Power to the Power parent
        Power.transform.parent = PowerParent.transform;

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
