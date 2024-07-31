using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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
    [Tooltip("Power avoid distance")]
    public float powerAvoidDistance = 5.0f;
    [Tooltip("Iteration limit to avoid infinite loops")]
    public int iterationLimit = 1000;


    //The GameManager to read music state from
    private GameManager gameManager;

    //tag on powerprefab
    private string powerTag;

    //layer of powerprefab
    public LayerMask powerLayer;

    //bool to check power overlap
    private bool powerOverlap = false;

    //iteration count
    private int iterCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Get the GameManager component
        SetupGameManager();

        //Get the tag of the PowerPrefab
        powerTag = PowerPrefab.tag;

        //Get the layer of the PowerPrefab in powerLayer layermask
        powerLayer = 1 << PowerPrefab.layer;

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
        powerOverlap = false;

        //Randomly select a position between inner radius and outer radius
        Vector3 spawnPosition = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(innerRadius, outerRadius);
        spawnPosition.z = spawnPosition.y;
        spawnPosition.y = PowerPrefab.transform.position.y;

        //Check if there is any other collider around spawn position within avoid distance
        Collider[] hitColliders = Physics.OverlapSphere(spawnPosition, avoidDistance);

        // Check if there is another power collider around spawn position within power avoid distance
        // This needs to check if the collider is a power collider using tags or layers
        Collider[] powerColliders = Physics.OverlapSphere(spawnPosition, powerAvoidDistance);

        // Filter the results based on the LayerMask
        foreach (Collider localCollider in powerColliders)
        {
            if (((1 << localCollider.gameObject.layer) & powerLayer) != 0)
            {
                // Set powerOverlap to true if there is a power collider
                powerOverlap = true;
                break; // No need to check further if we already found an overlap
            }
        }

        iterCount = 0;

        //if there is any collider around spawn position, spawn a new position
        while (hitColliders.Length > 0 || powerOverlap)
        {
            iterCount++;
            if (iterCount > iterationLimit)
            {
                Debug.LogWarning("Unable to find a valid spawn position after " + iterationLimit + " iterations.");
                return;
            }

            spawnPosition = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(innerRadius, outerRadius);
            spawnPosition.z = spawnPosition.y;
            spawnPosition.y = PowerPrefab.transform.position.y;
            hitColliders = Physics.OverlapSphere(spawnPosition, avoidDistance);
            powerColliders = Physics.OverlapSphere(spawnPosition, powerAvoidDistance);

            // Filter the results based on the LayerMask
            foreach (Collider localColliders in powerColliders)
            {
                if (((1 << localColliders.gameObject.layer) & powerLayer) != 0)
                {
                    //set powerOverlap to true if there is a power collider
                    powerOverlap = true;
                    break; // No need to check further if we already found an overlap
                }
            }
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
