using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardSlow : MonoBehaviour
{
    [Tooltip("The percentage of their full movespeed the player will move when slowed.")]
    public float slowPercent = 0.5f;

    /// <summary>
    /// Function to check if an NPC or player has collided with a pulling hazard/gets too close
    /// </summary>
    /// <param name="collision">The object that the hazard is colliding with</param>
    private void OnTriggerEnter(Collider collision)
    {
        //check if the object colliding with the hazard is an NPC
        if (LayerMask.LayerToName(collision.gameObject.layer) == "NPC")
        {
            //tell the colliding NPC's SeekEmptyChair script that it has hit the hazard
            SeekEmptyChair npcController = collision.gameObject.GetComponent<SeekEmptyChair>();
            if (npcController != null)
            {
                npcController.SlowNPC(slowPercent);
            }
        }
        //check if the object colliding with the hazard is the player
        else if (LayerMask.LayerToName(collision.gameObject.layer) == "Player")
        {
            //tell the player controller that it has hit the hazard
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SlowPlayer(slowPercent);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        //check if the object colliding with the hazard is an NPC
        if (LayerMask.LayerToName(collision.gameObject.layer) == "NPC")
        {
            //tell the colliding NPC's SeekEmptyChair script that it has hit the hazard
            SeekEmptyChair npcController = collision.gameObject.GetComponent<SeekEmptyChair>();
            if (npcController != null)
            {
                npcController.UnSlowNPC(slowPercent);
            }
        }
        //check if the object colliding with the hazard is the player
        else if (LayerMask.LayerToName(collision.gameObject.layer) == "Player")
        {
            //tell the player controller that it has hit the hazard
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.UnSlowPlayer(slowPercent);
            }
        }
    }
}
