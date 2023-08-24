using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Nori_Attack2State : SCR_AI_NoriBaseState
{
    SCR_AI_NoriSheet noriSheetScript;
    Transform noriTransform;
    LayerMask playerLayerMask;
    RaycastHit hit;
    float damage;
    float timer;
    Vector3 verticalOffset;
    Vector3 aoePosition;
    bool bHasDealtDamage;

    public override void StartState(GameObject noriSheet, NavMeshAgent meshAgent)
    {
        //Debug.Log("Nori Sheet Slap Attack State");
        if(noriSheetScript == null) //If the noriSheetScript is null then this state has not run before
        {
            noriSheetScript = noriSheet.GetComponent<SCR_AI_NoriSheet>(); //Get the NoriSheet script and save it to a local variable
            noriTransform = noriSheet.GetComponent<Transform>(); //Get the Nori Sheet's transform and save it to a local variable
            //healthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_PlayerHealth>();

            damage = noriSheetScript.slapDamage; //Set the damage varialbe to the slapDamage value
            playerLayerMask = noriSheetScript.EnemyStats.PlayerLayerMask; //Set the player's layer mask
            verticalOffset = new Vector3(0f, 0.5f, 0f); //Sets the vertical offset for all raycasts, this is because the origin of the placeholder cube is in the center, so this may not be needed
            
        }
        timer = noriSheetScript.SlapDelayTimer; //Sets the timer to the SlapDelayTimer, this must be done on every start

        noriSheetScript.AnimationController.SetAnimationBool("IdleState", false);
        noriSheetScript.AnimationController.SetAnimationBool("MovementState", false);
        noriSheetScript.AnimationController.SetAnimationBool("SweepAttackState", false);
        noriSheetScript.AnimationController.SetAnimationBool("SlapAttackState", false);

        noriSheetScript.AudioManager.PlaySound(noriSheetScript.AudioManager.AudioClips[3], false);
        bHasDealtDamage = false;
    }

    public override void UpdateState(GameObject noriSheet, NavMeshAgent meshAgent)
    {
        if(noriSheetScript.EnemyStats.IsStunned)
        {
            noriSheetScript.currentState = noriSheetScript.movementState;
            noriSheetScript.currentState.StartState(noriSheet, meshAgent);
        }

        noriSheetScript.AnimationController.SetAnimationBool("MovementState", false);
        noriSheetScript.AnimationController.SetAnimationBool("SlapAttackState", true);

        if (timer > 0f) //Only decrement the timer if it is above 0
        {
            timer -= Time.deltaTime; //Decrease the timer by deltaTime
        }

        if(timer <= 0f) //If the timer is 0 then the enemy can attack
        {
            
            //First perform the slap attack then perform the AOE effect
            if(Physics.CheckSphere(noriTransform.position + verticalOffset, 1f, playerLayerMask) && !bHasDealtDamage) //Perform a raycast to see if the player is hit by the slap
            {
                //Debug.Log("Hit the player!"); //Debug that the player was hit
                noriSheetScript.EnemyStats.PlayerStats.TakeDamage((int)damage + noriSheetScript.EnemyStats.EnemyDamageMod); //Decrease the current health by the damage amount
                bHasDealtDamage = true;
            }

            //Perform the AOE attack
            aoePosition = (noriTransform.position + verticalOffset) + (noriTransform.forward * 0.5f); //Set the AOE attacks origin position, this is X units forwards in relation to the Nori Sheet's position and rotation
            //Debug.DrawLine(noriTransform.position - verticalOffset, aoePosition, Color.red); //For debug purposes only, this shows where the AOE will originate from and should be a direct line between the Nori Sheet and the AOE position
            if(Physics.CheckSphere(aoePosition, 1f, playerLayerMask)) //Performs a checkSphere check, the sphere is 2 units in diameter and will only register a collision with the player
            {
                //Debug.Log("AOE hit the player"); //Debugs that the player has been hit
                noriSheetScript.EnemyStats.PlayerStats.StunPlayer(1, false);
            }

            if(noriSheetScript.AnimationController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
            {
                noriSheetScript.currentState = noriSheetScript.movementState; //Set the currentState to the Movement State
                noriSheetScript.currentState.StartState(noriSheet, meshAgent); //Start the Movement State, this stops the Nori Sheet from being stuck in this state
            }
            
        }    
    }

    public override void FixedUpdateState(GameObject noriSheet, NavMeshAgent meshAgent)
    {

    }
}
