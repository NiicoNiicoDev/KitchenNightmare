using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RSC_Idle : SCR_AI_RSC_BaseState
{
    private SCR_AI_RottenSalmonChunk salmonChunkScript;

    public override void StartState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        Debug.Log("Entered Idle State");
        salmonChunkScript = salmonChunk.GetComponent<SCR_AI_RottenSalmonChunk>();

        salmonChunkScript.salmonAnimator.SetBool("isMoving", false);

        navMeshAgent.isStopped = true;
    }

    public override void UpdateState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        salmonChunk.transform.LookAt(salmonChunkScript.player.transform);
    }

    public override void ExitState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        
    }
}
