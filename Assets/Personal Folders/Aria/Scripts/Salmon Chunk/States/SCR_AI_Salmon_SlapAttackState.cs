using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_Salmon_SlapAttackState : SCR_AI_SalmonBaseStates
{
    SCR_AI_SalmonChunk salmonChunkScript;
    Transform salmonTransform;
    float attackDamage;
    float timer;
    float readyTimer;
    bool bHasCompletedAttack;
    bool bHasDealtDamage;
    LayerMask playerLayerMask;
    RaycastHit hit;
    //Vector3 verticalOffset;

    public override void StartState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {
        //Debug.Log("Salmon Slap Attack State");
        if(salmonChunkScript == null)
        {
            salmonChunkScript = salmonChunk.GetComponent<SCR_AI_SalmonChunk>();
            attackDamage = salmonChunkScript.SlapDamage;
            //healthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_PlayerHealth>();
            playerLayerMask = salmonChunkScript.EnemyStats.PlayerLayerMask;
            salmonTransform = salmonChunk.GetComponent<Transform>();
        }

        if(salmonChunkScript.bEnraged)
        {
            attackDamage += 10f;
        }
        timer = salmonChunkScript.SlapRecoveryTimer;
        readyTimer = salmonChunkScript.SlapReadyTimer;

        meshAgent.isStopped = true;
        bHasCompletedAttack = false;

        salmonChunkScript.AnimationController.SetAnimationBool("IdleState", false);
        salmonChunkScript.AnimationController.SetAnimationBool("MovementState", false);
        salmonChunkScript.AnimationController.SetAnimationBool("SlapAttackState", true);

        salmonChunkScript.AudioManager.PlaySound(salmonChunkScript.AudioManager.AudioClips[3], false);
        bHasDealtDamage = false;
    }

    public override void UpdateState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {
        if(salmonChunkScript.EnemyStats.IsStunned)
        {
            bHasCompletedAttack = true;
            salmonChunkScript.currentState = salmonChunkScript.movementState;
            salmonChunkScript.currentState.StartState(salmonChunk, meshAgent);
        }

        if(readyTimer > 0)
        {
            readyTimer -= Time.deltaTime;
        }

        //Debug.Log("Salmon Chunk readying attack");

        //Try and slap the player with a spoon
        if(!bHasCompletedAttack && readyTimer <= 0f)
        {
            //Debug.Log("Salmon Chunk attacking");
            if(Physics.CheckSphere((salmonTransform.position + (Vector3.up * 0.5f)), 1.5f, playerLayerMask) && !bHasDealtDamage)
            {
                //Debug.Log("Hit the Player");
                salmonChunkScript.EnemyStats.PlayerStats.TakeDamage((int)attackDamage + salmonChunkScript.EnemyStats.EnemyDamageMod);
                bHasDealtDamage = true;
            }
            bHasCompletedAttack = true;
        }

        //Cooldown for 1 second
        if(bHasCompletedAttack)
        {
            //Debug.Log("Salmon Chunk is recovering");
            timer -= Time.deltaTime;
            if(timer <= 0f && salmonChunkScript.AnimationController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
            {
                salmonChunkScript.currentState = salmonChunkScript.movementState;
                salmonChunkScript.currentState.StartState(salmonChunk, meshAgent);
            }
        }
    }

    public override void FixedUpdateState(GameObject salmonChunk, NavMeshAgent meshAgent)
    {

    }
}
