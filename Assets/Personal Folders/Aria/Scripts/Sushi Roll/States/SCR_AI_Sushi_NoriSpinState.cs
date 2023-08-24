using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Sushi_NoriSpinState : SCR_AI_SushiBaseStates
{
    SCR_AI_SushiRoll sushiRollScript;
    SCR_NoriSpin noriSpin;

    Transform sushiRollTransform;
    Transform playerTransform;

    int damage;
    Quaternion quaternionRotation;

    float spinDuration;
    float cooldownTimer;
    float spinSpeed;
    float defaultSpeed;
    float timeoutLength;

    float newRot;
    float preAttackDelay;

    bool bCanStartSpinning;
    bool bStartCooldown;

    public override void StartState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {
        //Debug.Log("Sushi Roll Nori Spin State");
        if (sushiRollScript == null)
        {
            sushiRollScript = sushiRoll.GetComponent<SCR_AI_SushiRoll>();
            noriSpin = sushiRoll.GetComponentInChildren<SCR_NoriSpin>();
            sushiRollTransform = sushiRoll.transform;
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

            defaultSpeed = meshAgent.speed;
            spinSpeed = sushiRollScript.SpinSpeed;
            damage = sushiRollScript.NoriSpinDamage;
        }

        spinDuration = sushiRollScript.SpinDuration;
        cooldownTimer = sushiRollScript.RecoveryTimer;
        bStartCooldown = false;
        preAttackDelay = 1f;
        noriSpin.ReadySpin(damage + sushiRollScript.EnemyStats.EnemyDamageMod, spinDuration);
        bCanStartSpinning = false;
        meshAgent.isStopped = false;
        timeoutLength = 5f;

        sushiRollScript.AudioManager.PlaySound(sushiRollScript.AudioManager.AudioClips[7], false);

        Debug.Log("Starting Pos: " + sushiRollScript.StartingPosition);
    }

    public override void UpdateState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {
        if(sushiRollScript.GameManager.bPlayerDead)
        {
            //End the spin
            //Debug.Log("Ending Spin");
            meshAgent.speed = defaultSpeed;
            meshAgent.isStopped = true;
            noriSpin.EndSpin();
            sushiRollScript.currentState = sushiRollScript.idleState;
            sushiRollScript.currentState.StartState(sushiRoll, meshAgent);
            return;
        }

        preAttackDelay -= Time.deltaTime;
        
        if(preAttackDelay <= 0f && !bCanStartSpinning)
        {
            if(sushiRollTransform.localPosition.magnitude > (sushiRollScript.StartingPosition.magnitude + 0.2f))
            {
                //Set the destination
                meshAgent.destination = sushiRollScript.WorldStartingPos;
                timeoutLength -= Time.deltaTime;
                if(timeoutLength <= 0f)
                {
                    meshAgent.speed = defaultSpeed;
                    noriSpin.EndSpin();
                    sushiRollScript.currentState = sushiRollScript.movementState;
                    sushiRollScript.currentState.StartState(sushiRoll, meshAgent);
                    return;
                }
            }
            else
            {
                //In the centre of the arena
                //Start spinning
                meshAgent.speed = spinSpeed;
                bCanStartSpinning = true;
                meshAgent.isStopped = true;
            }
            
        }

        if(bCanStartSpinning && !bStartCooldown)
        {
            //Spin round and round while moving slowly towards the player
            //Once spin duration reaches 0 end the spin and begin cooldown
            //Ending the spin consists of stopping movement, and calling the EndSpin function in SCR_NoriSpin.cs
            //Resetting values and entering the movement state is done after the cooldown
            spinDuration -= Time.deltaTime;
            if(spinDuration <= 0f)
            {
                //Spin is over and the cooldown can start
                bStartCooldown = true;
                meshAgent.isStopped = true;
                noriSpin.EndSpin();
            }
            else
            {
                //Update the rotation of the SushiRoll
                newRot = (sushiRollTransform.localRotation.y + 400f) * Time.deltaTime; //Adding the rotation amount to the current rotation before multiplying by deltaTime is essential otherwise the Sushi will spin for a second but then stop and jitter
                quaternionRotation = Quaternion.Euler(sushiRollTransform.localRotation.x, newRot, sushiRollTransform.localRotation.z);
                sushiRollTransform.localRotation *= quaternionRotation;
                
                //meshAgent.SetDestination(playerTransform.position);
            }
            

        }

        if(bStartCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if(cooldownTimer <= 0f)
            {
                //Reset the movement speed back to its default value
                //Enter the movement state again, meshAgent.isStopped does not need to be set here as it is set in the MovementState
                meshAgent.speed = defaultSpeed;
                meshAgent.isStopped = false;
                sushiRollScript.currentState = sushiRollScript.movementState;
                sushiRollScript.currentState.StartState(sushiRoll, meshAgent);
            }
        }
    }

    public override void FixedUpdateState(GameObject sushiRoll, NavMeshAgent meshAgent)
    {

    }
}
