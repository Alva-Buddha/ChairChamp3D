using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour
{
    //create inputManager object
    private InputManager inputManager;

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
        //Timers for cooldowns
        dashTimer = 0.0f;
        pullTimer = 0.0f;
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
        //Move this object towerd's the input manager's target axis position by dash distance
        this.transform.position = Vector3.MoveTowards(this.transform.position, 
            new Vector3(inputManager.horizontalTargetAxis, this.transform.position.y, inputManager.verticalTargetAxis), dashDistance);
        //Set dash timer to cooldown
        dashTimer = dashCooldown;
    }

    public void Pull()
    {
        Debug.Log("Pull called");
        if (pullTimer > 0)
        {
            Debug.Log("Pull is on cooldown");
            return;
        }
        //Pull the target object identified from the input manager towards this object object
        inputManager.targetObject.transform.position = Vector3.MoveTowards(inputManager.targetObject.transform.position,
                       this.transform.position, pullDistance);
        //Set pull timer to cooldown
        pullTimer = pullCooldown;
    }
}
