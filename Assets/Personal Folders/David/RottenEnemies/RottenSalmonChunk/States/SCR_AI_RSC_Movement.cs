using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RSC_Movement : SCR_AI_RSC_BaseState
{
    private SCR_AI_RottenSalmonChunk salmonChunkScript;

    public override void StartState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        salmonChunkScript = salmonChunk.GetComponent<SCR_AI_RottenSalmonChunk>();

        salmonChunkScript.salmonAnimator.SetBool("isMoving", true);

        navMeshAgent.isStopped = false;

        navMeshAgent.speed = salmonChunkScript.movementSpeed;
    }
    //TODO: Create a trail of poison where the salmon has been standing
    public override void UpdateState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        Vector3 newPosition = Vector3.Slerp(salmonChunkScript.player.transform.position + new Vector3(-4f, 0f, 0f), salmonChunkScript.player.transform.position + new Vector3(4f, 0f, 0f), salmonChunkScript.movementSpeed);

        navMeshAgent.SetDestination(newPosition);
    }

    public override void ExitState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        
    }
}
