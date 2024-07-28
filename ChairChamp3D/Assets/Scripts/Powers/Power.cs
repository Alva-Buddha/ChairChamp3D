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
        Pull,
        Stun,
        Swap,
    }

    [Tooltip("Duration of the power")]
    public float powerTime = 0.5f; // Duration in seconds

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
    [Tooltip("Stun time")]
    public float stunTime = 2.0f;
    [Tooltip("Stun cooldown")]
    public float stunCooldown = 1.0f;
    [Tooltip("Swap cooldown")]
    public float SwapCooldown = 1.0f;

    //pull layer mask
    public LayerMask pullMask;

    [Header("Cooldown timers - do not change")]
    public float dashTimer = 0f;
    public float pullTimer = 0f;
    public float stunTimer = 0f;
    public float swapTimer = 0f;

    //rigidbody for this object
    private Rigidbody rb;

    //rigidbody for target object
    private Rigidbody targetRb;

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
        stunTimer = 0.0f;
        swapTimer = 0.0f;

        //get rigidbody component
        rb = GetComponent<Rigidbody>();

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
        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
        }
        if (swapTimer > 0)
        {
            swapTimer -= Time.deltaTime;
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
            //if power type is stun
            case PowerType.Stun:
                //call stun function
                Stun();
                break;
            //if power type is swap
            case PowerType.Swap:
                //call swap function
                Swap();
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

        // Calculate dash direction based on input manager's target axis
        Vector3 dashDirection = new Vector3(inputManager.horizontalTargetAxis, 0, inputManager.verticalTargetAxis).normalized;

        // Calculate the velocity needed to cover the dashDistance in powerTime
        Vector3 dashVelocity = dashDirection * (dashDistance / powerTime);

        // Set the Rigidbody's velocity to dashVelocity
        rb.velocity = dashVelocity;

        // Start the coroutine to reset the velocity after powerTime
        StartCoroutine(ResetVelocityAfterTime(powerTime));

        // Set dash timer to cooldown
        dashTimer = dashCooldown;
        // Play sound effect
        audioManager.PlayDashSFX();
    }

    // Coroutine to reset the Rigidbody's velocity after a specified time
    private IEnumerator ResetVelocityAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        // Reset the velocity to zero or to the Rigidbody's initial velocity if needed
        rb.velocity = Vector3.zero;
    }


    //function to pull target object
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
        // Get target object's Rigidbody component
        targetRb = inputManager.targetObject.GetComponent<Rigidbody>();
        if (targetRb != null)
        {
            // Start the coroutine to pull the target towards the player
            StartCoroutine(PullTargetOverTime(inputManager.targetObject.transform, powerTime));
        }
        else
        {
            Debug.Log("Target object does not have a Rigidbody");
        }
        // Set pull timer to cooldown
        pullTimer = pullCooldown;
        // Play sound effect
        audioManager.PlayPullSFX();
    }

    private IEnumerator PullTargetOverTime(Transform target, float duration)
    {
        float time = 0;
        Vector3 startPosition = target.position;
        Vector3 endPosition = this.transform.position;

        while (time < duration)
        {
            time += Time.deltaTime;
            // Calculate the pull force or velocity based on the elapsed time
            Vector3 direction = (endPosition - startPosition).normalized;
            float pullSpeed = pullDistance / duration; // Ensure the target is pulled over the specified duration
            targetRb.velocity = direction * pullSpeed; // Apply the calculated velocity

            yield return null; // Wait until the next frame
        }

        // Optionally, stop the target's movement after pulling
        targetRb.velocity = Vector3.zero;
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

    //function to stun target object for duration = stunTime
    public void Stun()
    {
        Debug.Log("StunTarget called for " + inputManager.targetObject.name);

        // Check if stun is on cooldown
        if (stunTimer > 0)
        {
            Debug.Log("Stun is on cooldown");
            return;
        }

        // Get the target Object's rigidbody component
        targetRb = inputManager.targetObject.GetComponent<Rigidbody>();

        if (targetRb != null)
        {
            // Call coroutine to set target object velocity to zero and lock constraints for stunTime
            StartCoroutine(StunTargetOverTime(targetRb, stunTime));
        }
        else
        {
            Debug.Log("Target object does not have a Rigidbody");
        }

        // Set stun timer to cooldown
        stunTimer = stunCooldown;

        // Play sound effect
        audioManager.PlayStunSFX();
    }

    // Coroutine to set target object velocity to zero and lock constraints for stunTime
    private IEnumerator StunTargetOverTime(Rigidbody targetRbLocal, float duration)
    {
        Debug.Log("StunTargetOverTime called");

        // Store the original constraints
        RigidbodyConstraints originalConstraints = targetRbLocal.constraints;

        // Set target object velocity to zero
        targetRbLocal.velocity = Vector3.zero;

        // Lock all constraints to freeze the target
        targetRbLocal.constraints = RigidbodyConstraints.FreezeAll;

        // Wait for the duration of the stun
        yield return new WaitForSeconds(duration);

        // Restore the original constraints
        targetRbLocal.constraints = originalConstraints;

        Debug.Log("StunTargetOverTime ended");
    }


    //function to swap position with target object
    public void Swap()
    {
        Debug.Log("SwapPosition called with target");

        // Check if swap is on cooldown
        if (swapTimer > 0)
        {
            Debug.Log("Swap is on cooldown");
            return;
        }

        // Get the target object's position
        Vector3 targetPosition = inputManager.targetObject.transform.position;

        // Swap the player's position with the target object's position
        Vector3 playerPosition = this.transform.position;
        this.transform.position = targetPosition;
        inputManager.targetObject.transform.position = playerPosition;

        // Set swap timer to cooldown
        swapTimer = SwapCooldown;

        // Play sound effect
        audioManager.PlaySwapSFX();
    }
}


