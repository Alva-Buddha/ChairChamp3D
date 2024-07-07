using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("Spawn parameters")]
    [Tooltip("The NPC prefab to spawn")]
    public GameObject NPCPrefab = null;
    [Tooltip("The number of NPCs to spawn")]
    public int numberOfNPC = 4;
    [Tooltip("Parent for spawned NPC objects")]
    public GameObject NPCParent = null;
    [Tooltip("Location of player spawn")]
    public Vector3 playerLocation = new Vector3(0, 0.2f, 10);

    private Vector3 spawnLocation;

    

    // Start is called before the first frame update
    void Start()
    {
        spawnLocation = playerLocation;
        //Loop to spawn NPC
        for (int i = 0; i < numberOfNPC; i++)
        {
            //Set spawnLocation for next NPC by rotating the spawnLocation vector around the y-axis away from the player
            spawnLocation = Quaternion.Euler(0, 360 / (numberOfNPC + 1), 0) * spawnLocation;

            //Instantiate NPC prefab
            GameObject NPC = Instantiate(NPCPrefab, spawnLocation, Quaternion.identity);

            //Set parent of NPC to NPCParent
            NPC.transform.parent = NPCParent.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
