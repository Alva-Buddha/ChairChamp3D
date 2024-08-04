using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardStun : MonoBehaviour
{
    public float stunDuration = 2.1f;

    /// <summary>
    /// Function to check if an NPC or player has collided with a stunning hazard
    /// </summary>
    /// <param name="collision">The object that the hazard is colliding with</param>
    private void OnTriggerEnter(Collider collision)
    {
        //check if the object colliding with the hazard is an NPC
        if (LayerMask.LayerToName(collision.gameObject.layer) == "NPC")
        {
            //tell the colliding NPC's SeekEmptyChair script that it has hit the hazard
                //<<<REFERENCE TO THE SEEKEMPTYCHAIR SCRIPT THAT COPIES THE PLAYER CODE BELOW>>>
        }
        //check if the object colliding with the hazard is the player
        else if (LayerMask.LayerToName(collision.gameObject.layer) == "Player")
        {
            //tell the player controller that it has hit the hazard
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.StartCoroutine(playerController.StunPlayer(stunDuration));
            }
        }
        // set the child object's "Stunned" variable
        Animator childAnimator = collision.gameObject.transform.GetChild(0).GetComponent<Animator>();
        childAnimator.SetTrigger("Stunned");
    }
}
