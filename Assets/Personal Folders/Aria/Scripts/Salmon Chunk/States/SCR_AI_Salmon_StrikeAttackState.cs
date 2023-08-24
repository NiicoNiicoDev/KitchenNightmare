using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Salmon_StrikeAttackState : SCR_AI_SalmonBaseStates
{
    SCR_AI_SalmonChunk salmonChunkScript;
    SCR_SpoonStrike spoonStrike;
    Rigidbody salmonRB;
    GameObject aoeObject;

    float jumpForce;
    float jumpHeight;
    float diveForce;
    float startHeight;

    float chargeTimer;
    float cooldownTimer;

    bool bBegunDive;

    public override void StartState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {
        //Debug.Log("Salmon Strike Attack State");
        if(salmonChunkScript == null)
        {
            salmonChunkScript = salmonChunk.GetComponent<SCR_AI_SalmonChunk>();
            spoonStrike = salmonChunk.GetComponentInChildren<SCR_SpoonStrike>();
            salmonRB = salmonChunk.GetComponent<Rigidbody>();
            

            jumpForce = salmonChunkScript.JumpForce;
            diveForce = salmonChunkScript.DiveForce;
        }

        startHeight = salmonChunk.transform.localPosition.y;
        jumpHeight = startHeight + salmonChunkScript.JumpHeight;

        chargeTimer = salmonChunkScript.StrikeReadyTimer;
        cooldownTimer = salmonChunkScript.StrikeCooldownTimer;
        bBegunDive = false;

        //Salmon Needs to Jump High
        //Target player's position
        //Ready the Spoon
        //Zoom to location
        //Strike and wait for recovery
        //Reset spoon and exit state

        aoeObject = MonoBehaviour.Instantiate(salmonChunkScript.EnemyStats.AOEObject, salmonChunk.transform.localPosition, salmonChunk.transform.localRotation);
        aoeObject.transform.localScale = new Vector3(1.5f, 1f, 1.5f);

        salmonChunkScript.AnimationController.SetAnimationBool("MovementState", false);
        salmonChunkScript.AnimationController.SetAnimationBool("StrikeAttackState", true);

        salmonChunkScript.AudioManager.PlaySound(salmonChunkScript.AudioManager.AudioClips[5], false);
    }

    public override void UpdateState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {
        if(salmonChunkScript.EnemyStats.IsStunned && salmonChunk.transform.localPosition.y <= startHeight + 0.5f)
        {
            salmonChunk.transform.localPosition = new Vector3(salmonChunk.transform.localPosition.x, startHeight, salmonChunk.transform.localPosition.z);
            meshAgent.enabled = true;
            salmonRB.useGravity = true;
            if (aoeObject) MonoBehaviour.Destroy(aoeObject);
            salmonChunkScript.currentState = salmonChunkScript.movementState;
            salmonChunkScript.currentState.StartState(salmonChunk, meshAgent);
        }

        meshAgent.enabled = false;
        salmonRB.useGravity = false;
        if (salmonChunk.transform.localPosition.y < jumpHeight && !bBegunDive)
        {
            salmonChunk.transform.localPosition += new Vector3(0f, jumpForce, 0f) * Time.deltaTime;
        }
        
        if(salmonChunk.transform.localPosition.y >= jumpHeight)
        {
            if(!spoonStrike.bIsReady)
            {
                spoonStrike.ReadyStrike(salmonChunkScript.StrikeDamage + salmonChunkScript.EnemyStats.EnemyDamageMod);
            }
            else
            {
                chargeTimer -= Time.deltaTime;
                if (chargeTimer <= 0f)
                {
                    bBegunDive = true;
                    //Debug.Log("Begun Dive");
                }
            }
        }
        
        if(bBegunDive)
        {
            if (salmonChunk.transform.localPosition.y >= startHeight + 0.1f)
            {
                salmonChunk.transform.localPosition -= new Vector3(0f, diveForce, 0f) * Time.deltaTime;
            }
            else
            {
                //Debug.Log("Dive Finished");
                spoonStrike.StruckGround();
                cooldownTimer -= Time.deltaTime;
                if(aoeObject) MonoBehaviour.Destroy(aoeObject);
                salmonChunk.transform.localPosition = new Vector3(salmonChunk.transform.localPosition.x, startHeight, salmonChunk.transform.localPosition.z);
                salmonChunkScript.AnimationController.SetAnimationBool("StrikeAttackState", false);
                salmonChunkScript.AnimationController.SetAnimationBool("IdleState", true);
                if (cooldownTimer <= 0f)
                {
                    spoonStrike.EndStrike();
                    meshAgent.enabled = true;
                    salmonRB.useGravity = true;

                    salmonChunkScript.currentState = salmonChunkScript.movementState;
                    salmonChunkScript.currentState.StartState(salmonChunk, meshAgent);
                }
            }
        }
    }

    public override void FixedUpdateState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {

    }
}
