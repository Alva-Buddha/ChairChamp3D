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

    //The object's rigidbody
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        chairs = GameObject.FindGameObjectsWithTag("Chair");
        gameManager = GameManager.Instance;
        rb = GetComponent<Rigidbody>();
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

            // Set velocity to move NPC around origin
            rb.velocity = perpendicularDirection * musicMoveSpeed;

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
        if (target == null) return; // Ensure there is a target
        if (Physics.Raycast(transform.position, target.transform.position - transform.position, out RaycastHit hit, checkBlockerDistance))
        {
            //Check if some object is hit AND it is not the target
            if (hit.collider.gameObject != null && hit.collider.gameObject != target)
            {
                isBlocked = true;
                //Debug.Log("Blocked by: " + hit.collider.gameObject.name);
                //Set identify perpendicular direction to avoid blocker
                Vector3 targetDirection = (target.transform.position - transform.position).normalized;
                //Identify positive or negative perpendicular direction randomly
                Vector3 perpendicularDirection = Vector3.Cross(Vector3.up, targetDirection).normalized * Random.Range(-2,2);
                //Set velocity at angle between target and perpendicular direction
                Vector3 avoidVelocity = (perpendicularDirection + targetDirection) * moveSpeed;
                //Slowly update velocity to avoid blocker with Lerp
                rb.velocity = Vector3.Lerp(rb.velocity, avoidVelocity, Time.deltaTime);
            }
            else
            {
                isBlocked = false;
                rb.velocity = Vector3.zero;
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
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z); // Apply velocity while maintaining current y velocity

        // Check if we are close enough to the target to consider stopping
        if (direction.magnitude > stoppingDistance)
        {
            // Rotate towards target using MoveRotation for smooth rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
        }
        else
        {
            // Optionally, stop the NPC when it reaches the target
            rb.velocity = Vector3.zero;
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
