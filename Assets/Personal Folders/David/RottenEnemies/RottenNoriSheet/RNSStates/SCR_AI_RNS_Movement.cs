using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RNS_Movement : SCR_AI_RNS_BaseStates
{
    private SCR_AI_RNS noriSheetScript;

    private SCR_EnemyStats stunScript;

    private Vector3 leaderOffset;

    public override void StartState(GameObject noriSheet, NavMeshAgent navMeshAgent)
    {
        noriSheetScript = noriSheet.GetComponent<SCR_AI_RNS>();

        stunScript = noriSheetScript.GetComponent<SCR_EnemyStats>();

        noriSheetScript.noriAnimator.SetBool("isMoving", true);

        navMeshAgent.speed = noriSheetScript.noriSpeed;

        if (!noriSheetScript.leader)
        {
            leaderOffset = new Vector3(Random.Range(-3.0f, 3.0f), 0f, Random.Range(-3.0f, 3.0f));
        }
    }

    public override void UpdateState(GameObject noriSheet, NavMeshAgent navMeshAgent)
    {
        if (stunScript.IsStunned)
        {
            navMeshAgent.isStopped= true;

            noriSheetScript.noriAnimator.SetBool("isMoving", false);

            return;
        }

        navMeshAgent.isStopped= false;

        if (noriSheetScript.leader)
        {
            navMeshAgent.SetDestination(noriSheetScript.player.transform.position);
        }
        else
        {
            navMeshAgent.SetDestination(noriSheetScript.leadNori.transform.position + leaderOffset);
        }
    }
}
