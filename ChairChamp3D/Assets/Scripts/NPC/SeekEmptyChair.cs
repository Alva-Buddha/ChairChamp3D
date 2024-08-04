using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    // The GameManager to read music state from
    private GameManager gameManager;

    // The closest chair to the NPC
    private GameObject closestChair = null;

    // Get array of chair objects in scene
    private GameObject[] chairs = null;

    [Header("Obstacle Avoidance")]
    [Tooltip("Distance to check for nearby blockers")]
    public float checkNearBlockerDistance = 2.0f;
    [Tooltip("Distance to check for faraway blockers")]
    public float checkFarBlockerDistance = 12.0f;
    [Tooltip("Y height of raycast to check for blockers")]
    public float checkBlockerHeight = 0.5f;
    [Tooltip("Number of rays to cast for nearby blockers")]
    public int numNearRays = 5;
    [Tooltip("Number of rays to cast for distant blockers")]
    public int numFarRays = 10;

    [Tooltip("Time to check while avoiding getting stuck")]
    public float checkStuckTime = 1f;
    [Tooltip("Distance to check to determine if stuck")]
    public float checkStuckDistance = 0.5f;

    // Flags to check if coroutines are running
    private bool checkStuckRunning = false;
    private bool checkUnStuckRunning = false;

    [Tooltip("Time to spend getting unstuck")]
    public float unstuckTime = 3f;

    [Tooltip("Flag if NPC is stuck")]
    public bool isStuck = false;

    [Tooltip("Threshold to consider for smooth movement")]
    public float smoothThreshold = 0.1f;

    //Array of nearby hit points from raycast
    private RaycastHit[] nearHits;

    //Array of distant hit points from raycast
    private RaycastHit[] farHits;

    [Header("Debugging")]
    [Tooltip("Forward ray visibility")]
    public bool showRay = true;
    [Tooltip("Forward ray scaling")]
    public float rayScale = 2;
    [Tooltip("Check variable for NPC velocity")]
    public Vector3 npcVelocity;
    [Tooltip("Check variable for any direction in the script")]
    public Vector3 debugDir;

    // The object's rigidbody
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        chairs = GameObject.FindGameObjectsWithTag("Chair");
        gameManager = GameManager.Instance;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        #region musicPlaying
        if (gameManager.musicPlaying)
        {
            //Call function to guide behavior while music is playing
            MusicMove();
        }
        #endregion
        #region !musicPlaying
        else
        {
            if (!reachedChair)
            {
                // Find closest unoccupied chair
                closestChair = FindClosestChair();
                // Function to check for nearby blocker between NPC and closestChair
                SmartApproach(closestChair);
                npcVelocity = rb.velocity;
            }
            else if (reachedChair)
            {
                rb.velocity = UpdateVector(rb.velocity, Vector3.zero);
                npcVelocity = rb.velocity;
            }
        }
        //Show ray for npc velocity
        if (showRay)
        {
            Debug.DrawRay(transform.position, npcVelocity * rayScale, Color.red);
        }
        #endregion
    }

    /// <summary>
    /// Function to find closest chair
    /// </summary>
    /// <returns>closest chair gameobject</returns>
    private GameObject FindClosestChair()
    {
        // Initialize closest distance to infinity
        float closestDistance = Mathf.Infinity;
        // Loop through all chairs
        foreach (GameObject chair in chairs)
        {
            // Get distance to chair
            float distance = Vector3.Distance(transform.position, chair.transform.position);
            // Check if chair is unoccupied and closer than the current closest chair
            if (!chair.GetComponent<ChairState>().isOccupied && distance < closestDistance)
            {
                // Set closest chair to current chair
                closestChair = chair;
                // Set closest distance to current distance
                closestDistance = distance;
            }
        }
        //If showRay draw a flat ray from NPC to closest chair
        if (showRay)
        {
            Debug.DrawRay(transform.position, FlattenVector(closestChair.transform.position - transform.position), Color.green);
        }
        // Return closest chair
        return closestChair;
    }

    /// <summary>
    /// Function to rotate and move towards closest chair object and update state when reached
    /// Stops when distance to target is closer than stopping distance
    /// </summary>
    /// <param name="target">The chair to move towards</param>
    /// <param name="stopDistance">The distance at which to stop moving</param>
    private void MoveTowards(GameObject target, Vector3 direction, float stopDistance)
    {
        if (target == null) return; // Ensure there is a target

        // Calculate the velocity vector towards the target
        Vector3 velocity = direction.normalized * moveSpeed;

        // Check if we are close enough to the target to consider stopping
        if (FlattenVector(target.transform.position - transform.position).magnitude > stopDistance)
        {
            // Preserve the current Y velocity and apply the calculated X and Z velocity
            rb.velocity = UpdateVector(rb.velocity, velocity);

            // Rotate towards target using MoveRotation for smooth rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
        }
        else
        {
            // Optionally, stop the NPC when it reaches the target
            rb.velocity = UpdateVector(rb.velocity, Vector3.zero);
            reachedChair = true; // Update the state to indicate the NPC has reached the chair
        }
    }

    // Checks if the NPC has reached the chair and update flag
    private void OnTriggerEnter(Collider collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Chair")
        {
            if (collision.gameObject.GetComponent<ChairState>().isOccupied == false)
            {
                reachedChair = true;
                closestChair = collision.gameObject;
            }
        }
    }

    // Function to move NPC with movespeed in weighted average direction based on 
    // 1. Direction to target (0.5 weight)
    // 2. Direction to avoid nearby obstacle around approach to target (0.4 weight)
    // 3. Direction to avoid far away obstacle around approach to target (0.1 weight)
    // If NPC is blocked in target direction, target direction weight is set to 0.4
    private void SmartApproach(GameObject target)
    {
        Vector3 targetDirection = FlattenVector(target.transform.position - transform.position).normalized;
        Vector3 weightedDirection = Vector3.zero;

        if (!isStuck)
        {
            // Calculate the direction to avoid nearby obstacles
            Vector3 avoidNearbyDirection = AvoidNearObstacle(targetDirection, target);
            // Calculate the direction to avoid far away obstacles
            Vector3 avoidFarDirection = AvoidFarObstacle(targetDirection, target);
            // Calculate the weighted average of the three directions
            weightedDirection = 0.5f * targetDirection + 0.4f * avoidNearbyDirection + 0.1f * avoidFarDirection;
            // Call CheckStuck coroutine to check if NPC is stuck
            if (!IsInvoking(nameof(CheckStuck)) && !checkStuckRunning)
            {
                //Debug.Log(this.name + ": Starting CheckStuck coroutine at: " + Time.time);
                StartCoroutine(CheckStuck());
            }
        }
        else
        {
            // Replace target direction with perpendicular direction to avoid getting stuck
            Vector3 perpendicularDirection = Vector3.Cross(Vector3.up, targetDirection).normalized;
            // Calculate the direction to avoid nearby obstacles
            Vector3 avoidNearbyDirection = AvoidNearObstacle(perpendicularDirection, target);
            // Calculate the direction to avoid far away obstacles
            Vector3 avoidFarDirection = AvoidFarObstacle(perpendicularDirection, target);
            weightedDirection = 0.5f * perpendicularDirection + 0.4f * avoidNearbyDirection + 0.1f * avoidFarDirection;
            // Start coroutine to check passage of time and update isStuck flag
            if (!IsInvoking(nameof(CheckUnStuck)) && !checkUnStuckRunning)
            {
                Debug.Log(this.name + ": Starting CheckUnStuck coroutine at: " + Time.time);
                StartCoroutine(CheckUnStuck());
            }
        }

        // If weighted direction is greater than a small threshold then initiate MoveTowards
        if (weightedDirection.magnitude < smoothThreshold)
        {
            weightedDirection = Vector3.zero;
        }
        else
        {
            // Move towards the target using the weighted direction
            MoveTowards(target, weightedDirection.normalized, stoppingDistance);
        }
    }

    /// <summary>
    /// Coroutine to compare NPC's position now vs before checkStuckTime seconds
    /// If position has changed less than checkStuckDistance, NPC is stuck
    /// </summary>
    IEnumerator CheckStuck()
    {
        // Set running flag to true
        checkStuckRunning = true;
        Vector3 initialPosition = transform.position;
        yield return new WaitForSeconds(checkStuckTime);
        Vector3 finalPosition = transform.position;
        if (Vector3.Distance(initialPosition, finalPosition) < checkStuckDistance && !reachedChair)
        {
            isStuck = true;
            // Log time of changing isStuck flag to true
            Debug.Log(this.name + ": Detected stuck at: " + Time.time);
        }
        // Set running flag to false
        checkStuckRunning = false;
        //Debug.Log(this.name + ": CheckStuck coroutine completed at: " + Time.time);
    }

    /// <summary>
    /// Coroutine to update isStuck flag after unstuckTime seconds
    /// </summary>
    IEnumerator CheckUnStuck()
    {
        // Set running flag to true
        checkUnStuckRunning = true;
        yield return new WaitForSeconds(unstuckTime);
        isStuck = false;
        // Log time of changing isStuck flag to false
        Debug.Log(this.name + ": Unstuck at: " + Time.time);
        // Set running flag to false
        checkUnStuckRunning = false;
    }

    // Function to avoid nearby obstacles
    private Vector3 AvoidNearObstacle(Vector3 targetDir, GameObject target)
    {
        nearHits = RaycastAroundDirection(targetDir, numNearRays, checkNearBlockerDistance, checkBlockerHeight);

        Vector3 avoidDir = Vector3.zero;
        // Iterate over nearby hits and calculate the average direction to avoid obstacles
        for (int i = 0; i < nearHits.Length; i++)
        {
            if (nearHits[i].collider != null && nearHits[i].collider.gameObject != target)
            {
                Vector3 hitDirection = FlattenVector(nearHits[i].point - transform.position).normalized;
                avoidDir -= hitDirection;
            }
        }
        return avoidDir.normalized;
    }

    // Function to avoid distant obstacles
    private Vector3 AvoidFarObstacle(Vector3 targetDir, GameObject target)
    {
        farHits = RaycastAroundDirection(targetDir, numFarRays, checkFarBlockerDistance, checkBlockerHeight);
        Vector3 avoidDir = Vector3.zero;
        // Iterate over distant hits and calculate the average direction to avoid obstacles
        for (int i = 0; i < farHits.Length; i++)
        {
            if (farHits[i].collider != null && farHits[i].collider.gameObject != target)
            {
                Vector3 hitDirection = FlattenVector(farHits[i].point - transform.position).normalized;
                avoidDir -= hitDirection;
            }
        }
        return avoidDir.normalized;
    }

    /// <summary>
    /// Loop to initiate specified number of raycasts around target direction, till a specified distance, at a specified height, and return hits
    /// </summary>
    /// <param name="direction">Direction to cast rays around</param>"
    /// <param name="distance">Distance to cast rays</param>
    /// <param name="height">Height to cast rays</param>
    private RaycastHit[] RaycastAroundDirection(Vector3 direction, int numRays, float distance, float height)
    {
        RaycastHit[] hits = new RaycastHit[numRays];
        float factor = 0.0f;
        if (distance < 5f)
        {
            factor = 18f;
        }
        Vector3 startDirection = Quaternion.AngleAxis(-90 + factor, Vector3.up) * direction;
        for (int i = 0; i < numRays; i++)
        {
            Vector3 rayDirection = Quaternion.AngleAxis(i * 150 / numRays, Vector3.up) * startDirection;
            Ray ray = new Ray(FlattenVector(transform.position) + new Vector3(0, height, 0), rayDirection);
            Physics.Raycast(ray, out hits[i], distance);
            if (showRay)
            {
                Debug.DrawRay(ray.origin, ray.direction * distance, Color.blue);
            }
        }
        return hits;
    }

    /// <summary>
    /// Function to set y component of a vector3 to 0 and return the resulting vector3
    /// </summary>
    /// <returns></returns>
    private Vector3 FlattenVector(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    /// <summary>
    /// Updates original vector with the x and z components of the update vector
    /// </summary>
    /// <param name="original">Vector which provides y components</param>
    /// <param name="update">Vector which provides x and z components</param>
    /// <returns></returns>
    private Vector3 UpdateVector(Vector3 original, Vector3 update)
    {
        return new Vector3(update.x, original.y, update.z);
    }

    // Function to guide behavior while music is playing
    private void MusicMove()
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
}
