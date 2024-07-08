using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Variables")]
    [Tooltip("The speed at which the player will move.")]
    public float moveSpeed = 10.0f;
    [Tooltip("The speed at which the player rotates")]
    public float rotationSpeed = 60f;
    [Tooltip("Time to start/stop")]
    public float timeToAccelerate = 0.5f;

    [Header("Debugging")]
    [Tooltip("Forward ray visibility")]
    public bool showRay = false;
    [Tooltip("Forward ray scaling")]
    public float rayScale = 2;

    [Header("GameState")]
    [Tooltip("Has the player reached a chair")]
    public bool reachedChair = false;
    [Tooltip("Music-on rotation speed")]
    public float musicRotationSpeed = 60f;

    //The InputManager to read input from
    private InputManager inputManager;

    //The GameManager to read music state from
    private GameManager gameManager;

    //The object's rigidbody
    private Rigidbody rb;

    [Header("Player Power")]
    [Tooltip("The player's power")]
    public Power playerPower;

    // Start is called before the first frame update
    void Start()
    {
        // Get the InputManager component
        SetupInput();
        // Get the GameManager component
        SetupGameManager();
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        // Get the Power component
        playerPower = GetComponent<Power>();    
    }

    ///<summary>
    ///Set up GameManager if it is not already set up. Throws an error if none exists
    ///</summary>
    private void SetupGameManager()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }
        if (gameManager == null)
        {
            Debug.LogWarning("There is no GameManager in the scene, there needs to be one for the Controller to work");
        }
    }


    /// <summary>
    /// Sets up the input manager if it is not already set up. Throws an error if none exists
    /// </summary>
    private void SetupInput()
    {
        if (inputManager == null)
        {
            inputManager = InputManager.instance;
        }
        if (inputManager == null)
        {
            Debug.LogWarning("There is no player input manager in the scene, there needs to be one for the Controller to work");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.musicPlaying == false && reachedChair == false)
        {
            // Collect input and move the player accordingly
            HandleInput();
        }
        else
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
                if (rb.velocity.magnitude > 0.01)
                {
                    //reduce the velocity magnitude based on timeToAccelerate
                    rb.velocity -= rb.velocity * Time.deltaTime / timeToAccelerate;
                }
            }
        }

        if(showRay)
        {
            // Draw a ray forward from the player object in the Scene view
            Debug.DrawRay(transform.position, transform.forward * rayScale, Color.black);
        }

    }

    /// <summary>
    /// Handles input and moves the player accordingly
    /// </summary>
    private void HandleInput()
    {
        // Find the position that the player should target
        Vector2 targetPosition = new Vector3(inputManager.horizontalTargetAxis, 0, inputManager.verticalTargetAxis);
        // Get movement input from the inputManager
        Vector3 movementVector = new Vector3(inputManager.horizontalMoveAxis, 0, inputManager.verticalMoveAxis);
        // Move the player
        MovePlayer(movementVector);

        //Get the powerheld state from input manager and call Power if powerheld is true
        if (inputManager.powerHeld)
        {
            playerPower.ActivatePower();
        }
    }

    /// <summary>
    /// Move player object and rotate in direction of movement
    /// </summary>
    /// <param name="movement">The direction to move the player</param>
    private void MovePlayer(Vector3 movement)
    {
        // Calculate the acceleration vector based on input, move speed, and time to accelerate
        Vector3 acceleration = movement.normalized * moveSpeed / timeToAccelerate;

        // Check if there is movement input
        if (movement != Vector3.zero)
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
                rb.velocity -= rb.velocity * Time.deltaTime/ timeToAccelerate;
            }
        }
    }
}
