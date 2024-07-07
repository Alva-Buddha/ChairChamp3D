using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairSpawner : MonoBehaviour
{
    [Header("Spawn parameters")]
    [Tooltip("The chair prefab to spawn")]
    public GameObject chairPrefab = null;
    [Tooltip("The number of chairs to spawn")]
    public int numberOfChairs = 4;
    [Tooltip("Parent for spawned chair objects")]
    public GameObject chairParent = null;
    [Tooltip("Location of 1st spawn")]
    public Vector3 spawnLocation = new Vector3(0, 0, 5);



    // Start is called before the first frame update
    void Start()
    {
        //Loop to spawn chairs
        for (int i = 0; i < numberOfChairs; i++)
        {
            //Instantiate chair prefab
            GameObject chair = Instantiate(chairPrefab, spawnLocation, Quaternion.identity);
            
            //Set parent of chair to chairParent
            chair.transform.parent = chairParent.transform;

            //Set spawnLocation for next chair by rotating the spawnLocation vector around the y-axis
            spawnLocation = Quaternion.Euler(0, 360/numberOfChairs, 0) * spawnLocation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
