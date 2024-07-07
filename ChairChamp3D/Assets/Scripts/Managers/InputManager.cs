using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    /// <summary>
    /// Standard Unity function while loading script
    /// </summary>
    public void Awake()
    {
        ResetValuesToDefault();
        // used to implement singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Function to reset values to default
    /// </summary>
    void ResetValuesToDefault()
    {
        horizontalMoveAxis = default;
        verticalMoveAxis = default;

        horizontalTargetAxis = default;
        verticalTargetAxis = default;

        powerPressed = default;
        powerHeld = default;

        pausePressed = default;
    }

    [Header("Player Movement Input")]
    [Tooltip("The move input along the horizontal")]
    public float horizontalMoveAxis;
    [Tooltip("The move input along the vertical")]
    public float verticalMoveAxis;

    /// <summary>
    /// Read player input for movement
    /// </summary>
    /// <param name="context">Input action callback context to be read for movement</param>
    public void ReadMovementInput(InputAction.CallbackContext context)
    {
        Vector3 inputVector = context.ReadValue<Vector3>();
        horizontalMoveAxis = inputVector.x;
        verticalMoveAxis = inputVector.z;
    }

    [Header("Target Around input")]
    public float horizontalTargetAxis;
    public float verticalTargetAxis;

    [Tooltip("The object that the player is targeting")]
    public GameObject targetObject;

    /// <summary>
    /// Read player input for targeting in a 3D game
    /// </summary>
    /// <param name="context">Input action callback context meant to be read for targeting</param>
    public void ReadMousePositionInput(InputAction.CallbackContext context)
    {
        // Read the mouse position input from the context
        Vector2 inputVector = context.ReadValue<Vector2>();

        // Raycast from the camera to identify colliding objects
        Ray ray = Camera.main.ScreenPointToRay(inputVector);
        // Identify object hit by the ray
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Get the position of the object hit by the ray
            Vector3 targetPosition = hit.point;
            // Set the target object to the object hit by the ray
            targetObject = hit.collider.gameObject;
            // Set the target axis to the position of the object hit by the ray
            horizontalTargetAxis = targetPosition.x;
            verticalTargetAxis = targetPosition.z;
        }
    }

    [Header("Player Power Input")]
    [Tooltip("Whether or not the power button was pressed this frame")]
    public bool powerPressed = false;
    [Tooltip("Whether or not the power button is being held")]
    public bool powerHeld = false;

    /// <summary>
    /// Reads the power input from the player
    /// </summary>
    /// <param name="context">The input action callback context meant to be read for power activation</param>
    public void ReadPowerInput(InputAction.CallbackContext context)
    {
        powerPressed = !context.canceled;
        powerHeld = !context.canceled;
        StartCoroutine("ResetPowerStart");
    }

    /// <summary>
    /// Coroutine that resets the fire pressed variable after one frame
    /// </summary>
    /// <returns>IEnumerator: Allows this to function as a coroutine</returns>
    private IEnumerator ResetPowerStart()
    {
        yield return new WaitForEndOfFrame();
        powerPressed = false;
    }

    [Header("Pause Input")]
    public bool pausePressed;
    public void ReadPauseInput(InputAction.CallbackContext context)
    {
        pausePressed = !context.canceled;
        StartCoroutine(ResetPausePressed());
    }

    /// <summary>
    /// Coroutine that resets the pause pressed variable at the end of the frame
    /// </summary>
    /// <returns>IEnumerator: Allows this to function as a coroutine</returns>
    IEnumerator ResetPausePressed()
    {
        yield return new WaitForEndOfFrame();
        pausePressed = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
