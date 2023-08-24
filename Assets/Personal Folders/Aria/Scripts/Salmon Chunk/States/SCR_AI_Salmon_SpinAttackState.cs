using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Salmon_SpinAttackState : SCR_AI_SalmonBaseStates
{
    SCR_AI_SalmonChunk salmonChunkScript;
    SCR_SpoonSpin spoonSpin;
    float readyTimer;
    float spinRecovery;
    float damage;
    float spinDex; //Spin Index = spinDex
    Vector3 salmonRot;
    GameObject aoeObject;

    bool bSpinReady;
    bool bStartedSpin;
    bool bCompletedSpin;

    public override void StartState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {
        Debug.Log("Salmon Spin Attack State");
        if(salmonChunkScript == null) //If the salmon chunk script is null then this is the first time this script has run
        {
            salmonChunkScript = salmonChunk.GetComponent<SCR_AI_SalmonChunk>(); //Gets the SCR_AI_SalmonChunk script from the salmonChunk object
            spoonSpin = salmonChunk.GetComponentInChildren<SCR_SpoonSpin>(true); //Gets the SCR_SpoonSpin component from the Spoon child object
            damage = salmonChunkScript.SpinDamage; //Sets the damage value so it can be passed over to the spoonSpin, this is saved here so it is only accessed once
        }

        //Sets the ready and cooldown timer
        readyTimer = salmonChunkScript.SpinReadyTimer;
        spinRecovery = salmonChunkScript.SpinRecoveryTimer;

        //Stops the Mesh Agent from moving
        meshAgent.isStopped = true;

        //Sets all the necessary flags to false, this must be done here as these flags are needed every time the state is run
        bSpinReady = false;
        bCompletedSpin = false;
        bStartedSpin = false;

        //Sets the spinDex to 0, again this must be done every time the state is run
        spinDex = 0;

        aoeObject = MonoBehaviour.Instantiate(salmonChunkScript.EnemyStats.AOEObject, salmonChunk.transform.localPosition, salmonChunk.transform.localRotation);
        aoeObject.transform.localScale = new Vector3(1.5f, 1f, 1.5f);

        salmonChunkScript.AnimationController.SetAnimationBool("MovementState", false);
        salmonChunkScript.AnimationController.SetAnimationBool("SpinAttackState", true);

        salmonChunkScript.AudioManager.PlaySound(salmonChunkScript.AudioManager.AudioClips[7], false);
    }

    public override void UpdateState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {
        if(salmonChunkScript.EnemyStats.IsStunned)
        {
            spoonSpin.EndSpin();
            if(aoeObject) MonoBehaviour.Destroy(aoeObject);

            salmonChunkScript.currentState = salmonChunkScript.movementState;
            salmonChunkScript.currentState.StartState(salmonChunk, meshAgent);
        }

        salmonRot = salmonChunk.transform.localRotation.eulerAngles; //Gets the Salmon Chunk's rotation

        bSpinReady = spoonSpin.bIsReady; //Gets the state of the Is Ready flag from the SCR_SpoonSpin script

        if (!bSpinReady && bStartedSpin) //If the spin is not ready but has started then the player has been hit
        {
            bCompletedSpin = true; //Set the bCompletedSpin flag to true
        }

        if (readyTimer > 0) //Only decrement the timer if it is above 0
        {
            readyTimer -= Time.deltaTime;
        }

        if(readyTimer <= 0f && !bSpinReady && !bStartedSpin) //if the timer has reached 0 and the spin is not ready and has not started
        {
            spoonSpin.ReadySpin(damage + salmonChunkScript.EnemyStats.EnemyDamageMod); //Ready the Spin
            bStartedSpin = true; //Set the Start Spin flag to true; the spin has started
        }

        /*
        //If the Spin is Ready then start spinning
        if(bSpinReady) //If the Spin is ready
        {
            salmonRot.y += spinDex * Time.deltaTime * 1.2f; //Update the Y rotation of the Salmon by the spinDex. This is not framerate dependant, but is multiplied by two to make sure that a full rotation is completed 
            salmonChunk.transform.localRotation = Quaternion.Euler(salmonRot);

            spinDex++; //Increase the spinDex by one
            //Debug.Log("SpinDex: " + spinDex);
            if(spinDex == 360f) //if the spinDex is 360
            {
                bCompletedSpin = true; //The spin is complete
                spoonSpin.EndSpin(); //End the spin
            }
            
        }
        */

        if(salmonChunkScript.AnimationController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && !salmonChunkScript.AnimationController.animator.IsInTransition(0))
        {
            //Animation has ended
            bCompletedSpin = true;
            spoonSpin.EndSpin();
            salmonChunkScript.AnimationController.SetAnimationBool("IdleState", true);
            salmonChunkScript.AnimationController.SetAnimationBool("SpinAttackState", false);
        }

        //If nothing is hit for the duration of the spin, then end it in this script
        if (bCompletedSpin) //If the spin is finished then start the recovery timer
        {
            if (aoeObject) MonoBehaviour.Destroy(aoeObject);
            spinRecovery -= Time.deltaTime;
            spoonSpin.EndSpin();
            if (spinRecovery <= 0f)
            {
                //Enter the movement state again
                salmonChunkScript.currentState = salmonChunkScript.movementState;
                salmonChunkScript.currentState.StartState(salmonChunk, meshAgent);
            }
        }
        
    }

    public override void FixedUpdateState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {

    }
}
