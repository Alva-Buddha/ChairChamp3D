using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekEmptyChair : MonoBehaviour
{
    [Header("Movement Variables")]
    [Tooltip("The speed at which the NPC will move.")]
    public float moveSpeed = 10.0f;
    [Tooltip("The speed at which the NPC rotates")]
    public float rotationSpeed = 60f;

    [Tooltip("The distance at which the NPC will stop moving towards the chair")]
    public float stoppingDistance = 0.1f;

    [Tooltip("Boolean to check if the NPC has reached the chair")]
    public bool reachedChair = false;

    private GameObject closestChair = null;

    //get array of chair objects in scene
    GameObject[] chairs = null;

    // Start is called before the first frame update
    void Start()
    {
        chairs = GameObject.FindGameObjectsWithTag("Chair");
    }

    // Update is called once per frame
    void Update()
    {
        if (!reachedChair)
        {
            //Find closest unoccupied chair
            GameObject closestChair = FindClosestChair();
            //Seek closest chair
            MoveTowards(closestChair);
        }    
    }

    /// <summary>
    /// Function to find closest chair
    /// </summary>
    /// <returns>closest chair gameobject</returns>
    private GameObject FindClosestChair()
    {
        //initialize closest distance to infinity
        float closestDistance = Mathf.Infinity;
        //loop through all chairs
        foreach (GameObject chair in chairs)
        {
            //get distance to chair
            float distance = Vector3.Distance(transform.position, chair.transform.position);
            //check if chair is unoccupied and closer than the current closest chair
            if (!chair.GetComponent<ChairState>().isOccupied && distance < closestDistance)
            {
                //set closest chair to current chair
                closestChair = chair;
                //set closest distance to current distance
                closestDistance = distance;
            }
        }
        //return closest chair
        return closestChair;
    }

    /// <summary>
    /// Function to rotate and move towards closest chair object and update state when reached
    /// </summary>
    /// <param name="target">The chair to move towards</param>
    private void MoveTowards(GameObject target)
    {
        if (!reachedChair && !target.GetComponent<ChairState>().isOccupied)
        {
            // Get direction to target
            Vector3 direction = target.transform.position - transform.position;
            direction.y = 0; // Ignore the y component for rotation

            // Rotate towards target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move towards target
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);

            // Check if NPC has reached the chair
            if (Vector3.Distance(transform.position, target.transform.position) < stoppingDistance)
            {
                // Set reachedChair to true
                reachedChair = true;
            }
        }
    }

}
