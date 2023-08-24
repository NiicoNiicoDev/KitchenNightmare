using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SCR_AI_RNS_Idle : SCR_AI_RNS_BaseStates
{
    private SCR_AI_RNS noriSheetScript;

    public override void StartState(GameObject noriSheet, NavMeshAgent navMeshAgent)
    {
        noriSheetScript = noriSheet.GetComponent<SCR_AI_RNS>();

        navMeshAgent.isStopped = true;

        noriSheetScript.noriAnimator.SetBool("isMoving", false);
    }

    public override void UpdateState(GameObject noriSheet, NavMeshAgent navMeshAgent)
    {
        
    }
}
