using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//DO NOT USE, USE SCR_PLAYERSTATS
public class SCR_StunPlayer : MonoBehaviour
{
    //reference to the stun animation
    //depending on how it is designed, this may need changing to an animator
    [SerializeField] private Animator playerAnimator;

    [SerializeField] private string stunBoolName = "Stunned";

    //reference to the affected player scripts
    #region PLAYER SCRIPTS
    private SCR_PlayerAttack attackScript;

    private NavMeshAgent movementScript;

    private SCR_WeaponInventory inventory;
    #endregion

    //timer used to track how long the player has been stunned
    private float timer = 0f;

    //how long the player is stunned before they are no longer stunnedd
    private float timeStunned = 0f;
    
    //tracks whether the player is currently stunned
    private bool bStunned = false;

    //placeholder while there is no stun animation, holds the start colour of the player
    private Color initialColour;

    // Start is called before the first frame update
    void Start()
    {
        //initial the player scripts
        attackScript = GetComponent<SCR_PlayerAttack>();
        movementScript = GetComponent<NavMeshAgent>();
        inventory = GetComponent<SCR_WeaponInventory>();

        //get the current colour of the player
        //initialColour = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        //if the player is stunned
        if (bStunned)
        {
            //track the time stunned in the timer
            timer += Time.deltaTime;

            //if the player has been stunned the allocated amount of time
            if(timer >= timeStunned)
            {
                //set the bool to false so the player can be stunned again
                bStunned = false;

                //enable the affected scripts again
                attackScript.enabled = true;

                movementScript.enabled = true;

                inventory.enabled = true;

                if (playerAnimator)
                {
                    playerAnimator.SetBool(stunBoolName, false);
                }

                //reset the timer
                timer = 0f;
            }
        }
    }

    //called when the player has been stunned. Stun time can be unique to the triggered hazard.
    public void StunPlayer(float stunTime)
    {
        //if they're not already stunned
        if (!bStunned)
        {
            //set the stun time
            timeStunned = stunTime;

            //set the stun bool to true to start tracking the stun time, and prevent stunning the player multiple times at once
            bStunned = true;

            if (playerAnimator)
            {
                playerAnimator.SetBool(stunBoolName, true);
            }

            //disable the relevant scripts to create a "stunned" effect
            attackScript.enabled = false;

            movementScript.enabled = false;

            inventory.enabled = false;

            //placeholder for animation, set the player yellow to indicate that they are stunned
            //GetComponent<Renderer>().material.color = Color.yellow;
        }
    }
}
