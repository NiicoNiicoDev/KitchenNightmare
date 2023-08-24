using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RSC_StunCloudAttack : SCR_AI_RSC_BaseState
{
    private SCR_AI_RottenSalmonChunk salmonChunkScript;

    public override void StartState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        salmonChunkScript = salmonChunk.GetComponent<SCR_AI_RottenSalmonChunk>();

        navMeshAgent.isStopped = true;

        salmonChunkScript.salmonAnimator.SetTrigger("Attack1");

        salmonChunkScript.StartRSCCoroutine(ReleaseStunCloud());
    }

    public override void UpdateState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        
    }

    public override void ExitState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        salmonChunkScript.CancelRSCCoroutine();
        salmonChunkScript.stunCloudParticles.SetActive(false);
        salmonChunkScript.canChangeState = true;
    }

    IEnumerator ReleaseStunCloud()
    {
        salmonChunkScript.stunCloudParticles.SetActive(true);
        yield return new WaitForSeconds(2f);
        salmonChunkScript.stunCloudParticles.SetActive(false);
        salmonChunkScript.EnterState(salmonChunkScript.flee);
    }
}
