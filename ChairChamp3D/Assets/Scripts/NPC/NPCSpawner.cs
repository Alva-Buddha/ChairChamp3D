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
    [Tooltip("Transform of player spawn")]
    public GameObject player = null;

    private Vector3 spawnLocation;

    

    // Start is called before the first frame update
    void Start()
    {
        // check if player object is set to null, and if null, search for player tag and asign to object
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        spawnLocation = player.transform.position;
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
