using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Variables")]
    [Tooltip("The speed at which the player will move.")]
    public float moveSpeed = 10.0f;
    [Tooltip("The speed at which the player rotates ")]
    public float rotationSpeed = 60f;
    [Tooltip("The speed at which the player moves during music")]
    public float musicMoveSpeed = 5f;

    [Header("Debugging")]
    [Tooltip("Forward ray visibility")]
    public bool showRay = false;
    [Tooltip("Forward ray scaling")]
    public float rayScale = 2;

    [Header("GameState")]
    [Tooltip("Has the player reached a chair")]
    public bool reachedChair = false;

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
                Vector3 Origin = new Vector3(0, rb.position.y, 0);
                // Calculate direction from player to (0, rb.y, 0)
                Vector3 directionToOrigin = (Origin - rb.position).normalized;

                // Calculate perpendicular direction for circular movement around origin
                Vector3 perpendicularDirection = Vector3.Cross(Vector3.up, directionToOrigin).normalized;

                // Set velocity to move player around origin
                rb.velocity = perpendicularDirection * musicMoveSpeed;

                // Calculate angular velocity for facing the origin
                // Determine the target rotation to face the origin
                Quaternion targetRotation = Quaternion.LookRotation(-directionToOrigin, Vector3.up);

                // Calculate the angular velocity needed to rotate the player towards the target rotation
                Quaternion deltaRotation = targetRotation * Quaternion.Inverse(rb.rotation);
                deltaRotation.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
                angleInDegrees = Mathf.DeltaAngle(0, angleInDegrees);
                Vector3 angularVelocity = (Mathf.Deg2Rad * angleInDegrees / Time.fixedDeltaTime) * rotationAxis.normalized;

                // Apply the calculated angular velocity
                rb.angularVelocity = angularVelocity;
            }
            else
            {
                // Stop the player from moving
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
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
    /// Move player object and rotate in direction of movement dynamically using Rigidbody
    /// </summary>
    /// <param name="movement">The direction to move the player</param>
    private void MovePlayer(Vector3 movement)
    {
        // Calculate the velocity vector based on input and move speed
        Vector3 velocity = movement * moveSpeed;

        // Apply the calculated velocity to the Rigidbody component for dynamic movement
        rb.velocity = velocity;

        //debugging ray to show forward direction of object
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);

        //debugging ray to show velocity direction
        Debug.DrawRay(transform.position, rb.velocity, Color.green);

        // Check if there is movement to determine if rotation should occur
        if (movement != Vector3.zero)
        {
            // Calculate the target angle in degrees
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;

            // Create a target rotation based on the target angle
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            // Smoothly rotate towards the target rotation using Rigidbody.MoveRotation
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime));
        }
        else
        {
            // Optionally, handle stopping more smoothly
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 5);
        }
    }
}
