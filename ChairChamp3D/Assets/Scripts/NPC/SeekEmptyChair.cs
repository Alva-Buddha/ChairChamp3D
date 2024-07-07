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

    [Tooltip("Rotation speed when music is playing")]
    public float musicRotationSpeed = 30f;

    //The GameManager to read music state from
    private GameManager gameManager;

    private GameObject closestChair = null;

    //get array of chair objects in scene
    GameObject[] chairs = null;

    [Header("Debugging")]
    [Tooltip("Forward ray visibility")]
    public bool showRay = false;
    [Tooltip("Forward ray scaling")]
    public float rayScale = 2;

    

    // Start is called before the first frame update
    void Start()
    {
        chairs = GameObject.FindGameObjectsWithTag("Chair");
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.musicPlaying)
        {
            //rotate around origin at speed = music rotation speed
            transform.RotateAround(Vector3.zero, Vector3.up, musicRotationSpeed * Time.deltaTime);
            //face origin
            transform.LookAt(Vector3.zero);
        }
        else
        {
            if (!reachedChair)
            {
                //Find closest unoccupied chair
                closestChair = FindClosestChair();
            }
            //Seek closest chair
            MoveTowards(closestChair);
        }
        if (showRay)
        {
            // Draw a ray forward from the player object in the Scene view
            Debug.DrawRay(transform.position, transform.forward * rayScale, Color.black);
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

        //Debug.Log("Closest Chair: " + closestChair.name);
        //return closest chair
        return closestChair;
    }

    /// <summary>
    /// Function to rotate and move towards closest chair object and update state when reached
    /// </summary>
    /// <param name="target">The chair to move towards</param>
    private void MoveTowards(GameObject target)
    {

        //Debug.Log("Moving towards: " + target.name);

        // Get direction to target
        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0; // Ignore the y component for rotation

        // Rotate towards target
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.transform.position) > stoppingDistance)
        {
            // Move towards target
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        }
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Chair")
        { 
            if (reachedChair)
            {
                closestChair = collision.gameObject;
            }
        }
    }

}
