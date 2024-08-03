using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    // Input variables (for changing controls)
    public Vector3 MoveInput { get; private set; }
    public Vector2 TargetingInput { get; private set; }
    public bool PowerInput { get; private set; }
    public bool MenuOpenCloseInput { get; private set; }

    // Layer mask for target object
    public LayerMask targetLayerMask;

    //Layer mask for base object
    public LayerMask baseLayerMask;

    // Movement held flag
    public bool movementHeld;

    private PlayerInput playerInput;
    // Controls
    private InputAction moveAction;
    private InputAction powerAction;
    private InputAction menuOpenCloseAction;
    private InputAction targetingAction;

    /// <summary>
    /// Standard Unity function while loading script
    /// </summary>
    public void Awake()
    {
        ResetValuesToDefault();
        #region singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        #endregion

        playerInput = GetComponent<PlayerInput>();

        SetupInputActions();

        //Set default target layer mask to chair and NPC
        targetLayerMask = LayerMask.GetMask("Chair", "NPC");
        Debug.Log("Input Target layer mask is set to:" + targetLayerMask.value.ToString());

        //Set default base layer mask to base
        baseLayerMask = LayerMask.GetMask("Base");
        Debug.Log("Input Base layer mask is set to:" + baseLayerMask.value.ToString());
    }

    private void Update()
    {
        UpdateInputs();
    }

    /// <summary>
    /// Function to initialise input control scheme on game start
    /// </summary>
    private void SetupInputActions()
    {
        moveAction = playerInput.actions["Movement"];
        targetingAction = playerInput.actions["Targeting"];
        powerAction = playerInput.actions["Power"];
        menuOpenCloseAction = playerInput.actions["MenuOpenClose"];
    }

    /// <summary>
    /// Function to update input control scheme to match what the latest player action was
    /// </summary>
    private void UpdateInputs()
    {
        MoveInput = moveAction.ReadValue<Vector3>();
        TargetingInput = targetingAction.ReadValue<Vector2>();
        PowerInput = powerAction.WasPressedThisFrame();
        MenuOpenCloseInput = menuOpenCloseAction.WasPressedThisFrame();
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

        //capture if the movement button is being held down in a boolean flag
        if (horizontalMoveAxis != 0 || verticalMoveAxis != 0)
        {
            movementHeld = true;
        }
        else
        {
            movementHeld = false;
        }
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
        // Identify object hit by the ray in layermask
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayerMask))
        {
            // Get the position of the object hit by the ray
            Vector3 targetPosition = hit.point;
            // Set the target object to the object hit by the ray
            targetObject = hit.collider.gameObject;
            // Set the target axis to the position of the object hit by the ray
            horizontalTargetAxis = targetPosition.x;
            verticalTargetAxis = targetPosition.z;
        }
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, baseLayerMask))
        {
            Vector3 targetPosition = hit.point;
            horizontalTargetAxis = targetPosition.x;
            verticalTargetAxis = targetPosition.z;
            Debug.Log("Base hit with input position x:"+horizontalTargetAxis+" and z:"+verticalTargetAxis);
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
}
