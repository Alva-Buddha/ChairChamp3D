using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public GameObject pickupEffect;
    public float vfxDuration = 4f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Pickup(other));
        }
    }

    IEnumerator Pickup(Collider player)
    {
        // Spawn cool effect
        if (pickupEffect == null)
        {
            Debug.Log("No pickup VFX assigned to PowerupPickup object");
        }
        else
        {
            Instantiate(pickupEffect, transform.position, transform.rotation);
        }

        // Apply effect to the player
        PlayerController powerups = player.GetComponent<PlayerController>();

        // Create a variable that gets all power types
        Power.PowerType[] powerTypes = (Power.PowerType[])System.Enum.GetValues(typeof(Power.PowerType));

        // Create a list to hold the eligible power types (exclude None and the current power)
        List<Power.PowerType> eligiblePowerTypes = new();

        foreach (Power.PowerType powerType in powerTypes)
        {
            if (powerType != Power.PowerType.None && powerType != powerups.playerPower.currentPower)
            {
                eligiblePowerTypes.Add(powerType);
            }
        }

        // Get a random power type from the eligible power types
        Power.PowerType randomPower = eligiblePowerTypes[UnityEngine.Random.Range(0, eligiblePowerTypes.Count)];

        // Assign the random power to the player
        powerups.playerPower.currentPower = randomPower;

        // Disable the powerup visuals and collisions so that attached VFX can continue to play
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // Remove power-up object after X seconds
        yield return new WaitForSeconds(vfxDuration);
        Destroy(gameObject);
    }
}
