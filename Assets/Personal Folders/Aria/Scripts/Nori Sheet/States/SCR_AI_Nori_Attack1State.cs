using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Nori_Attack1State : SCR_AI_NoriBaseState
{
    SCR_AI_NoriSheet noriSheetScript;
    Transform noriTransform;
    float damage;
    float angleOffset;
    float range;
    RaycastHit hit;
    Ray ray;
    float timer;
    bool bHasDealtDamage;

    public override void StartState(GameObject noriSheet, NavMeshAgent meshAgent)
    {
        //Debug.Log("Nori Sheet Sweep Attack State");
        if(noriSheetScript == null) //If noriSheetScript is null then this is the first time this state has been run in this instance
        {
            noriSheetScript = noriSheet.GetComponent<SCR_AI_NoriSheet>(); //Grab the NoriSheet script from the Nori Sheet object
            noriTransform = noriSheet.GetComponent<Transform>(); //Grab the transform
            //healthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_PlayerHealth>(); //Find and grab the temp health script from the player, this will need to be changed

            damage = noriSheetScript.sweepDamage; //Set the damage value to a local variable
            range = noriSheetScript.sweepAttackRange; //Set the range value to a local variable
        }

        //These values need to be reset every time this state is started
        angleOffset = -noriSheetScript.SweepAngle; //Set the angle offset to the inverse of the SweepAngle. This is so that the angle offset starts at a negative value but can also be reset when need be
        timer = noriSheetScript.SweepDelayTimer; //Set the delay timer, this gives the player a small window to dodge out of the way of the attack

        noriSheetScript.AnimationController.SetAnimationBool("IdleState", false);
        noriSheetScript.AnimationController.SetAnimationBool("MovementState", false);
        noriSheetScript.AnimationController.SetAnimationBool("SweepAttackState", false);
        noriSheetScript.AnimationController.SetAnimationBool("SlapAttackState", false);

        noriSheetScript.AudioManager.PlaySound(noriSheetScript.AudioManager.AudioClips[6], false);
        bHasDealtDamage = false;
    }

    public override void UpdateState(GameObject noriSheet, NavMeshAgent meshAgent)
    {
        //Enemy needs to check for the player in a X degree arc (from Y degrees to -Y degrees) within a range of Z units
        //If the player is found in that sweep then deal damage
        //If the player is not found then exit the attack state

        if(noriSheetScript.EnemyStats.IsStunned)
        {
            //Reset all components of the attack and return to move state
            noriSheetScript.currentState = noriSheetScript.movementState; 
            noriSheetScript.currentState.StartState(noriSheet, meshAgent);
            return;
        }

        noriSheetScript.AnimationController.SetAnimationBool("MovementState", false);
        noriSheetScript.AnimationController.SetAnimationBool("SweepAttackState", true);

        if (timer > 0f) //Only decrease the timer if it is greater than 0f
        {
            timer -= Time.deltaTime; //Decrease the timer by deltaTime
        }
        
        if(timer <= 0f) //If the timer has reached 0 then
        {
            

            if (angleOffset <= noriSheetScript.SweepAngle) //Check if the angle offset is less than the max Sweep Angle
            {
                //Code adapted from Owiegand, 2014
                ray.origin = new Vector3(noriTransform.position.x, noriTransform.position.y + 0.5f, noriTransform.position.z); //Set the origin of the Ray to the base of the Nori Sheet
                ray.direction = Quaternion.AngleAxis(angleOffset, noriTransform.up) * noriTransform.forward; //Calculate the direction of the ray using the local forward and up of the Nori Sheet and the angleOffset
                //ray.direction = transform.TransformDirection(ray.direction);
                //End of Adapted Code

                Debug.DrawRay(ray.origin, (ray.direction + noriTransform.forward) * 2.5f, Color.green); //For debug only, allows for the ray to be seen in the Scene view
                angleOffset++; //Increase the angleOffset by one
            }

            if (Physics.Raycast(ray, out hit, range, noriSheetScript.EnemyStats.PlayerLayerMask) && !bHasDealtDamage) //Check if the raycast has intersected a player
            {
                //Debug.Log("Hit " + hit.collider.name); //Debug that the player has been hit
                noriSheetScript.EnemyStats.PlayerStats.TakeDamage((int)damage + noriSheetScript.EnemyStats.EnemyDamageMod); //Decrease the player's health by the damage value
                bHasDealtDamage = true;
                /*
                noriSheetScript.currentState = noriSheetScript.movementState; //Set the current state to the Movement State
                noriSheetScript.currentState.StartState(noriSheet, meshAgent); //Start the Movement State
                */
            }

            if(noriSheetScript.AnimationController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
            {
                noriSheetScript.currentState = noriSheetScript.movementState; //Set the current state to the Movement State
                noriSheetScript.currentState.StartState(noriSheet, meshAgent); //Start the Movement State
            }
        }
    }

    public override void FixedUpdateState(GameObject noriSheet, NavMeshAgent meshAgent)
    {
        
    }
}
