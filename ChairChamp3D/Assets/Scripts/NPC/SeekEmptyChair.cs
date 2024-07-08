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
    [Tooltip("Time to start/stop")]
    public float timeToAccelerate = 1f;

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

    //The object's rigidbody
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        chairs = GameObject.FindGameObjectsWithTag("Chair");
        gameManager = GameManager.Instance;
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.musicPlaying)
        {
            // Calculate distance from player to origin
            Vector3 distanceToOrigin = (Vector3.zero - rb.position);

            // Calculate perpendicular vector for circular movement around origin
            Vector3 perpendicularVector = Vector3.Cross(Vector3.up, distanceToOrigin);

            // Set velocity by multiplying perpendicular vector with music rotation speed in radians
            rb.velocity = perpendicularVector * musicRotationSpeed * Mathf.Deg2Rad;

            // Calculate the target angle in degrees
            float targetAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

            // Create a target rotation based on the target angle
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            // Smoothly rotate towards the target rotation using Rigidbody.MoveRotation
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime));
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
        Vector3 movement = target.transform.position - transform.position;
        movement.y = 0; // Ignore the y component for rotation

        // Calculate the acceleration vector based on input, move speed, and time to accelerate
        Vector3 acceleration = movement.normalized * moveSpeed / timeToAccelerate;

        if (Vector3.Distance(transform.position, target.transform.position) > stoppingDistance)
        {
            // Apply the calculated force to the Rigidbody component
            rb.velocity += acceleration * Time.deltaTime;

            //Cap velocity at move speed
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, moveSpeed);

            // Calculate the target angle in degrees
            float targetAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

            // Create a target rotation based on the target angle
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            // Smoothly rotate towards the target rotation using Rigidbody.MoveRotation
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime));
        }
        else
        {
            if (rb.velocity.magnitude > 0.01)
            {
                //reduce the velocity magnitude based on timeToAccelerate
                rb.velocity -= rb.velocity * Time.deltaTime / timeToAccelerate;
            }
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
