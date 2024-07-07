using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Variables")]
    [Tooltip("The speed at which the player will move.")]
    public float moveSpeed = 10.0f;
    [Tooltip("The speed at which the player rotates in asteroids movement mode")]
    public float rotationSpeed = 60f;

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


    // Start is called before the first frame update
    void Start()
    {
        // Get the InputManager component
        SetupInput();
        // Get the GameManager component
        SetupGameManager();
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
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
                // Rotate the player around origin at constant speed
                transform.RotateAround(Vector3.zero, Vector3.up, musicRotationSpeed * Time.deltaTime);
                // Face the origin
                transform.LookAt(Vector3.zero);
            }
            else
            {
                // Stop the player from moving
                rb.velocity = Vector3.zero;
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
        //TargetPoint(targetPosition);
    }

    /// <summary>
    /// Move player object and rotate in direction of movement
    /// </summary>
    /// <param name="movement">The direction to move the player</param>
    private void MovePlayer(Vector3 movement)
    {
        // Move the player
        transform.position += movement * moveSpeed * Time.deltaTime;

        // Rotate the player to face the direction of movement
        if (movement != Vector3.zero)
        {
            // Calculate the target angle in degrees
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;

            //Log to see target angle z+ is 0 degrees and x+ is +90 degrees
            //Debug.Log("Target Angle: " + targetAngle);

            // Get the current rotation and the target rotation
            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            // Rotate towards the target rotation at the specified rotation speed
            transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
