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
    [Tooltip("The speed at which the NPC moves during music")]
    public float musicMoveSpeed = 5f;

    [Tooltip("The distance at which the NPC will stop moving towards the chair")]
    public float stoppingDistance = 0.1f;

    [Tooltip("Boolean to check if the NPC has reached the chair")]
    public bool reachedChair = false;

    [Tooltip("Distance to check for blockers")]
    public float checkBlockerDistance = 5.0f;
    [Tooltip("Y height of raycast to check for blockers")]
    public float checkBlockerHeight = 0.5f;

    [Tooltip("Left vs right preference")]
    public float lrPreference;

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
    [Tooltip("Flag to check if NPC is detecting a blocker")]
    public bool isBlocked = false;
    [Tooltip("Check variable for NPC velocity")]
    public Vector3 npcVelocity;

    //The object's rigidbody
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        chairs = GameObject.FindGameObjectsWithTag("Chair");
        gameManager = GameManager.Instance;
        rb = GetComponent<Rigidbody>();

        //Initiate LR preference as a random number between -2 and 2 excluding 0
        int[] validValues = { -2, -1, 1, 2 };
        lrPreference = validValues[Random.Range(0, validValues.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        #region musicPlaying
        if (gameManager.musicPlaying)
        {
            // Calculate direction from NPC to origin
            Vector3 directionToOrigin = (Vector3.zero - rb.position).normalized;

            // Calculate perpendicular direction for circular movement around origin
            Vector3 perpendicularDirection = Vector3.Cross(Vector3.up, directionToOrigin).normalized;

            // Set velocity to move NPC around origin, preserving the Y component
            Vector3 newVelocity = perpendicularDirection * musicMoveSpeed;
            newVelocity.y = rb.velocity.y; // Preserve the current Y velocity
            rb.velocity = newVelocity;
            npcVelocity = rb.velocity;

            // Calculate angular velocity for facing the origin
            // Determine the target rotation to face the origin
            Quaternion targetRotation = Quaternion.LookRotation(-directionToOrigin, Vector3.up);

            // Calculate the angular velocity needed to rotate the NPC towards the target rotation
            Quaternion deltaRotation = targetRotation * Quaternion.Inverse(rb.rotation);
            deltaRotation.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
            angleInDegrees = Mathf.DeltaAngle(0, angleInDegrees);
            Vector3 angularVelocity = (Mathf.Deg2Rad * angleInDegrees / Time.fixedDeltaTime) * rotationAxis.normalized;

            // Apply the calculated angular velocity
            rb.angularVelocity = angularVelocity;
        }
        #endregion
        #region !musicPlaying
        else
        {
            isBlocked = false;
            if (!reachedChair)
            {
                //Find closest unoccupied chair
                closestChair = FindClosestChair();
                //Function to check for nearby blocker between NPC and closestChair
                CheckBlocker(closestChair);
            }
            //Seek closest chair if not blocked
            if (!isBlocked)
            {
                MoveTowards(closestChair);
                npcVelocity = rb.velocity;
            }
        }
        #endregion
        if (showRay)
        {
            // Draw a ray forward from the player object in the Scene view
            Debug.DrawRay(transform.position, transform.forward * rayScale, Color.black);

            // Draw a ray showing the rigidbody velocity
            Debug.DrawRay(transform.position, rb.velocity, Color.red);

            // Draw a ray showing line linking NPC and closest chair
            if (closestChair != null)
            {
                Debug.DrawRay(transform.position, closestChair.transform.position - transform.position, Color.green);
            }
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
    /// Function to check for and avoid nearby blockers between NPC and closest chair
    /// </summary>
    /// <param name="target">The chair to move towards</param>>
    private void CheckBlocker(GameObject target)
    {
        Vector3 raystart = new Vector3(transform.position.x, checkBlockerHeight, transform.position.z);

        if (target == null) return; // Ensure there is a target
        if (Physics.Raycast(raystart, target.transform.position - transform.position, out RaycastHit hit, checkBlockerDistance))
        {
            //Check if some object is hit AND it is not the target
            if (hit.collider.gameObject != null && hit.collider.gameObject != target)
            {
                isBlocked = true;
                //Debug.Log("Blocked by: " + hit.collider.gameObject.name);
                //Set identify perpendicular direction to avoid blocker
                Vector3 targetDirection = (target.transform.position - transform.position).normalized;
                //Identify positive or negative perpendicular direction randomly
                Vector3 perpendicularDirection = Vector3.Cross(Vector3.up, targetDirection).normalized * lrPreference;
                //Set velocity at angle between target and perpendicular direction
                Vector3 avoidVelocity = (perpendicularDirection + targetDirection) * moveSpeed;
                //Slowly update velocity to avoid blocker with Lerp
                Vector3 newVelocity = rb.velocity;
                newVelocity.x = avoidVelocity.x;
                newVelocity.z = avoidVelocity.z;
                rb.velocity = Vector3.Lerp(rb.velocity, newVelocity, Time.deltaTime);
                npcVelocity = rb.velocity;
            }
            else
            {
                isBlocked = false;
                //Vector3 newVelocity = rb.velocity;
                //newVelocity.x = 0;
                //newVelocity.z = 0;
                //rb.velocity = newVelocity;
                return;
            }
        }
    }


    /// <summary>
    /// Function to rotate and move towards closest chair object and update state when reached
    /// </summary>
    /// <param name="target">The chair to move towards</param>
    private void MoveTowards(GameObject target)
    {
        if (target == null) return; // Ensure there is a target

        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0; // Ignore the y component for movement

        // Calculate the velocity vector towards the target
        Vector3 velocity = direction.normalized * moveSpeed;

        // Check if we are close enough to the target to consider stopping
        if (direction.magnitude > stoppingDistance)
        {
            // Preserve the current Y velocity and apply the calculated X and Z velocity
            Vector3 newVelocity = rb.velocity;
            newVelocity.x = velocity.x;
            newVelocity.z = velocity.z;
            rb.velocity = newVelocity;
            npcVelocity = rb.velocity;

            // Rotate towards target using MoveRotation for smooth rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
        }
        else
        {
            // Optionally, stop the NPC when it reaches the target
            Vector3 stopVelocity = rb.velocity;
            stopVelocity.x = 0;
            stopVelocity.z = 0;
            rb.velocity = stopVelocity;
            npcVelocity = rb.velocity;
            reachedChair = true; // Update the state to indicate the NPC has reached the chair
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
