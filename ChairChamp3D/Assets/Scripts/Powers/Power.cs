using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour
{
    //create inputManager object
    private InputManager inputManager;
    private AudioManager audioManager;

    [Tooltip("The types of power possible")]
    public enum PowerType
    {
        None,
        Dash,
        Pull
    }

    [Tooltip("The type of power the player has")]
    public PowerType currentPower = PowerType.None;
    [Tooltip("Distance to dash in direction of the mouse")]
    public float dashDistance = 5.0f;
    [Tooltip("Dash cooldown")]
    public float dashCooldown = 1.0f;
    [Tooltip("Distance to pull in direction of player")]
    public float pullDistance = 5.0f;
    [Tooltip("Pull cooldown")]
    public float pullCooldown = 1.0f;

    //pull layer mask
    public LayerMask pullMask;

    [Header("Cooldown timers - do not change")]
    public float dashTimer = 0f;
    public float pullTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //initialize inputManager
        SetupInput();

        //initialize audioManager
        SetupAudio();

        //Timers for cooldowns
        dashTimer = 0.0f;
        pullTimer = 0.0f;

        #region Set up audio
        // Find the AudioManager gameobject using the 'Audio' tag
        GameObject audioManagerObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioManagerObject != null)
        {
            audioManager = audioManagerObject.GetComponent<AudioManager>();
        }
        else
        {
            Debug.LogError("No GameObject with tag 'Audio' found in the scene.");
        }
        #endregion
    }

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

    private void SetupAudio()
    {
        if (audioManager == null)
        {
            audioManager = AudioManager.instance;
        }
        if (audioManager == null)
        {
            Debug.LogWarning("There is no player audio manager in the scene, there needs to be one for the Controller to work");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if inputmanager power is held call activate power function - handled by player controller
        //if (inputManager.powerHeld)
        //{
        //    Debug.Log("Activate Power called");
        //    ActivatePower();
        //}
        //if timers are greater than 0, decrease them by time.deltatime
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
        }
        if (pullTimer > 0)
        {
            pullTimer -= Time.deltaTime;
        }
    }

    //function to activate power
    public void ActivatePower()
    {
        //switch statement to determine power type
        switch (currentPower)
        {
            //if power type is dash
            case PowerType.Dash:
                //call dash function
                Dash();
                break;
            //if power type is pull
            case PowerType.Pull:
                //call pull function
                Pull();
                break;
            default:
                Debug.Log("No power selected or power is None");
                break;
        }
    }

    //function to dash
    public void Dash()
    {
        Debug.Log("Dash called");
        if (dashTimer > 0)
        {
            Debug.Log("Dash is on cooldown");
            return;
        }
        //Move this object towards the input manager's target axis position by dash distance
        this.transform.position = Vector3.MoveTowards(this.transform.position, 
            new Vector3(inputManager.horizontalTargetAxis, this.transform.position.y, inputManager.verticalTargetAxis), dashDistance);
        //Set dash timer to cooldown
        dashTimer = dashCooldown;
        // Play sound effect
        audioManager.PlayDashSFX();
    }

    public void Pull()
    {
        Debug.Log("Pull called");
        if (pullTimer > 0)
        {
            Debug.Log("Pull is on cooldown");
            return;
        }
        // Check if inputManager target object belongs to layerMask
        bool isTargetPullable = IsLayerInLayerMask(inputManager.targetObject.layer, pullMask);
        if (!isTargetPullable)
        {
            Debug.Log("Target object is not pullable");
            return;
        }
        //Pull the target object identified from the input manager towards this object object
        inputManager.targetObject.transform.position = Vector3.MoveTowards(inputManager.targetObject.transform.position,
                       this.transform.position, pullDistance);
        //Set pull timer to cooldown
        pullTimer = pullCooldown;
        // Play sound effect
        audioManager.PlayPullSFX();
    }

    // Helper method to check if a layer is in a given LayerMask
    bool IsLayerInLayerMask(int layer, LayerMask layerMask)
    {
        // Convert the object's layer to a bitfield for comparison
        int layerBit = 1 << layer;
        // Perform a bitwise AND with the layerMask and check if the result is not zero
        bool isInMask = (layerMask.value & layerBit) != 0;
        return isInMask;
    }
}
