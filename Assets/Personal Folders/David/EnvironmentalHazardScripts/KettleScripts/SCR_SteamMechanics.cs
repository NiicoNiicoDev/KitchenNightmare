using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//controls the steam behaviour
public class SCR_SteamMechanics : MonoBehaviour
{
    //how long the player is stunned for if touched
    [SerializeField] private float stunTime = 1f;

    [SerializeField] private bool shouldDealDamage = false;

    private void OnTriggerEnter(Collider other)
    {
        //if the player collides with the steam
        if (other.CompareTag("Player"))
        {
            Debug.Log("Stunned!");

            //call the stun function from the respective script
            other.GetComponent<SCR_PlayerStats>().StunPlayer(stunTime, false);

            if (shouldDealDamage)
            {
                other.GetComponent<SCR_PlayerStats>().TakeDamage(25);
            }
        }
    }
}
