using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairState : MonoBehaviour
{
    //variable to check if the chair is occupied
    public bool isOccupied = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 

    }

    /// <summary>
    /// Function to check collision with NPC or player, and change occupied flag and sprite color based on layer of colliding object
    /// </summary>
    /// <param name="collision">The collider that the chair is colliding with</param>
    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (!isOccupied)
        {
            //check if the object colliding with the chair is an NPC
            if (LayerMask.LayerToName(collision.gameObject.layer) == "NPC")
            {
                //set the chair to occupied
                isOccupied = true;
                //change the sprite color to red
                GetComponent<SpriteRenderer>().color = Color.red;
            }
            //check if the object colliding with the chair is the player
            else if (LayerMask.LayerToName(collision.gameObject.layer) == "Player")
            {
                //set the chair to occupied
                isOccupied = true;
                //change the sprite color to blue
                GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }
}
