using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairState : MonoBehaviour
{
    public bool isOccupied = false; // variable to check if the chair is occupied

    private GameManager gameManager; // variable to link to the GameManager script which controls global variables and settings

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
    private void OnTriggerEnter(Collider collision)
    {
        if (!isOccupied)
        {
            //check if the object colliding with the chair is an NPC
            if (LayerMask.LayerToName(collision.gameObject.layer) == "NPC")
            {
                //set the chair to occupied
                isOccupied = true;
                //tell the colliding NPC's SeekEmptyChair script that it has reached the chair
                collision.gameObject.GetComponent<SeekEmptyChair>().reachedChair = true;

                //Decrease the unoccupied chairs variable in the GameManager script
                gameManager.unoccupiedChairs--;
                gameManager.npcChairs++;
            }
            //check if the object colliding with the chair is the player
            else if (LayerMask.LayerToName(collision.gameObject.layer) == "Player")
            {
                //set the chair to occupied
                isOccupied = true;
                //tell the player controller that it has reached the chair
                collision.gameObject.GetComponent<PlayerController>().reachedChair = true;

                //Increase the global score variable in the GameManager script
                ScoreManager.Instance.IncreaseScore(1);
                //Decrease the unoccupied chairs variable in the GameManager script
                gameManager.unoccupiedChairs--;
                gameManager.playerChairs++;
            }
            if (isOccupied)
            {
                //Lock the rigidbody of the chair so it cannot be moved
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }
}
