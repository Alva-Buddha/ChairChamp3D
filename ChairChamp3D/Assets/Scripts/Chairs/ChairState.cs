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
    private void OnTriggerEnter(Collider collision)
    {
        Color currentColor = GetComponent<MeshRenderer>().material.color;


        if (!isOccupied)
        {
            // Check if the object colliding with the chair is an NPC
            if (LayerMask.LayerToName(collision.gameObject.layer) == "NPC")
            {
                // Set the chair to occupied
                isOccupied = true;
                // Change the material color to red
                GetComponent<MeshRenderer>().material.color = new Color(Color.red.r, Color.red.g, Color.red.b, currentColor.a);
            }
            // Check if the object colliding with the chair is the player
            else if (LayerMask.LayerToName(collision.gameObject.layer) == "Player")
            {
                // Set the chair to occupied
                isOccupied = true;
                // Change the material color to green
                GetComponent<MeshRenderer>().material.color = new Color(Color.green.r, Color.green.g, Color.green.b, currentColor.a);
            }
        }
    }
}
