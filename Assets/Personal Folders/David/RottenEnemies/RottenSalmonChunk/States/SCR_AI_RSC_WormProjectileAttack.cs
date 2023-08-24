using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RSC_WormProjectileAttack : SCR_AI_RSC_BaseState
{
    private SCR_AI_RottenSalmonChunk salmonChunkScript;

    public override void StartState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        salmonChunkScript = salmonChunk.GetComponent<SCR_AI_RottenSalmonChunk>();

        navMeshAgent.isStopped = true;

        salmonChunkScript.salmonAnimator.SetTrigger("Attack2");

        salmonChunkScript.StartRSCCoroutine(FireWorms(salmonChunk));
    }

    public override void UpdateState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        
    }

    public override void ExitState(GameObject salmonChunk, NavMeshAgent navMeshAgent)
    {
        salmonChunkScript.CancelRSCCoroutine();

        salmonChunkScript.canChangeState = true;
    }

    IEnumerator FireWorms(GameObject salmonChunk)
    {
        yield return new WaitForSeconds(3.5f);
        int noOfWorms = Random.Range(1, 4);

        while(noOfWorms > 0)
        {
            Quaternion rotation = Quaternion.Euler(-90f, Random.Range(salmonChunk.transform.rotation.y - 45f, salmonChunk.transform.rotation.y + 45f), 0f);
            MonoBehaviour.Instantiate(salmonChunkScript.wormProjectiles, salmonChunk.transform.position + new Vector3(0f, 1.5f, 0f), rotation);
            noOfWorms--;
            yield return null;
        }
        yield return new WaitForSeconds(1.9f);
        salmonChunkScript.canChangeState = true;
    }
}
