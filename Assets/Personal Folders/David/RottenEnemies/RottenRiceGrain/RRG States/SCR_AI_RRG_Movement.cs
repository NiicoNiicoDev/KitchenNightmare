using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RRG_Movement : SCR_AI_RRG_BaseStates
{
    private SCR_AI_RottenRiceGrain riceGrainScript;

    private SCR_EnemyStats stunScript;

    public override void StartState(GameObject rottenRiceGrain, NavMeshAgent navMeshAgent)
    {
        Debug.Log("Movement State");
        riceGrainScript = rottenRiceGrain.GetComponent<SCR_AI_RottenRiceGrain>();
        stunScript = rottenRiceGrain.GetComponent<SCR_EnemyStats>();
        navMeshAgent.speed = riceGrainScript.movementSpeed;
    }

    public override void UpdateState(GameObject rottenRiceGrain, NavMeshAgent navMeshAgent)
    {
        if (stunScript.IsStunned)
        {
            navMeshAgent.isStopped = true;
            riceGrainScript.riceGrainAnimator.SetBool("IsMoving", false);
            return;
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(riceGrainScript.player.transform.position);

        riceGrainScript.riceGrainAnimator.SetBool("IsMoving", true);
    }
}
