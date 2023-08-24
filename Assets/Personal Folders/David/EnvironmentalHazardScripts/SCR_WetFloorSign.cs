using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the behaviour of wet floor signs
public class SCR_WetFloorSign : MonoBehaviour
{
    //time that player is stunned when this object is hit
    [SerializeField] private float stunTime = 1f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Stunned!");

            //call the stun function in the stun player script
            other.GetComponent<SCR_PlayerStats>().StunPlayer(stunTime, false);

            //destroy the sign
            Destroy(gameObject);
        }
    }
}
