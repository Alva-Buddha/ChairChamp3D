using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairState : MonoBehaviour
{
    /// <summary>
    /// variable to check if the chair is occupied
    /// </summary>
    public bool isOccupied = false;
    /// <summary>
    /// variable to link to the GameManager script which controls global variables and settings
    /// </summary>
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        //Finds the GameManager in the scene
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
                //Decrease the unoccupied chairs variable in the GameManager script
                gameManager.unoccupiedChairs--;
                gameManager.npcChairs++;
            }
            //check if the object colliding with the chair is the player
            else if (LayerMask.LayerToName(collision.gameObject.layer) == "Player")
            {
                //set the chair to occupied
                isOccupied = true;
                //change the sprite color to blue
                GetComponent<SpriteRenderer>().color = Color.green;
                //Increase the global score variable in the GameManager script
                gameManager.score++;
                //Decrease the unoccupied chairs variable in the GameManager script
                gameManager.unoccupiedChairs--;
                gameManager.playerChairs++;
            }
        }
    }
}
