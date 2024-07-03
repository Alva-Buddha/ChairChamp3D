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

    //The InputManager to read input from
    private InputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        // Get the InputManager component
        SetupInput();
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
        // Collect input and move the player accordingly
        HandleInput();
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
            // Calculate the target angle in degrees, -90 is adjusted to align with sprite facing up
            float targetAngle = Mathf.Atan2(movement.z, movement.x) * Mathf.Rad2Deg - 90;

            // Get the current rotation and the target rotation
            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            // Rotate towards the target rotation at the specified rotation speed
            transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
