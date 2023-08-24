using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RSC_Flee : SCR_AI_RSC_BaseState
{
    private SCR_AI_RottenSalmonChunk salmonChunkScript;

    public override void StartState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        salmonChunkScript = salmonChunk.GetComponent<SCR_AI_RottenSalmonChunk>();

        navMeshAgent.isStopped = false;

        salmonChunkScript.StartRSCCoroutine(StopFleeing());
    }

    public override void UpdateState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        salmonChunk.transform.rotation = Quaternion.LookRotation(salmonChunk.transform.position - salmonChunkScript.player.transform.position);

        Vector3 newPosition = salmonChunk.transform.position + salmonChunk.transform.forward * salmonChunkScript.movementSpeed;

        navMeshAgent.SetDestination(newPosition);
    }

    public override void ExitState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        salmonChunkScript.CancelRSCCoroutine();

        salmonChunkScript.canChangeState = true;
    }

    IEnumerator StopFleeing()
    {
        yield return new WaitForSeconds(salmonChunkScript.fleeTime);

        salmonChunkScript.canChangeState = true;
    }
}
